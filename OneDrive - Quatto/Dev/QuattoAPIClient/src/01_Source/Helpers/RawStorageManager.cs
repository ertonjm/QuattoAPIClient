/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Raw Storage Manager
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Gerenciador de armazenamento de JSON bruto para auditoria e reprocesso.
Suporta múltiplos modos:
- SqlVarbinary (tabela SQL com GZIP)
- FileSystem (arquivos .json.gz)
- None (sem armazenamento)

FUNCIONALIDADES:
- Compressão GZIP automática
- Hash SHA256 para detecção de duplicatas
- Metadados enriquecidos (status, latência, correlation ID)
- Suporte a retry/reprocesso

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace QuattoAPIClient.Source.Helpers
{
    /// <summary>
    /// Gerenciador de armazenamento de JSON bruto
    /// </summary>
    public class RawStorageManager
    {
        private readonly RawStorageMode _mode;
        private readonly string _target;
        private readonly bool _compress;
        private readonly bool _hash;
        private readonly string _connectionString;
        private readonly IDTSComponentMetaData100 _metadata;

        public RawStorageManager(
            RawStorageMode mode,
            string target,
            bool compress,
            bool hash,
            string connectionString,
            IDTSComponentMetaData100 metadata)
        {
            _mode = mode;
            _target = target ?? throw new ArgumentNullException(nameof(target));
            _compress = compress;
            _hash = hash;
            _connectionString = connectionString ?? string.Empty;
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        /// <summary>
        /// Armazena JSON bruto com metadados
        /// </summary>
        public long Store(RawPayload payload)
        {
            if (_mode == RawStorageMode.None)
                return 0;

            try
            {
                byte[] jsonBytes = Encoding.UTF8.GetBytes(payload.JsonContent);
                byte[] storedBytes = _compress ? CompressGzip(jsonBytes) : jsonBytes;
                string hash = _hash ? ComputeSha256(jsonBytes) : string.Empty;

                switch (_mode)
                {
                    case RawStorageMode.SqlVarbinary:
                        return StoreSqlVarbinary(payload, storedBytes, hash, jsonBytes.Length);

                    case RawStorageMode.FileSystem:
                        return StoreFileSystem(payload, storedBytes, hash, jsonBytes.Length);

                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                _metadata.FireWarning(0, "RawStorageManager",
                    $"Erro ao armazenar JSON bruto: {ex.Message}",
                    "", 0);
                return 0;
            }
        }

        /// <summary>
        /// Armazena em tabela SQL como VARBINARY
        /// </summary>
        private long StoreSqlVarbinary(RawPayload payload, byte[] storedBytes, string hash, int originalSize)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string sql = $@"
                    INSERT INTO {_target}
                    (CorrelationID, SystemName, Endpoint, Environment, HttpMethod, RequestUrl,
                     ResponseStatusCode, ResponseBodyGzip, ResponseBodyHash, ResponseSizeBytes,
                     PageNumber, PageSize, RecordCount, ElapsedMs, CollectedUtc)
                    VALUES
                    (@CorrelationID, @SystemName, @Endpoint, @Environment, @HttpMethod, @RequestUrl,
                     @StatusCode, @Payload, @Hash, @SizeBytes,
                     @PageNumber, @PageSize, @RecordCount, @ElapsedMs, SYSUTCDATETIME());
                    SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CorrelationID", payload.CorrelationId);
                    cmd.Parameters.AddWithValue("@SystemName", payload.SystemName);
                    cmd.Parameters.AddWithValue("@Endpoint", payload.Endpoint);
                    cmd.Parameters.AddWithValue("@Environment", payload.Environment);
                    cmd.Parameters.AddWithValue("@HttpMethod", payload.HttpMethod);
                    cmd.Parameters.AddWithValue("@RequestUrl", payload.RequestUrl);
                    cmd.Parameters.AddWithValue("@StatusCode", payload.StatusCode);
                    cmd.Parameters.AddWithValue("@Payload", storedBytes);
                    cmd.Parameters.AddWithValue("@Hash", hash);
                    cmd.Parameters.AddWithValue("@SizeBytes", originalSize);
                    cmd.Parameters.AddWithValue("@PageNumber", payload.PageNumber);
                    cmd.Parameters.AddWithValue("@PageSize", payload.PageSize);
                    cmd.Parameters.AddWithValue("@RecordCount", payload.RecordCount);
                    cmd.Parameters.AddWithValue("@ElapsedMs", payload.ElapsedMs);

                    object? result = cmd.ExecuteScalar();
                    long payloadId = result != null ? Convert.ToInt64(result) : 0;

                    bool cancelEvent = false;
                    _metadata.FireInformation(0, "RawStorageManager",
                        $"JSON armazenado: PayloadID={payloadId}, Tamanho={originalSize:N0} bytes " +
                        $"({(storedBytes.Length / (double)originalSize * 100):F1}% após compressão)",
                        "", 0, ref cancelEvent);

                    return payloadId;
                }
            }
        }

        /// <summary>
        /// Armazena como arquivo no filesystem
        /// </summary>
        private long StoreFileSystem(RawPayload payload, byte[] storedBytes, string hash, int originalSize)
        {
            string directory = ExpandPath(_target, payload);
            Directory.CreateDirectory(directory);

            string filename = $"payload-p{payload.PageNumber:000000}-{payload.CorrelationId:N}.json.gz";
            string filepath = Path.Combine(directory, filename);

            File.WriteAllBytes(filepath, storedBytes);

            if (_hash)
            {
                string metaFile = Path.ChangeExtension(filepath, ".meta");
                StringBuilder metaContent = new StringBuilder();
                metaContent.AppendLine($"correlation_id={payload.CorrelationId}");
                metaContent.AppendLine($"system={payload.SystemName}");
                metaContent.AppendLine($"endpoint={payload.Endpoint}");
                metaContent.AppendLine($"environment={payload.Environment}");
                metaContent.AppendLine($"page={payload.PageNumber}");
                metaContent.AppendLine($"status_code={payload.StatusCode}");
                metaContent.AppendLine($"elapsed_ms={payload.ElapsedMs}");
                metaContent.AppendLine($"size_bytes={originalSize}");
                metaContent.AppendLine($"sha256={hash}");
                metaContent.AppendLine($"collected_utc={DateTime.UtcNow:yyyy-MM-ddTHH:mm:ss.fffZ}");

                File.WriteAllText(metaFile, metaContent.ToString());
            }

            bool cancelEvent = false;
            _metadata.FireInformation(0, "RawStorageManager",
                $"JSON armazenado: {filepath} ({originalSize:N0} bytes)",
                "", 0, ref cancelEvent);

            return 0; // FileSystem mode doesn't return ID
        }

        /// <summary>
        /// Expande placeholders no caminho
        /// </summary>
        private string ExpandPath(string template, RawPayload payload)
        {
            DateTime now = DateTime.UtcNow;
            return template
                .Replace("{yyyy}", now.ToString("yyyy"))
                .Replace("{MM}", now.ToString("MM"))
                .Replace("{dd}", now.ToString("dd"))
                .Replace("{HH}", now.ToString("HH"))
                .Replace("{system}", payload.SystemName)
                .Replace("{environment}", payload.Environment)
                .Replace("{endpoint}", payload.Endpoint.Replace("/", "_").Replace("\\", "_"));
        }

        /// <summary>
        /// Comprime bytes usando GZIP
        /// </summary>
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

        /// <summary>
        /// Calcula hash SHA256
        /// </summary>
        private string ComputeSha256(byte[] data)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }

    /// <summary>
    /// Modos de armazenamento suportados
    /// </summary>
    public enum RawStorageMode
    {
        None,
        SqlVarbinary,
        FileSystem
    }

    /// <summary>
    /// Payload de JSON bruto com metadados
    /// </summary>
    public class RawPayload
    {
        public Guid CorrelationId { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = "GET";
        public string RequestUrl { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string JsonContent { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public int ElapsedMs { get; set; }
    }
}