Claro, Erton! Aqui estão **exemplos práticos de uso do componente Quatto API Client para SSIS**, cobrindo desde configuração básica até cenários avançados, todos alinhados à estrutura do projeto e aos arquivos de exemplos presentes na pasta `examples/` do seu repositório.[1](blob:https://outlook.office.com/a006e500-63ee-4d4e-b3ee-f81f6b3724c2)

***

# Exemplos de Uso — Quatto API Client (SSIS)

***

## 1. **Exemplo Básico: Consumo de API REST com Paginação**

**Cenário:**  
Extrair pedidos do endpoint `/v1/orders` da API Gladium, armazenando dados em uma tabela de staging.

### **Configuração do Data Flow**

*   **Connection Manager:**
    *   Tipo: APIConnection
    *   Autenticação: Bearer Token
    *   Parâmetro sensível: `ApiToken` (SSISDB)

*   **CorporateApiSource (Componente):**
    *   `BaseUrl`: `https://api.gladium.com.br`
    *   `Endpoint`: `/v1/orders`
    *   `PageSize`: `500`
    *   `QueryTemplate`: `"?since={Watermark:o}&page={Page}&pageSize={PageSize}"`
    *   `IncrementalWatermarkColumn`: `updatedAt`
    *   `RawStoreMode`: `FileSystem`
    *   `RawStorePathOrTable`: `D:\DW\Raw\PRD\Gladium\{yyyy}\{MM}\{dd}`
    *   `SchemaMappingJson`:
        ```json
        {
          "columns": [
            { "name": "order_id", "path": "$.id", "type": "DT_WSTR", "length": 50 },
            { "name": "updated_at", "path": "$.updatedAt", "type": "DT_DBTIMESTAMP2" },
            { "name": "status", "path": "$.status", "type": "DT_WSTR", "length": 20 },
            { "name": "amount_total", "path": "$.amount", "type": "DT_NUMERIC", "precision": 18, "scale": 2 }
          ]
        }
        ```
    *   **Saída:**
        *   OLE DB Destination → `stg.STG_Gladium_Orders`

***

## 2. **Exemplo Avançado: Foreach Loop para Múltiplos Endpoints**

**Cenário:**  
Processar vários endpoints (pedidos, clientes, produtos) em um único pacote SSIS usando Foreach Loop.

### **Estrutura**

*   **Tabela de controle:** `dbo.ApiEndpoints`
    *   Colunas: `SystemName`, `EndpointPath`, `PageSize`, `SchemaMappingJson`, `IsActive`

*   **Control Flow:**
    1.  Execute SQL Task para carregar endpoints ativos em uma variável Recordset.
    2.  Foreach Loop Container varre cada endpoint:
        *   Define variáveis: `SystemName`, `EndpointPath`, `PageSize`, `SchemaMappingJson`
        *   Executa Data Flow com CorporateApiSource configurado via Expressions.

### **Exemplo de Expression para o componente:**

```text
BaseUrl = "https://api.gladium.com.br"
Endpoint = @[User::EndpointPath]
PageSize = @[User::PageSize]
SchemaMappingJson = @[User::SchemaMappingJson]
RawStorePathOrTable = "D:\DW\Raw\PRD\" + @[User::SystemName] + "\{yyyy}\{MM}\{dd}"
```

***

## 3. **Exemplo: Extração Incremental com Watermark**

**Cenário:**  
Somente registros modificados após o último watermark são extraídos.

*   **Procedures SQL:**
    *   `usp_ApiWatermark_Get` para ler o último watermark
    *   `usp_ApiWatermark_Upsert` para atualizar após execução

*   **Configuração no componente:**
    *   `IncrementalWatermarkColumn`: `updatedAt`
    *   `QueryTemplate`: `"?since={Watermark:o}&page={Page}&pageSize={PageSize}"`

***

## 4. **Exemplo: Armazenamento de JSON Bruto para Auditoria**

**Cenário:**  
Todos os payloads recebidos da API são armazenados em disco para rastreabilidade.

*   **Configuração no componente:**
    *   `RawStoreMode`: `FileSystem`
    *   `RawStorePathOrTable`: `D:\DW\Raw\PRD\Gladium\{yyyy}\{MM}\{dd}`

*   **Resultado:**
    *   Arquivos `.json.gz` e `.meta` são gravados por página, contendo o payload e metadados (hash, status, tamanho).

***

## 5. **Exemplo: Tratamento de Erros e Quarentena**

**Cenário:**  
Linhas com erro de parsing ou mapeamento são redirecionadas para uma tabela de erros.

*   **Configuração no Data Flow:**
    *   Error Output do CorporateApiSource → OLE DB Destination → `stg.STG_Errors_Orders`
    *   Colunas: `error_time_utc`, `system_name`, `endpoint`, `correlation_id`, `error_code`, `error_column`, `payload_meta`

***

## 6. **Exemplo: Uso de Parâmetros Sensíveis no SSISDB**

**Cenário:**  
Tokens e segredos são configurados como parâmetros sensíveis do projeto SSISDB.

*   **Configuração:**
    *   Parâmetros: `ApiToken`, `OAuth2_ClientId`, `OAuth2_ClientSecret`, `DW_ConnectionString`
    *   Marcar como Sensível e referenciar via Expressions no componente.

***

## 7. **Exemplo: Logging e Telemetria**

**Cenário:**  
Monitorar quantidade de páginas, registros, tempo de resposta e ocorrências de retry/throttling.

*   **Configuração:**
    *   Habilitar logging no SSIS para eventos: `OnInformation`, `OnWarning`, `OnError`
    *   Consultar logs para métricas operacionais e auditoria.

***

## 8. **Exemplo: Integração com Azure (Cloud Shell/Storage Account)**

**Cenário:**  
Utilizar Storage Account para armazenar arquivos brutos em ambiente Azure.

*   **Configuração:**
    *   `RawStoreMode`: `FileSystem`
    *   `RawStorePathOrTable`: caminho UNC ou blob Azure (ex.: `\\storageaccount.file.core.windows.net\dwraw\PRD\Gladium\{yyyy}\{MM}\{dd}`)

***

## 9. **Exemplo: Uso em Ambiente de Homologação/Sandbox**

**Cenário:**  
Executar o componente em ambiente de testes sem afetar produção.

*   **Configuração:**
    *   Parâmetro `Environment`: `HML`
    *   `RawStorePathOrTable`: `D:\DW\Raw\HML\Gladium\{yyyy}\{MM}\{dd}`

***

## 10. **Exemplo: Mapeamento de Esquema para API Portal SESC**

**Cenário:**  
Consumir endpoint `/v1/clients` da API Portal SESC.

*   **SchemaMappingJson:**
    ```json
    {
      "columns": [
        { "name": "client_id", "path": "$.id", "type": "DT_WSTR", "length": 50 },
        { "name": "name", "path": "$.name", "type": "DT_WSTR", "length": 100 },
        { "name": "email", "path": "$.email", "type": "DT_WSTR", "length": 100 },
        { "name": "created_at", "path": "$.createdAt", "type": "DT_DBTIMESTAMP2" }
      ]
    }
    ```

***

Esses exemplos cobrem os principais cenários de uso do componente, desde o básico até o avançado, e podem ser adaptados conforme a evolução do projeto ou necessidades específicas de integração.

Se quiser exemplos para outros tipos de API, integração com outros sistemas, ou um pacote SSIS de referência completo, basta pedir!
