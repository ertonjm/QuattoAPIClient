/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Schema Mapper
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Mapeador de campos JSON para colunas SSIS.
Suporta:
- JSONPath básico ($.field, $.nested.field)
- Conversão automática de tipos
- Valores default
- Tratamento de nulos

FUNCIONALIDADES:
- Mapeamento declarativo via JSON
- Suporte a tipos SSIS (DT_WSTR, DT_I8, DT_DBTIMESTAMP2, etc)
- Validação de schema em design-time
- Performance otimizada (cache de LineageIDs)

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace QuattoAPIClient.Source.Helpers
{
    /// <summary>
    /// Mapeador de campos JSON para colunas SSIS
    /// </summary>
    public class SchemaMapper
    {
        private readonly SchemaMapping _mapping;
        private readonly IDTSComponentMetaData100 _metadata;
        private readonly Dictionary<string, int> _lineageIdCache;

        public SchemaMapper(SchemaMapping mapping, IDTSComponentMetaData100 metadata)
        {
            _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _lineageIdCache = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            BuildLineageIdCache();
        }

        /// <summary>
        /// Cria mapper a partir de JSON string
        /// </summary>
        public static SchemaMapper FromJson(string json, IDTSComponentMetaData100 metadata)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new SchemaMapper(new SchemaMapping(), metadata);
            }

            var mapping = JsonSerializer.Deserialize<SchemaMapping>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return new SchemaMapper(mapping ?? new SchemaMapping(), metadata);
        }

        /// <summary>
        /// Mapeia registro JSON para buffer SSIS
        /// </summary>
        public void MapToBuffer(PipelineBuffer buffer, JsonElement record)
        {
            if (_mapping.Columns == null || _mapping.Columns.Count == 0)
                return;

            foreach (var columnMap in _mapping.Columns)
            {
                try
                {
                    if (!_lineageIdCache.TryGetValue(columnMap.Name, out int lineageId))
                    {
                        bool cancelEvent = false;
                        _metadata.FireWarning(0, "SchemaMapper",
                            $"Coluna '{columnMap.Name}' não encontrada no output. Pulando.",
                            "", 0);
                        continue;
                    }

                    JsonElement? value = ExtractJsonValue(record, columnMap.Path);
                    SetBufferValue(buffer, lineageId, columnMap, value);
                }
                catch (Exception ex)
                {
                    bool cancelEvent = false;
                    _metadata.FireWarning(0, "SchemaMapper",
                        $"Erro ao mapear coluna '{columnMap.Name}': {ex.Message}",
                        "", 0);

                    // Set null on error if column is nullable
                    if (_lineageIdCache.TryGetValue(columnMap.Name, out int lineageId))
                    {
                        try
                        {
                            int bufferIndex = buffer.GetColumnIndexByLineageID(lineageId);
                            buffer.SetNull(bufferIndex);
                        }
                        catch { /* ignore */ }
                    }
                }
            }
        }

        /// <summary>
        /// Extrai valor de JSON usando JSONPath simplificado
        /// </summary>
        private JsonElement? ExtractJsonValue(JsonElement root, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            // Remove leading $ if present
            path = path.TrimStart('$').TrimStart('.');

            if (string.IsNullOrEmpty(path))
                return root;

            // Split path by dots
            string[] parts = path.Split('.');
            JsonElement current = root;

            foreach (string part in parts)
            {
                if (current.ValueKind != JsonValueKind.Object)
                    return null;

                if (!current.TryGetProperty(part, out JsonElement next))
                    return null;

                current = next;
            }

            return current;
        }

        /// <summary>
        /// Define valor no buffer SSIS com conversão de tipo
        /// </summary>
        private void SetBufferValue(
            PipelineBuffer buffer,
            int lineageId,
            ColumnMapping columnMap,
            JsonElement? value)
        {
            int bufferIndex = buffer.GetColumnIndexByLineageID(lineageId);

            // Handle null
            if (!value.HasValue || value.Value.ValueKind == JsonValueKind.Null)
            {
                if (!string.IsNullOrEmpty(columnMap.DefaultValue))
                {
                    SetDefaultValue(buffer, bufferIndex, columnMap);
                }
                else
                {
                    buffer.SetNull(bufferIndex);
                }
                return;
            }

            JsonElement jsonValue = value.Value;

            // Map by SSIS data type
            switch (columnMap.Type.ToUpperInvariant())
            {
                case "DT_WSTR":
                case "DT_STR":
                    string strValue = jsonValue.ValueKind == JsonValueKind.String
                        ? jsonValue.GetString() ?? string.Empty
                        : jsonValue.ToString();

                    if (columnMap.Length > 0 && strValue.Length > columnMap.Length)
                        strValue = strValue.Substring(0, columnMap.Length);

                    buffer.SetString(bufferIndex, strValue);
                    break;

                case "DT_I4":
                    if (jsonValue.TryGetInt32(out int intValue))
                        buffer.SetInt32(bufferIndex, intValue);
                    else
                        buffer.SetNull(bufferIndex);
                    break;

                case "DT_I8":
                    if (jsonValue.TryGetInt64(out long longValue))
                        buffer.SetInt64(bufferIndex, longValue);
                    else
                        buffer.SetNull(bufferIndex);
                    break;

                case "DT_R8":
                    if (jsonValue.TryGetDouble(out double doubleValue))
                        buffer.SetDouble(bufferIndex, doubleValue);
                    else
                        buffer.SetNull(bufferIndex);
                    break;

                case "DT_NUMERIC":
                case "DT_DECIMAL":
                    if (jsonValue.TryGetDecimal(out decimal decimalValue))
                        buffer.SetDecimal(bufferIndex, decimalValue);
                    else
                        buffer.SetNull(bufferIndex);
                    break;

                case "DT_BOOL":
                    if (jsonValue.ValueKind == JsonValueKind.True)
                        buffer.SetBoolean(bufferIndex, true);
                    else if (jsonValue.ValueKind == JsonValueKind.False)
                        buffer.SetBoolean(bufferIndex, false);
                    else
                        buffer.SetNull(bufferIndex);
                    break;

                case "DT_DBTIMESTAMP":
                case "DT_DBTIMESTAMP2":
                case "DT_DATE":
                    if (jsonValue.ValueKind == JsonValueKind.String)
                    {
                        string? dateStr = jsonValue.GetString();
                        if (DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                            buffer.SetDateTime(bufferIndex, dateValue);
                        else
                            buffer.SetNull(bufferIndex);
                    }
                    else
                    {
                        buffer.SetNull(bufferIndex);
                    }
                    break;

                case "DT_GUID":
                    if (jsonValue.ValueKind == JsonValueKind.String)
                    {
                        string? guidStr = jsonValue.GetString();
                        if (Guid.TryParse(guidStr, out Guid guidValue))
                            buffer.SetGuid(bufferIndex, guidValue);
                        else
                            buffer.SetNull(bufferIndex);
                    }
                    else
                    {
                        buffer.SetNull(bufferIndex);
                    }
                    break;

                default:
                    // Unsupported type - set as string
                    buffer.SetString(bufferIndex, jsonValue.ToString());
                    break;
            }
        }

        /// <summary>
        /// Define valor default no buffer
        /// </summary>
        private void SetDefaultValue(PipelineBuffer buffer, int bufferIndex, ColumnMapping columnMap)
        {
            if (string.IsNullOrEmpty(columnMap.DefaultValue))
            {
                buffer.SetNull(bufferIndex);
                return;
            }

            try
            {
                switch (columnMap.Type.ToUpperInvariant())
                {
                    case "DT_WSTR":
                    case "DT_STR":
                        buffer.SetString(bufferIndex, columnMap.DefaultValue);
                        break;

                    case "DT_I4":
                        if (int.TryParse(columnMap.DefaultValue, out int intDefault))
                            buffer.SetInt32(bufferIndex, intDefault);
                        else
                            buffer.SetNull(bufferIndex);
                        break;

                    case "DT_I8":
                        if (long.TryParse(columnMap.DefaultValue, out long longDefault))
                            buffer.SetInt64(bufferIndex, longDefault);
                        else
                            buffer.SetNull(bufferIndex);
                        break;

                    case "DT_BOOL":
                        if (bool.TryParse(columnMap.DefaultValue, out bool boolDefault))
                            buffer.SetBoolean(bufferIndex, boolDefault);
                        else
                            buffer.SetNull(bufferIndex);
                        break;

                    default:
                        buffer.SetNull(bufferIndex);
                        break;
                }
            }
            catch
            {
                buffer.SetNull(bufferIndex);
            }
        }

        /// <summary>
        /// Constrói cache de LineageIDs para performance
        /// </summary>
        private void BuildLineageIdCache()
        {
            _lineageIdCache.Clear();

            foreach (IDTSOutput100 output in _metadata.OutputCollection)
            {
                if (output.IsErrorOut)
                    continue;

                foreach (IDTSOutputColumn100 column in output.OutputColumnCollection)
                {
                    _lineageIdCache[column.Name] = column.LineageID;
                }
            }
        }

        /// <summary>
        /// Aplica schema mapping ao output do componente
        /// </summary>
        public static void ApplyToOutput(IDTSOutput100 output, SchemaMapping mapping)
        {
            if (mapping?.Columns == null)
                return;

            output.OutputColumnCollection.RemoveAll();

            foreach (var columnMap in mapping.Columns)
            {
                IDTSOutputColumn100 column = output.OutputColumnCollection.New();
                column.Name = columnMap.Name;
                column.Description = $"Mapped from {columnMap.Path}";

                DataType dataType = MapToSSISDataType(columnMap.Type);

                column.SetDataTypeProperties(
                    dataType,
                    columnMap.Length,
                    columnMap.Scale,
                    columnMap.Precision,
                    0 // CodePage - 0 for Unicode
                );
            }
        }

        /// <summary>
        /// Mapeia string de tipo para DataType SSIS
        /// </summary>
        private static DataType MapToSSISDataType(string type)
        {
            return type.ToUpperInvariant() switch
            {
                "DT_WSTR" => DataType.DT_WSTR,
                "DT_STR" => DataType.DT_STR,
                "DT_I4" => DataType.DT_I4,
                "DT_I8" => DataType.DT_I8,
                "DT_R8" => DataType.DT_R8,
                "DT_NUMERIC" => DataType.DT_NUMERIC,
                "DT_DECIMAL" => DataType.DT_DECIMAL,
                "DT_BOOL" => DataType.DT_BOOL,
                "DT_DBTIMESTAMP" => DataType.DT_DBTIMESTAMP,
                "DT_DBTIMESTAMP2" => DataType.DT_DBTIMESTAMP2,
                "DT_DATE" => DataType.DT_DATE,
                "DT_GUID" => DataType.DT_GUID,
                _ => DataType.DT_WSTR
            };
        }
    }

    /// <summary>
    /// Definição de schema mapping
    /// </summary>
    public class SchemaMapping
    {
        public List<ColumnMapping> Columns { get; set; } = new List<ColumnMapping>();
    }

    /// <summary>
    /// Mapeamento de coluna individual
    /// </summary>
    public class ColumnMapping
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string Type { get; set; } = "DT_WSTR";
        public int Length { get; set; } = 4000;
        public int Precision { get; set; } = 18;
        public int Scale { get; set; } = 2;
        public string? DefaultValue { get; set; }
    }
}