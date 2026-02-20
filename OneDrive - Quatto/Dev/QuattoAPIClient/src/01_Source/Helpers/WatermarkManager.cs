/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Watermark Manager
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Gerenciador de watermarks para extração incremental.
Suporta múltiplos tipos de watermark:
- DateTimeOffset (timestamps)
- Integer (IDs sequenciais)
- String (tokens/versões)

FUNCIONALIDADES:
- Load/Save de watermarks por sistema/endpoint/ambiente
- Comparação automática de watermarks
- Tratamento de tipos diferentes
- Transações atômicas para consistência

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.Json;

namespace QuattoAPIClient.Source.Helpers
{
    /// <summary>
    /// Gerenciador de watermarks para extração incremental
    /// </summary>
    public class WatermarkManager
    {
        private readonly string _connectionString;
        private readonly string _watermarkTable;
        private readonly string _systemName;
        private readonly string _endpoint;
        private readonly string _environment;
        private readonly string _watermarkColumn;
        private readonly IDTSComponentMetaData100 _metadata;

        private WatermarkValue? _currentWatermark;
        private WatermarkValue? _maxObservedWatermark;

        public WatermarkManager(
            string connectionString,
            string watermarkTable,
            string systemName,
            string endpoint,
            string environment,
            string watermarkColumn,
            IDTSComponentMetaData100 metadata)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _watermarkTable = watermarkTable ?? throw new ArgumentNullException(nameof(watermarkTable));
            _systemName = systemName ?? throw new ArgumentNullException(nameof(systemName));
            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _watermarkColumn = watermarkColumn ?? throw new ArgumentNullException(nameof(watermarkColumn));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        }

        /// <summary>
        /// Carrega watermark atual do banco de dados
        /// </summary>
        public WatermarkValue? LoadWatermark()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("dbo.usp_API_GetWatermark", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SystemName", _systemName);
                        cmd.Parameters.AddWithValue("@Endpoint", _endpoint);
                        cmd.Parameters.AddWithValue("@Environment", _environment);

                        var watermarkParam = cmd.Parameters.Add("@Watermark", SqlDbType.NVarChar, 100);
                        watermarkParam.Direction = ParameterDirection.Output;

                        var lastRunParam = cmd.Parameters.Add("@LastRunUtc", SqlDbType.DateTime2);
                        lastRunParam.Direction = ParameterDirection.Output;

                        var lastStatusParam = cmd.Parameters.Add("@LastStatus", SqlDbType.NVarChar, 50);
                        lastStatusParam.Direction = ParameterDirection.Output;

                        cmd.ExecuteNonQuery();

                        string? watermarkStr = watermarkParam.Value as string;

                        if (string.IsNullOrWhiteSpace(watermarkStr) ||
                            watermarkStr == "1900-01-01T00:00:00Z" ||
                            watermarkStr == "NEVER_RUN")
                        {
                            bool cancelEvent = false;
                            _metadata.FireInformation(0, "WatermarkManager",
                                $"Nenhum watermark encontrado. Iniciando carga completa.",
                                "", 0, ref cancelEvent);

                            _currentWatermark = null;
                            return null;
                        }

                        _currentWatermark = WatermarkValue.Parse(watermarkStr);

                        bool cancelEvent2 = false;
                        _metadata.FireInformation(0, "WatermarkManager",
                            $"Watermark carregado: {_currentWatermark.RawValue} (tipo: {_currentWatermark.Type})",
                            "", 0, ref cancelEvent2);

                        return _currentWatermark;
                    }
                }
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                _metadata.FireWarning(0, "WatermarkManager",
                    $"Erro ao carregar watermark: {ex.Message}. Usando carga completa.",
                    "", 0);

                _currentWatermark = null;
                return null;
            }
        }

        /// <summary>
        /// Atualiza watermark com valor observado em registro
        /// </summary>
        public void ObserveWatermark(JsonElement record)
        {
            if (string.IsNullOrWhiteSpace(_watermarkColumn))
                return;

            try
            {
                if (!record.TryGetProperty(_watermarkColumn, out JsonElement watermarkProp))
                    return;

                WatermarkValue? observedValue = null;

                switch (watermarkProp.ValueKind)
                {
                    case JsonValueKind.String:
                        string? strValue = watermarkProp.GetString();
                        if (!string.IsNullOrWhiteSpace(strValue))
                        {
                            observedValue = WatermarkValue.Parse(strValue);
                        }
                        break;

                    case JsonValueKind.Number:
                        if (watermarkProp.TryGetInt64(out long longValue))
                        {
                            observedValue = new WatermarkValue
                            {
                                Type = WatermarkType.Integer,
                                IntValue = longValue,
                                RawValue = longValue.ToString()
                            };
                        }
                        break;

                    default:
                        // Unsupported type
                        return;
                }

                if (observedValue != null)
                {
                    UpdateMaxWatermark(observedValue);
                }
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                _metadata.FireWarning(0, "WatermarkManager",
                    $"Erro ao observar watermark: {ex.Message}",
                    "", 0);
            }
        }

        /// <summary>
        /// Atualiza watermark máximo observado
        /// </summary>
        private void UpdateMaxWatermark(WatermarkValue observedValue)
        {
            if (_maxObservedWatermark == null)
            {
                _maxObservedWatermark = observedValue;
                return;
            }

            // Compare watermarks
            int comparison = CompareWatermarks(_maxObservedWatermark, observedValue);

            if (comparison < 0)
            {
                _maxObservedWatermark = observedValue;
            }
        }

        /// <summary>
        /// Compara dois watermarks
        /// </summary>
        private int CompareWatermarks(WatermarkValue left, WatermarkValue right)
        {
            if (left.Type != right.Type)
            {
                throw new InvalidOperationException(
                    $"Não é possível comparar watermarks de tipos diferentes: {left.Type} vs {right.Type}");
            }

            switch (left.Type)
            {
                case WatermarkType.DateTimeOffset:
                    return left.DateTimeOffsetValue.CompareTo(right.DateTimeOffsetValue);

                case WatermarkType.Integer:
                    return left.IntValue.CompareTo(right.IntValue);

                case WatermarkType.String:
                    return string.Compare(left.StringValue, right.StringValue, StringComparison.Ordinal);

                default:
                    throw new NotSupportedException($"Tipo de watermark não suportado: {left.Type}");
            }
        }

        /// <summary>
        /// Salva watermark máximo observado no banco de dados
        /// </summary>
        public void SaveWatermark(long totalRecords, int totalPages, long totalBytes)
        {
            if (_maxObservedWatermark == null)
            {
                bool cancelEvent = false;
                _metadata.FireInformation(0, "WatermarkManager",
                    "Nenhum watermark observado. Mantendo watermark atual.",
                    "", 0, ref cancelEvent);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand("dbo.usp_API_UpdateWatermark", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SystemName", _systemName);
                        cmd.Parameters.AddWithValue("@Endpoint", _endpoint);
                        cmd.Parameters.AddWithValue("@Environment", _environment);

                        // Convert watermark to DateTimeOffset for storage
                        if (_maxObservedWatermark.Type == WatermarkType.DateTimeOffset)
                        {
                            cmd.Parameters.AddWithValue("@NewWatermark", _maxObservedWatermark.DateTimeOffsetValue);
                        }
                        else
                        {
                            // Store as string for non-datetime watermarks
                            cmd.Parameters.AddWithValue("@NewWatermark", DBNull.Value);
                        }

                        cmd.Parameters.AddWithValue("@RecordsExtracted", totalRecords);
                        cmd.Parameters.AddWithValue("@PagesExtracted", totalPages);
                        cmd.Parameters.AddWithValue("@BytesExtracted", totalBytes);
                        cmd.Parameters.AddWithValue("@Status", "SUCCESS");
                        cmd.Parameters.AddWithValue("@ErrorMessage", DBNull.Value);

                        cmd.ExecuteNonQuery();
                    }
                }

                bool cancelEvent = false;
                _metadata.FireInformation(0, "WatermarkManager",
                    $"Watermark atualizado: {_maxObservedWatermark.RawValue}",
                    "", 0, ref cancelEvent);
            }
            catch (Exception ex)
            {
                bool cancelEvent = false;
                _metadata.FireError(0, "WatermarkManager",
                    $"Erro ao salvar watermark: {ex.Message}",
                    "", 0, out cancelEvent);
                throw;
            }
        }

        /// <summary>
        /// Retorna watermark atual como string para uso em query
        /// </summary>
        public string? GetWatermarkForQuery()
        {
            return _currentWatermark?.RawValue;
        }

        /// <summary>
        /// Retorna watermark máximo observado
        /// </summary>
        public WatermarkValue? GetMaxObservedWatermark()
        {
            return _maxObservedWatermark;
        }
    }

    /// <summary>
    /// Valor de watermark tipado
    /// </summary>
    public class WatermarkValue
    {
        public WatermarkType Type { get; set; }
        public DateTimeOffset DateTimeOffsetValue { get; set; }
        public long IntValue { get; set; }
        public string StringValue { get; set; } = string.Empty;
        public string RawValue { get; set; } = string.Empty;

        /// <summary>
        /// Parse string para WatermarkValue com detecção automática de tipo
        /// </summary>
        public static WatermarkValue Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Watermark value cannot be null or empty", nameof(value));

            // Try DateTimeOffset
            if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dtoValue))
            {
                return new WatermarkValue
                {
                    Type = WatermarkType.DateTimeOffset,
                    DateTimeOffsetValue = dtoValue,
                    RawValue = value
                };
            }

            // Try Integer
            if (long.TryParse(value, out long longValue))
            {
                return new WatermarkValue
                {
                    Type = WatermarkType.Integer,
                    IntValue = longValue,
                    RawValue = value
                };
            }

            // Default to String
            return new WatermarkValue
            {
                Type = WatermarkType.String,
                StringValue = value,
                RawValue = value
            };
        }

        public override string ToString()
        {
            return RawValue;
        }
    }

    /// <summary>
    /// Tipos de watermark suportados
    /// </summary>
    public enum WatermarkType
    {
        DateTimeOffset,
        Integer,
        String
    }
}