/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - HTTP Helper
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Helper para operações HTTP com retry, backoff exponencial e
tratamento de rate limiting (429, 5xx).

FUNCIONALIDADES:
- Retry com backoff configurável (Exponential, Linear, Fixed)
- Tratamento de header Retry-After
- Logging detalhado de requisições
- Métricas de latência

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace QuattoAPIClient.Source.Helpers
{
    /// <summary>
    /// Helper para operações HTTP com retry e backoff
    /// </summary>
    public class HttpHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IDTSComponentMetaData100 _metadata;
        private readonly RetryPolicy _retryPolicy;
        private readonly List<int> _latencies;

        public int TotalRequests { get; private set; }
        public int SuccessfulRequests { get; private set; }
        public int FailedRequests { get; private set; }
        public int RetriedRequests { get; private set; }
        public int ThrottledRequests { get; private set; }

        public HttpHelper(
            HttpClient httpClient,
            IDTSComponentMetaData100 metadata,
            RetryPolicy retryPolicy)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _retryPolicy = retryPolicy ?? new RetryPolicy();
            _latencies = new List<int>();

            TotalRequests = 0;
            SuccessfulRequests = 0;
            FailedRequests = 0;
            RetriedRequests = 0;
            ThrottledRequests = 0;
        }

        /// <summary>
        /// Executa requisição HTTP GET com retry automático
        /// </summary>
        public HttpResponse ExecuteGetWithRetry(string url, Guid correlationId)
        {
            return ExecuteWithRetry(
                () => _httpClient.GetAsync(url),
                url,
                "GET",
                correlationId
            );
        }

        /// <summary>
        /// Executa requisição HTTP POST com retry automático
        /// </summary>
        public HttpResponse ExecutePostWithRetry(
            string url,
            HttpContent content,
            Guid correlationId)
        {
            return ExecuteWithRetry(
                () => _httpClient.PostAsync(url, content),
                url,
                "POST",
                correlationId
            );
        }

        /// <summary>
        /// Executa requisição com política de retry configurável
        /// </summary>
        private HttpResponse ExecuteWithRetry(
            Func<Task<HttpResponseMessage>> requestFunc,
            string url,
            string method,
            Guid correlationId)
        {
            int attempt = 0;
            bool cancelEvent = false;
            DateTime requestStart = DateTime.UtcNow;
            Exception? lastException = null;

            while (attempt < _retryPolicy.MaxAttempts)
            {
                attempt++;
                TotalRequests++;

                try
                {
                    DateTime attemptStart = DateTime.UtcNow;

                    // Execute request
                    HttpResponseMessage response = requestFunc().GetAwaiter().GetResult();

                    DateTime attemptEnd = DateTime.UtcNow;
                    int latencyMs = (int)(attemptEnd - attemptStart).TotalMilliseconds;
                    _latencies.Add(latencyMs);

                    int statusCode = (int)response.StatusCode;

                    // Log request
                    _metadata.FireInformation(0, "HttpHelper",
                        $"[{correlationId}] {method} {url} → {statusCode} ({latencyMs}ms, attempt {attempt})",
                        "", 0, ref cancelEvent);

                    // Handle rate limiting (429) or server errors (5xx)
                    if (statusCode == 429 || statusCode >= 500)
                    {
                        if (statusCode == 429)
                            ThrottledRequests++;

                        string responseBody = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        if (attempt >= _retryPolicy.MaxAttempts)
                        {
                            FailedRequests++;

                            return new HttpResponse
                            {
                                StatusCode = statusCode,
                                Success = false,
                                Body = responseBody,
                                Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                                LatencyMs = latencyMs,
                                Attempts = attempt,
                                ErrorMessage = $"Max retries ({_retryPolicy.MaxAttempts}) exceeded. Status: {statusCode}"
                            };
                        }

                        // Calculate wait time
                        int waitMs = CalculateBackoffDelay(attempt);

                        // Check Retry-After header
                        if (response.Headers.TryGetValues("Retry-After", out var retryAfterValues))
                        {
                            string? retryAfter = retryAfterValues.FirstOrDefault();
                            if (int.TryParse(retryAfter, out int retryAfterSeconds))
                            {
                                waitMs = Math.Min(retryAfterSeconds * 1000, 300000); // Max 5 min
                            }
                        }

                        _metadata.FireWarning(0, "HttpHelper",
                            $"[{correlationId}] Status {statusCode}. Retry {attempt}/{_retryPolicy.MaxAttempts} in {waitMs}ms...",
                            "", 0);

                        RetriedRequests++;
                        Thread.Sleep(waitMs);
                        continue;
                    }

                    // Success response
                    if (response.IsSuccessStatusCode)
                    {
                        SuccessfulRequests++;

                        string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        return new HttpResponse
                        {
                            StatusCode = statusCode,
                            Success = true,
                            Body = body,
                            Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                            LatencyMs = latencyMs,
                            Attempts = attempt
                        };
                    }
                    else
                    {
                        // Client error (4xx) - don't retry
                        FailedRequests++;

                        string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

                        return new HttpResponse
                        {
                            StatusCode = statusCode,
                            Success = false,
                            Body = body,
                            Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                            LatencyMs = latencyMs,
                            Attempts = attempt,
                            ErrorMessage = $"HTTP {statusCode}: {response.ReasonPhrase}"
                        };
                    }
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;

                    _metadata.FireWarning(0, "HttpHelper",
                        $"[{correlationId}] HTTP error on attempt {attempt}/{_retryPolicy.MaxAttempts}: {ex.Message}",
                        "", 0);

                    if (attempt >= _retryPolicy.MaxAttempts)
                    {
                        FailedRequests++;
                        break;
                    }

                    int waitMs = CalculateBackoffDelay(attempt);
                    RetriedRequests++;
                    Thread.Sleep(waitMs);
                }
                catch (TaskCanceledException ex)
                {
                    lastException = ex;

                    _metadata.FireError(0, "HttpHelper",
                        $"[{correlationId}] Request timeout on attempt {attempt}: {ex.Message}",
                        "", 0, out cancelEvent);

                    if (attempt >= _retryPolicy.MaxAttempts)
                    {
                        FailedRequests++;
                        break;
                    }

                    int waitMs = CalculateBackoffDelay(attempt);
                    RetriedRequests++;
                    Thread.Sleep(waitMs);
                }
            }

            // All retries exhausted
            DateTime requestEnd = DateTime.UtcNow;
            int totalLatencyMs = (int)(requestEnd - requestStart).TotalMilliseconds;

            return new HttpResponse
            {
                StatusCode = 0,
                Success = false,
                Body = string.Empty,
                Headers = new Dictionary<string, string>(),
                LatencyMs = totalLatencyMs,
                Attempts = attempt,
                ErrorMessage = $"Failed after {attempt} attempts. Last error: {lastException?.Message}"
            };
        }

        /// <summary>
        /// Calcula delay de backoff baseado na política configurada
        /// </summary>
        private int CalculateBackoffDelay(int attempt)
        {
            return _retryPolicy.BackoffMode.ToUpperInvariant() switch
            {
                "EXPONENTIAL" => Math.Min(
                    _retryPolicy.MaxDelayMs,
                    _retryPolicy.BaseDelayMs * (int)Math.Pow(2, attempt - 1)
                ),
                "LINEAR" => Math.Min(
                    _retryPolicy.MaxDelayMs,
                    _retryPolicy.BaseDelayMs * attempt
                ),
                "FIXED" => _retryPolicy.BaseDelayMs,
                _ => _retryPolicy.BaseDelayMs
            };
        }

        /// <summary>
        /// Retorna estatísticas de latência
        /// </summary>
        public LatencyStats GetLatencyStats()
        {
            if (_latencies.Count == 0)
            {
                return new LatencyStats
                {
                    Count = 0,
                    AvgMs = 0,
                    MinMs = 0,
                    MaxMs = 0,
                    P50Ms = 0,
                    P95Ms = 0,
                    P99Ms = 0
                };
            }

            var sorted = _latencies.OrderBy(x => x).ToList();

            return new LatencyStats
            {
                Count = sorted.Count,
                AvgMs = (int)sorted.Average(),
                MinMs = sorted.Min(),
                MaxMs = sorted.Max(),
                P50Ms = sorted[sorted.Count / 2],
                P95Ms = sorted[(int)(sorted.Count * 0.95)],
                P99Ms = sorted[(int)(sorted.Count * 0.99)]
            };
        }

        /// <summary>
        /// Reseta contadores de estatísticas
        /// </summary>
        public void ResetStats()
        {
            TotalRequests = 0;
            SuccessfulRequests = 0;
            FailedRequests = 0;
            RetriedRequests = 0;
            ThrottledRequests = 0;
            _latencies.Clear();
        }
    }

    /// <summary>
    /// Política de retry configurável
    /// </summary>
    public class RetryPolicy
    {
        public int MaxAttempts { get; set; } = 5;
        public string BackoffMode { get; set; } = "Exponential"; // Exponential, Linear, Fixed
        public int BaseDelayMs { get; set; } = 1000;
        public int MaxDelayMs { get; set; } = 300000; // 5 minutes

        public RetryPolicy() { }

        public RetryPolicy(int maxAttempts, string backoffMode, int baseDelayMs)
        {
            MaxAttempts = maxAttempts;
            BackoffMode = backoffMode;
            BaseDelayMs = baseDelayMs;
        }
    }

    /// <summary>
    /// Resposta HTTP encapsulada
    /// </summary>
    public class HttpResponse
    {
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public string Body { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public int LatencyMs { get; set; }
        public int Attempts { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Estatísticas de latência
    /// </summary>
    public class LatencyStats
    {
        public int Count { get; set; }
        public int AvgMs { get; set; }
        public int MinMs { get; set; }
        public int MaxMs { get; set; }
        public int P50Ms { get; set; }
        public int P95Ms { get; set; }
        public int P99Ms { get; set; }

        public override string ToString()
        {
            return $"Latency: avg={AvgMs}ms, min={MinMs}ms, max={MaxMs}ms, " +
                   $"p50={P50Ms}ms, p95={P95Ms}ms, p99={P99Ms}ms (n={Count})";
        }
    }
}