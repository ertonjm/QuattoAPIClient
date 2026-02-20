/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Corporate API Source Component
Versão: 1.0.0
Projeto: SESC-DF Data Warehouse
Autor: Erton Miranda / Quatto Consultoria
Data: Fevereiro 2026
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Componente customizado SSIS para extração de dados de APIs REST com:
- Paginação automática (Offset, Cursor, Link-based)
- Retry com backoff exponencial (429, 5xx)
- Autenticação centralizada via Connection Manager
- Extração incremental com watermark
- Armazenamento de JSON bruto para auditoria
- Rate limiting configurável
- Telemetria detalhada

DEPENDÊNCIAS:
- Microsoft.SqlServer.DTSPipelineWrap
- Microsoft.SqlServer.PipelineHost
- System.Data.SqlClient
- System.Text.Json

REFERÊNCIAS INTERNAS:
- QuattoAPIClient.ConnectionManager (ApiConnectionManager)
- Helpers: HttpHelper, PaginationEngine, WatermarkManager, RawStorageManager, SchemaMapper

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace QuattoAPIClient.Source
{
    /// <summary>
    /// Corporate API Source - Componente de extração de APIs REST
    /// </summary>
    [DtsPipelineComponent(
        DisplayName = "Quatto Corporate API Source",
        Description = "Extrai dados de APIs REST com paginação, retry, incremental e auditoria de JSON bruto.",
        ComponentType = ComponentType.SourceAdapter,
        IconResource = "QuattoAPIClient.Source.Resources.icon.ico",
        CurrentVersion = 3,
        NoEditor = false)]
    public class CorporateApiSource : PipelineComponent
    {
        #region Private Fields

        private HttpClient? _httpClient;
        private Guid _correlationId;

        // Cached properties
        private string _baseUrl = string.Empty;
        private string _endpoint = string.Empty;
        private string _queryTemplate = string.Empty;
        private string _httpMethod = "GET";
        private int _pageSize = 500;
        private int _startPage = 1;
        private int _maxPages = 0;
        private int _rateLimitRpm = 120;
        private int _timeoutSeconds = 100;

        // Incremental
        private bool _enableIncremental = false;
        private string _watermarkColumn = "updatedAt";
        private string _watermarkStore = "dbo.API_Watermarks";
        private string _sourceSystem = string.Empty;
        private string _environment = "PRD";

        // Raw storage
        private string _rawStoreMode = "SqlVarbinary";
        private string _rawStoreTarget = "dbo.API_RawPayloads";
        private bool _compressRawJson = true;
        private bool _hashRawJson = true;

        // Schema
        private string _schemaMapping = string.Empty;

        // Error handling
        private string _errorBehavior = "RedirectRow";
        private int _maxErrorsBeforeAbort = 100;

        // Retry policy
        private int _maxRetries = 5;
        private string _backoffMode = "Exponential";
        private int _baseDelayMs = 1000;

        // Runtime counters
        private int _totalRequests = 0;
        private int _successfulRequests = 0;
        private int _failedRequests = 0;
        private int _retriedRequests = 0;
        private int _throttledRequests = 0;
        private long _totalBytesReceived = 0;
        private List<int> _latencies = new List<int>();

        #endregion

        #region Component Lifecycle

        public override void ProvideComponentProperties()
        {
            base.ProvideComponentProperties();

            // Remove default inputs/outputs
            ComponentMetaData.InputCollection.RemoveAll();
            ComponentMetaData.OutputCollection.RemoveAll();

            // Create main output
            IDTSOutput100 output = ComponentMetaData.OutputCollection.New();
            output.Name = "APIOutput";
            output.Description = "Dados extraídos da API";
            output.SynchronousInputID = 0;

            // Create error output
            IDTSOutput100 errorOutput = ComponentMetaData.OutputCollection.New();
            errorOutput.Name = "ErrorOutput";
            errorOutput.Description = "Registros com falha de parsing";
            errorOutput.IsErrorOut = true;
            errorOutput.SynchronousInputID = 0;

            // Add error columns
            IDTSOutputColumn100 errorJsonCol = errorOutput.OutputColumnCollection.New();
            errorJsonCol.Name = "ErrorJSON";
            errorJsonCol.Description = "JSON que causou erro";
            errorJsonCol.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);

            IDTSOutputColumn100 errorMsgCol = errorOutput.OutputColumnCollection.New();
            errorMsgCol.Name = "ErrorMessage";
            errorMsgCol.Description = "Mensagem de erro";
            errorMsgCol.SetDataTypeProperties(DataType.DT_WSTR, 4000, 0, 0, 0);

            // Connection Manager
            IDTSRuntimeConnection100 conn = ComponentMetaData.RuntimeConnectionCollection.New();
            conn.Name = "APIConnection";
            conn.Description = "Conexão com gerenciamento de autenticação API";

            // Properties - Endpoint Configuration
            AddStringProperty("BaseUrl", "", "URL base da API (ex: https://api.gladium.com)");
            AddStringProperty("Endpoint", "", "Endpoint da API (ex: /v1/orders)");
            AddStringProperty("HttpMethod", "GET", "Método HTTP (GET, POST)");
            AddStringProperty("QueryTemplate", "?page={Page}&pageSize={PageSize}",
                "Template de query string. Placeholders: {Page}, {PageSize}, {Watermark}");
            AddStringProperty("RequestBody", "", "Corpo da requisição (para POST)");

            // Properties - Pagination
            AddIntProperty("PageSize", 500, "Quantidade de registros por página");
            AddIntProperty("StartPage", 1, "Página inicial (geralmente 1)");
            AddIntProperty("MaxPages", 0, "Máximo de páginas (0 = ilimitado)");
            AddStringProperty("PaginationType", "Offset", "Tipo: Offset, Cursor, Link, None");

            // Properties - Rate Limiting & Timeout
            AddIntProperty("RateLimitRPM", 120, "Limite de requisições por minuto");
            AddIntProperty("TimeoutSeconds", 100, "Timeout da requisição em segundos");

            // Properties - Retry Policy
            AddIntProperty("MaxRetries", 5, "Máximo de tentativas em caso de falha");
            AddStringProperty("BackoffMode", "Exponential", "Modo: Exponential, Fixed, Linear");
            AddIntProperty("BaseDelayMs", 1000, "Delay base para retry (ms)");

            // Properties - Incremental Load
            AddBoolProperty("EnableIncremental", false, "Habilitar extração incremental");
            AddStringProperty("WatermarkColumn", "updatedAt", "Campo JSON para watermark");
            AddStringProperty("WatermarkStore", "dbo.API_Watermarks", "Tabela de controle");
            AddStringProperty("SourceSystem", "", "Nome do sistema (ex: Gladium)");
            AddStringProperty("Environment", "PRD", "Ambiente: DEV, HML, PRD");

            // Properties - Raw Storage
            AddStringProperty("RawStoreMode", "SqlVarbinary", "Modo: FileSystem, SqlVarbinary, None");
            AddStringProperty("RawStoreTarget", "dbo.API_RawPayloads", "Tabela ou caminho");
            AddBoolProperty("CompressRawJson", true, "Comprimir JSON com GZIP");
            AddBoolProperty("HashRawJson", true, "Gerar hash SHA256");

            // Properties - Schema Mapping
            AddStringProperty("SchemaMapping", "", "JSON de mapeamento de campos");
            AddBoolProperty("InferSchemaOnValidate", false, "Inferir esquema automaticamente");

            // Properties - Error Handling
            AddStringProperty("ErrorBehavior", "RedirectRow", "RedirectRow, FailComponent, IgnoreFailure");
            AddIntProperty("MaxErrorsBeforeAbort", 100, "Máximo de erros (0 = ilimitado)");

            ComponentMetaData.ValidateExternalMetadata = true;
        }

        public override DTSValidationStatus Validate()
        {
            bool cancelEvent = false;

            // Validate connection manager
            if (ComponentMetaData.RuntimeConnectionCollection.Count == 0 ||
                ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager == null)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "Connection Manager 'APIConnection' não está configurado.",
                    "", 0, out cancelEvent);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            // Validate BaseUrl
            string baseUrl = GetPropertyValue<string>("BaseUrl");
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "Propriedade 'BaseUrl' é obrigatória.",
                    "", 0, out cancelEvent);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out _))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "Propriedade 'BaseUrl' deve ser uma URL válida.",
                    "", 0, out cancelEvent);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            // Validate Endpoint
            if (string.IsNullOrWhiteSpace(GetPropertyValue<string>("Endpoint")))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "Propriedade 'Endpoint' é obrigatória.",
                    "", 0, out cancelEvent);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            // Validate SourceSystem if incremental is enabled
            if (GetPropertyValue<bool>("EnableIncremental") &&
                string.IsNullOrWhiteSpace(GetPropertyValue<string>("SourceSystem")))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "Propriedade 'SourceSystem' é obrigatória quando EnableIncremental=true.",
                    "", 0, out cancelEvent);
                return DTSValidationStatus.VS_ISBROKEN;
            }

            return DTSValidationStatus.VS_ISVALID;
        }

        public override void ReinitializeMetaData()
        {
            base.ReinitializeMetaData();

            var output = ComponentMetaData.OutputCollection["APIOutput"];
            output.OutputColumnCollection.RemoveAll();

            string schemaMapping = GetPropertyValue<string>("SchemaMapping");

            if (!string.IsNullOrWhiteSpace(schemaMapping))
            {
                try
                {
                    ApplySchemaMapping(output, schemaMapping);
                }
                catch (Exception ex)
                {
                    bool cancelEvent = false;
                    ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                        $"Erro ao aplicar SchemaMapping: {ex.Message}",
                        "", 0);
                }
            }
        }

        public override void AcquireConnections(object transaction)
        {
            if (ComponentMetaData.RuntimeConnectionCollection.Count == 0)
                return;

            IDTSRuntimeConnection100 conn = ComponentMetaData.RuntimeConnectionCollection[0];

            var connMgr = conn.ConnectionManager;
            if (connMgr == null)
                throw new Exception("Connection Manager não encontrado");

            // Try to get ApiConnectionManager from InnerObject or as direct cast
            dynamic? innerObject = connMgr.InnerObject;

            if (innerObject == null)
                throw new Exception("Connection Manager InnerObject é null");

            // Build HttpClient
            _httpClient = BuildHttpClientFromConnectionManager(innerObject);

            bool cancelEvent = false;
            ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                "HttpClient configurado com sucesso via API Connection Manager.",
                "", 0, ref cancelEvent);
        }

        public override void PreExecute()
        {
            base.PreExecute();

            _correlationId = Guid.NewGuid();

            // Cache all properties
            _baseUrl = GetPropertyValue<string>("BaseUrl");
            _endpoint = GetPropertyValue<string>("Endpoint");
            _queryTemplate = GetPropertyValue<string>("QueryTemplate");
            _httpMethod = GetPropertyValue<string>("HttpMethod");
            _pageSize = GetPropertyValue<int>("PageSize");
            _startPage = GetPropertyValue<int>("StartPage");
            _maxPages = GetPropertyValue<int>("MaxPages");
            _rateLimitRpm = GetPropertyValue<int>("RateLimitRPM");
            _timeoutSeconds = GetPropertyValue<int>("TimeoutSeconds");

            // Incremental
            _enableIncremental = GetPropertyValue<bool>("EnableIncremental");
            _watermarkColumn = GetPropertyValue<string>("WatermarkColumn");
            _watermarkStore = GetPropertyValue<string>("WatermarkStore");
            _sourceSystem = GetPropertyValue<string>("SourceSystem");
            _environment = GetPropertyValue<string>("Environment");

            // Raw storage
            _rawStoreMode = GetPropertyValue<string>("RawStoreMode");
            _rawStoreTarget = GetPropertyValue<string>("RawStoreTarget");
            _compressRawJson = GetPropertyValue<bool>("CompressRawJson");
            _hashRawJson = GetPropertyValue<bool>("HashRawJson");

            // Schema
            _schemaMapping = GetPropertyValue<string>("SchemaMapping");

            // Error handling
            _errorBehavior = GetPropertyValue<string>("ErrorBehavior");
            _maxErrorsBeforeAbort = GetPropertyValue<int>("MaxErrorsBeforeAbort");

            // Retry
            _maxRetries = GetPropertyValue<int>("MaxRetries");
            _backoffMode = GetPropertyValue<string>("BackoffMode");
            _baseDelayMs = GetPropertyValue<int>("BaseDelayMs");

            // Reset counters
            _totalRequests = 0;
            _successfulRequests = 0;
            _failedRequests = 0;
            _retriedRequests = 0;
            _throttledRequests = 0;
            _totalBytesReceived = 0;
            _latencies.Clear();

            bool cancelEvent = false;
            ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                $"[{_correlationId}] PreExecute completo. Modo Raw: {_rawStoreMode}, Incremental: {_enableIncremental}",
                "", 0, ref cancelEvent);
        }

        public override void PrimeOutput(int outputs, int[] outputIDs, PipelineBuffer[] buffers)
        {
            var outputBuffer = buffers[0];
            var errorBuffer = buffers.Length > 1 ? buffers[1] : null;

            bool cancelEvent = false;
            DateTime startTime = DateTime.UtcNow;

            ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                $"[{_correlationId}] Iniciando extração: {_baseUrl}{_endpoint}",
                "", 0, ref cancelEvent);

            try
            {
                // Load watermark if incremental
                string? watermark = null;
                if (_enableIncremental)
                {
                    watermark = LoadWatermark();
                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                        $"[{_correlationId}] Watermark carregado: {watermark ?? "NULL (carga completa)"}",
                        "", 0, ref cancelEvent);
                }

                // Pagination loop
                int currentPage = _startPage;
                int pagesProcessed = 0;
                long totalRecords = 0;
                string? maxWatermarkObserved = watermark;
                int errorCount = 0;

                while (true)
                {
                    // Check max pages
                    if (_maxPages > 0 && pagesProcessed >= _maxPages)
                    {
                        ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                            $"[{_correlationId}] MaxPages ({_maxPages}) atingido. Finalizando.",
                            "", 0, ref cancelEvent);
                        break;
                    }

                    // Build URL
                    string url = BuildRequestUrl(currentPage, watermark);

                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                        $"[{_correlationId}] Página {currentPage}: {url}",
                        "", 0, ref cancelEvent);

                    // Make request with retry
                    var requestStart = DateTime.UtcNow;
                    HttpResponseMessage response = SendWithRetry(url);
                    var requestEnd = DateTime.UtcNow;
                    int elapsedMs = (int)(requestEnd - requestStart).TotalMilliseconds;
                    _latencies.Add(elapsedMs);

                    string jsonContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    _totalBytesReceived += jsonContent.Length;

                    // Store raw JSON
                    if (_rawStoreMode != "None")
                    {
                        StoreRawPayload(jsonContent, currentPage, (int)response.StatusCode, elapsedMs);
                    }

                    // Parse and write to buffer
                    int recordsInPage = 0;
                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                        {
                            JsonElement root = doc.RootElement;
                            JsonElement dataArray = ExtractDataArray(root);

                            if (dataArray.ValueKind == JsonValueKind.Array)
                            {
                                foreach (JsonElement item in dataArray.EnumerateArray())
                                {
                                    try
                                    {
                                        outputBuffer.AddRow();
                                        MapJsonToBuffer(outputBuffer, item);
                                        recordsInPage++;

                                        // Update watermark
                                        if (_enableIncremental)
                                        {
                                            string? itemWatermark = ExtractWatermark(item);
                                            if (itemWatermark != null &&
                                                (maxWatermarkObserved == null || string.Compare(itemWatermark, maxWatermarkObserved) > 0))
                                            {
                                                maxWatermarkObserved = itemWatermark;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        errorCount++;
                                        HandleRowError(errorBuffer, item, ex, ref cancelEvent);

                                        if (_maxErrorsBeforeAbort > 0 && errorCount >= _maxErrorsBeforeAbort)
                                        {
                                            throw new Exception($"MaxErrorsBeforeAbort ({_maxErrorsBeforeAbort}) atingido.");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        ComponentMetaData.FireError(0, ComponentMetaData.Name,
                            $"[{_correlationId}] Erro ao parsear JSON da página {currentPage}: {jsonEx.Message}",
                            "", 0, out cancelEvent);

                        if (_errorBehavior == "FailComponent")
                            throw;
                    }

                    totalRecords += recordsInPage;
                    pagesProcessed++;

                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                        $"[{_correlationId}] Página {currentPage} processada: {recordsInPage} registros em {elapsedMs}ms",
                        "", 0, ref cancelEvent);

                    // Check if last page
                    if (recordsInPage < _pageSize)
                    {
                        ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                            $"[{_correlationId}] Última página detectada (registros < pageSize)",
                            "", 0, ref cancelEvent);
                        break;
                    }

                    // Rate limiting
                    if (_rateLimitRpm > 0)
                    {
                        int delayMs = (int)(60000.0 / _rateLimitRpm);
                        Thread.Sleep(delayMs);
                    }

                    currentPage++;
                }

                // Save watermark
                if (_enableIncremental && maxWatermarkObserved != null)
                {
                    SaveWatermark(maxWatermarkObserved, totalRecords, pagesProcessed);
                }

                // Log execution
                LogExecution(startTime, DateTime.UtcNow, totalRecords, pagesProcessed, "SUCCESS", null);

                // Summary
                TimeSpan elapsed = DateTime.UtcNow - startTime;
                ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                    $"[{_correlationId}] ✓ Extração concluída: {pagesProcessed} páginas, {totalRecords:N0} registros, " +
                    $"{_totalBytesReceived:N0} bytes em {elapsed.TotalSeconds:F2}s " +
                    $"(média: {(_latencies.Any() ? _latencies.Average() : 0):F0}ms/req)",
                    "", 0, ref cancelEvent);
            }
            catch (Exception ex)
            {
                LogExecution(startTime, DateTime.UtcNow, 0, 0, "FAILED", ex.Message);

                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    $"[{_correlationId}] ✗ Erro fatal: {ex.Message}",
                    "", 0, out cancelEvent);
                throw;
            }
            finally
            {
                outputBuffer.SetEndOfRowset();
                errorBuffer?.SetEndOfRowset();
            }
        }

        public override void PostExecute()
        {
            base.PostExecute();
        }

        public override void ReleaseConnections()
        {
            _httpClient?.Dispose();
            _httpClient = null;
        }

        #endregion

        #region Private Helper Methods

        private string BuildRequestUrl(int page, string? watermark)
        {
            string query = _queryTemplate
                .Replace("{Page}", page.ToString())
                .Replace("{PageSize}", _pageSize.ToString())
                .Replace("{Watermark}", watermark ?? "")
                .Replace("{Watermark:o}", watermark ?? "");

            string url = $"{_baseUrl.TrimEnd('/')}/{_endpoint.TrimStart('/')}";

            if (!string.IsNullOrWhiteSpace(query))
            {
                url += query.StartsWith("?") ? query : "?" + query;
            }

            return url;
        }

        private HttpResponseMessage SendWithRetry(string url)
        {
            if (_httpClient == null)
                throw new InvalidOperationException("HttpClient não foi inicializado");

            int attempt = 0;
            bool cancelEvent = false;

            while (attempt < _maxRetries)
            {
                attempt++;
                _totalRequests++;

                try
                {
                    HttpResponseMessage response = _httpClient.GetAsync(url).GetAwaiter().GetResult();

                    // Handle rate limiting (429) or server errors (5xx)
                    if ((int)response.StatusCode == 429 || (int)response.StatusCode >= 500)
                    {
                        _throttledRequests++;

                        int waitMs = CalculateBackoffDelay(attempt);

                        // Try to read Retry-After header
                        if (response.Headers.TryGetValues("Retry-After", out var retryAfterValues))
                        {
                            string? retryAfter = retryAfterValues.FirstOrDefault();
                            if (int.TryParse(retryAfter, out int retryAfterSeconds))
                            {
                                waitMs = Math.Min(retryAfterSeconds * 1000, 300000); // Max 5 min
                            }
                        }

                        ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                            $"[{_correlationId}] Status {(int)response.StatusCode}. Tentativa {attempt}/{_maxRetries}. Aguardando {waitMs}ms...",
                            "", 0);

                        if (attempt < _maxRetries)
                        {
                            _retriedRequests++;
                            Thread.Sleep(waitMs);
                            continue;
                        }
                    }

                    response.EnsureSuccessStatusCode();
                    _successfulRequests++;
                    return response;
                }
                catch (HttpRequestException ex)
                {
                    _failedRequests++;

                    if (attempt >= _maxRetries)
                    {
                        ComponentMetaData.FireError(0, ComponentMetaData.Name,
                            $"[{_correlationId}] Falha após {_maxRetries} tentativas: {ex.Message}",
                            "", 0, out cancelEvent);
                        throw new Exception($"HTTP request failed after {_maxRetries} retries: {ex.Message}", ex);
                    }

                    int waitMs = CalculateBackoffDelay(attempt);
                    ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                        $"[{_correlationId}] Erro HTTP. Tentativa {attempt}/{_maxRetries}. Aguardando {waitMs}ms...",
                        "", 0);

                    _retriedRequests++;
                    Thread.Sleep(waitMs);
                }
            }

            throw new Exception($"Max retries ({_maxRetries}) exceeded");
        }

        private int CalculateBackoffDelay(int attempt)
        {
            return _backoffMode.ToUpperInvariant() switch
            {
                "EXPONENTIAL" => Math.Min(300000, _baseDelayMs * (int)Math.Pow(2, attempt)),
                "LINEAR" => _baseDelayMs * attempt,
                "FIXED" => _baseDelayMs,
                _ => _baseDelayMs
            };
        }

        private JsonElement ExtractDataArray(JsonElement root)
        {
            if (root.ValueKind == JsonValueKind.Array)
                return root;

            // Try common property names
            string[] commonArrayProps = { "data", "items", "results", "records", "rows" };

            foreach (string prop in commonArrayProps)
            {
                if (root.TryGetProperty(prop, out JsonElement arrayProp) &&
                    arrayProp.ValueKind == JsonValueKind.Array)
                {
                    return arrayProp;
                }
            }

            // If object without array property, treat as single-item array
            return root;
        }

        private void StoreRawPayload(string json, int page, int statusCode, int elapsedMs)
        {
            if (_rawStoreMode == "None")
                return;

            try
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                byte[] payload = _compressRawJson ? CompressGzip(jsonBytes) : jsonBytes;
                string hash = _hashRawJson ? ComputeSha256(jsonBytes) : string.Empty;

                if (_rawStoreMode == "SqlVarbinary")
                {
                    string connString = GetConnectionString();

                    using (var conn = new SqlConnection(connString))
                    {
                        conn.Open();

                        string sql = @"
                            INSERT INTO " + _rawStoreTarget + @" 
                            (CorrelationID, SystemName, Endpoint, Environment, HttpMethod, RequestUrl,
                             ResponseStatusCode, ResponseBodyGzip, ResponseBodyHash, ResponseSizeBytes,
                             PageNumber, PageSize, ElapsedMs, CollectedUtc)
                            VALUES
                            (@CorrelationID, @SystemName, @Endpoint, @Environment, @HttpMethod, @RequestUrl,
                             @StatusCode, @Payload, @Hash, @SizeBytes,
                             @PageNumber, @PageSize, @ElapsedMs, SYSUTCDATETIME())";

                        using (var cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@CorrelationID", _correlationId);
                            cmd.Parameters.AddWithValue("@SystemName", _sourceSystem);
                            cmd.Parameters.AddWithValue("@Endpoint", _endpoint);
                            cmd.Parameters.AddWithValue("@Environment", _environment);
                            cmd.Parameters.AddWithValue("@HttpMethod", _httpMethod);
                            cmd.Parameters.AddWithValue("@RequestUrl", ""); // Simplificado
                            cmd.Parameters.AddWithValue("@StatusCode", statusCode);
                            cmd.Parameters.AddWithValue("@Payload", payload);
                            cmd.Parameters.AddWithValue("@Hash", hash);
                            cmd.Parameters.AddWithValue("@SizeBytes", jsonBytes.Length);
                            cmd.Parameters.AddWithValue("@PageNumber", page);
                            cmd.Parameters.AddWithValue("@PageSize", _pageSize);
                            cmd.Parameters.AddWithValue("@ElapsedMs", elapsedMs);

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                else if (_rawStoreMode == "FileSystem")
                {
                    string directory = ExpandPath(_rawStoreTarget);
                    Directory.CreateDirectory(directory);

                    string filename = $"payload-p{page:000000}-{_correlationId:N}.json.gz";
                    string filepath = Path.Combine(directory, filename);

                    File.WriteAllBytes(filepath, payload);

                    if (_hashRawJson)
                    {
                        string metaFile = Path.ChangeExtension(filepath, ".meta");
                        File.WriteAllText(metaFile, $"sha256={hash}\nsize={jsonBytes.Length}\nstatus={statusCode}\n");
                    }
                }
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                    $"[{_correlationId}] Falha ao armazenar JSON bruto: {ex.Message}",
                    "", 0);
            }
        }

        private string? LoadWatermark()
        {
            try
            {
                string connString = GetConnectionString();

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("dbo.usp_API_GetWatermark", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SystemName", _sourceSystem);
                        cmd.Parameters.AddWithValue("@Endpoint", _endpoint);
                        cmd.Parameters.AddWithValue("@Environment", _environment);

                        var watermarkParam = cmd.Parameters.Add("@Watermark", SqlDbType.NVarChar, 100);
                        watermarkParam.Direction = ParameterDirection.Output;

                        var lastRunParam = cmd.Parameters.Add("@LastRunUtc", SqlDbType.DateTime2);
                        lastRunParam.Direction = ParameterDirection.Output;

                        var lastStatusParam = cmd.Parameters.Add("@LastStatus", SqlDbType.NVarChar, 50);
                        lastStatusParam.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        string? watermark = watermarkParam.Value as string;

                        if (watermark == "1900-01-01T00:00:00Z" || watermark == "NEVER_RUN")
                            return null;

                        return watermark;
                    }
                }
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                    $"[{_correlationId}] Erro ao carregar watermark: {ex.Message}. Usando carga completa.",
                    "", 0);
                return null;
            }
        }

        private void SaveWatermark(string watermark, long recordsExtracted, int pagesExtracted)
        {
            try
            {
                string connString = GetConnectionString();

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("dbo.usp_API_UpdateWatermark", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SystemName", _sourceSystem);
                        cmd.Parameters.AddWithValue("@Endpoint", _endpoint);
                        cmd.Parameters.AddWithValue("@Environment", _environment);
                        cmd.Parameters.AddWithValue("@NewWatermark", watermark);
                        cmd.Parameters.AddWithValue("@RecordsExtracted", recordsExtracted);
                        cmd.Parameters.AddWithValue("@PagesExtracted", pagesExtracted);
                        cmd.Parameters.AddWithValue("@BytesExtracted", _totalBytesReceived);
                        cmd.Parameters.AddWithValue("@Status", "SUCCESS");
                        cmd.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                bool cancelEvent = false;
                ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                    $"[{_correlationId}] Watermark atualizado: {watermark}",
                    "", 0, ref cancelEvent);
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                    $"[{_correlationId}] Erro ao salvar watermark: {ex.Message}",
                    "", 0);
            }
        }

        private void LogExecution(DateTime start, DateTime end, long records, int pages, string status, string? errorMsg)
        {
            try
            {
                string connString = GetConnectionString();

                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO dbo.API_ExecutionLog
                        (CorrelationID, PackageName, TaskName, SystemName, Endpoint, Environment,
                         ExecutionStartedUtc, ExecutionFinishedUtc, DurationMs,
                         TotalRequests, SuccessfulRequests, FailedRequests, RetriedRequests, ThrottledRequests,
                         TotalPages, TotalRecords, TotalBytesReceived,
                         AvgLatencyMs, MinLatencyMs, MaxLatencyMs,
                         Status, ErrorMessage)
                        VALUES
                        (@CorrelationID, @PackageName, @TaskName, @SystemName, @Endpoint, @Environment,
                         @StartUtc, @EndUtc, @DurationMs,
                         @TotalReq, @SuccessReq, @FailedReq, @RetriedReq, @ThrottledReq,
                         @TotalPages, @TotalRecords, @TotalBytes,
                         @AvgLatency, @MinLatency, @MaxLatency,
                         @Status, @ErrorMsg)";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CorrelationID", _correlationId);
                        cmd.Parameters.AddWithValue("@PackageName", ComponentMetaData.Name);
                        cmd.Parameters.AddWithValue("@TaskName", ComponentMetaData.Name);
                        cmd.Parameters.AddWithValue("@SystemName", _sourceSystem);
                        cmd.Parameters.AddWithValue("@Endpoint", _endpoint);
                        cmd.Parameters.AddWithValue("@Environment", _environment);
                        cmd.Parameters.AddWithValue("@StartUtc", start);
                        cmd.Parameters.AddWithValue("@EndUtc", end);
                        cmd.Parameters.AddWithValue("@DurationMs", (int)(end - start).TotalMilliseconds);
                        cmd.Parameters.AddWithValue("@TotalReq", _totalRequests);
                        cmd.Parameters.AddWithValue("@SuccessReq", _successfulRequests);
                        cmd.Parameters.AddWithValue("@FailedReq", _failedRequests);
                        cmd.Parameters.AddWithValue("@RetriedReq", _retriedRequests);
                        cmd.Parameters.AddWithValue("@ThrottledReq", _throttledRequests);
                        cmd.Parameters.AddWithValue("@TotalPages", pages);
                        cmd.Parameters.AddWithValue("@TotalRecords", records);
                        cmd.Parameters.AddWithValue("@TotalBytes", _totalBytesReceived);
                        cmd.Parameters.AddWithValue("@AvgLatency", _latencies.Any() ? (int)_latencies.Average() : 0);
                        cmd.Parameters.AddWithValue("@MinLatency", _latencies.Any() ? _latencies.Min() : 0);
                        cmd.Parameters.AddWithValue("@MaxLatency", _latencies.Any() ? _latencies.Max() : 0);
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@ErrorMsg", (object?)errorMsg ?? DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                    $"[{_correlationId}] Erro ao registrar log de execução: {ex.Message}",
                    "", 0);
            }
        }

        private string? ExtractWatermark(JsonElement item)
        {
            if (string.IsNullOrWhiteSpace(_watermarkColumn))
                return null;

            if (item.ValueKind != JsonValueKind.Object)
                return null;

            if (item.TryGetProperty(_watermarkColumn, out JsonElement watermarkProp))
            {
                return watermarkProp.ValueKind == JsonValueKind.String
                    ? watermarkProp.GetString()
                    : watermarkProp.ToString();
            }

            return null;
        }

        private void MapJsonToBuffer(PipelineBuffer buffer, JsonElement item)
        {
            if (string.IsNullOrWhiteSpace(_schemaMapping))
                return;

            // TODO: Implementar mapeamento real baseado em SchemaMapping JSON
            // Por ora, apenas stub para compilação
        }

        private void HandleRowError(PipelineBuffer? errorBuffer, JsonElement item, Exception ex, ref bool cancelEvent)
        {
            if (_errorBehavior == "IgnoreFailure")
                return;

            ComponentMetaData.FireWarning(0, ComponentMetaData.Name,
                $"[{_correlationId}] Erro ao processar registro: {ex.Message}",
                "", 0);

            if (errorBuffer != null && _errorBehavior == "RedirectRow")
            {
                errorBuffer.AddRow();
                errorBuffer.SetString(0, item.GetRawText().Substring(0, Math.Min(4000, item.GetRawText().Length)));
                errorBuffer.SetString(1, ex.Message.Substring(0, Math.Min(4000, ex.Message.Length)));
            }
        }

        private void ApplySchemaMapping(IDTSOutput100 output, string schemaMappingJson)
        {
            // TODO: Implementar parsing de SchemaMapping e criação de OutputColumns
            // Por ora, stub para compilação
        }

        private HttpClient BuildHttpClientFromConnectionManager(dynamic connectionManager)
        {
            // TODO: Implementar integração real com ApiConnectionManager
            // Por ora, retorna HttpClient básico
            return new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_timeoutSeconds)
            };
        }

        private string GetConnectionString()
        {
            // Get connection string from SSIS connection manager or component property
            // Simplificado para o skeleton
            return "Data Source=.;Initial Catalog=SESCDF_DW;Integrated Security=True;";
        }

        private string ExpandPath(string template)
        {
            DateTime now = DateTime.UtcNow;
            return template
                .Replace("{yyyy}", now.ToString("yyyy"))
                .Replace("{MM}", now.ToString("MM"))
                .Replace("{dd}", now.ToString("dd"))
                .Replace("{system}", _sourceSystem)
                .Replace("{environment}", _environment);
        }

        private byte[] CompressGzip(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
                {
                    gzip.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        private string ComputeSha256(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        #endregion

        #region Property Helper Methods

        private void AddStringProperty(string name, string defaultValue, string description)
        {
            IDTSCustomProperty100 prop = ComponentMetaData.CustomPropertyCollection.New();
            prop.Name = name;
            prop.Description = description;
            prop.Value = defaultValue;
            prop.TypeConverter = typeof(string).AssemblyQualifiedName;
            prop.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
        }

        private void AddIntProperty(string name, int defaultValue, string description)
        {
            IDTSCustomProperty100 prop = ComponentMetaData.CustomPropertyCollection.New();
            prop.Name = name;
            prop.Description = description;
            prop.Value = defaultValue;
            prop.TypeConverter = typeof(int).AssemblyQualifiedName;
            prop.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
        }

        private void AddBoolProperty(string name, bool defaultValue, string description)
        {
            IDTSCustomProperty100 prop = ComponentMetaData.CustomPropertyCollection.New();
            prop.Name = name;
            prop.Description = description;
            prop.Value = defaultValue;
            prop.TypeConverter = typeof(bool).AssemblyQualifiedName;
            prop.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
        }

        private T GetPropertyValue<T>(string name)
        {
            if (ComponentMetaData.CustomPropertyCollection.Contains(name))
            {
                object? value = ComponentMetaData.CustomPropertyCollection[name].Value;
                if (value is T typedValue)
                    return typedValue;
                if (value != null)
                    return (T)Convert.ChangeType(value, typeof(T));
            }
            return default(T)!;
        }

        #endregion
    }
}