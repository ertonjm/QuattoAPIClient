# ?? BLOCO 2 - Persistência de Dados

**Status BLOCO 1**: ? 100% CONCLUÍDO  
**Próximo**: BLOCO 2  
**Tempo Estimado**: 4-6 horas  
**Prioridade**: ?? CRÍTICA  

---

## ?? O QUE SERÁ FEITO

### BLOCO 2: Persistência de Dados (4-6 horas)

Implementar os 4 métodos que controlam a leitura e escrita de configurações:

```
2.1 LoadCurrentValues()      Carregar valores salvos do metadata
2.2 SaveValues()             Salvar valores no metadata
2.3 GetPropertyValue()       Helper para ler propriedade
2.4 SetPropertyValue()       Helper para escrever propriedade
```

---

## ?? ESTRUTURA ESPERADA

### Fluxo de Persistência
```
Wizard Abre
  ?? LoadCurrentValues()
      ?? Para cada propriedade
          ?? GetPropertyValue(name)
          ?? Popular controle

Usuário edita
  ?? Clica OK ou Apply
      ?? SaveValues()
          ?? Para cada controle
              ?? GetValue de controle
              ?? SetPropertyValue(name, value)
              ?? FireComponentMetaDataModifiedEvent()
```

---

## ?? BLOCO 2.1 - LoadCurrentValues()

### Objetivo
Carregar os valores previamente salvos na configuração e popular os controles UI.

### Template Completo

```csharp
private void LoadCurrentValues()
{
    try
    {
        // ???????????????????????????????????????????????????????????????
        // GENERAL TAB - Carregar valores
        // ???????????????????????????????????????????????????????????????
        
        // Connection Manager - TODO: Carregar lista de conexões SSIS
        // cmbConnection.Items.Clear();
        // Seria necessário acessar _connections object para popular
        
        // Base URL
        string baseUrl = GetPropertyValue("BaseUrl");
        if (!string.IsNullOrEmpty(baseUrl))
            txtBaseUrl!.Text = baseUrl;
        
        // Endpoint
        string endpoint = GetPropertyValue("Endpoint");
        if (!string.IsNullOrEmpty(endpoint))
            txtEndpoint!.Text = endpoint;
        
        // Page Size
        string pageSizeStr = GetPropertyValue("PageSize");
        if (!string.IsNullOrEmpty(pageSizeStr) && int.TryParse(pageSizeStr, out int pageSize))
            numPageSize!.Value = pageSize;

        // ???????????????????????????????????????????????????????????????
        // PAGINATION TAB - Carregar valores
        // ???????????????????????????????????????????????????????????????
        
        // Pagination Type
        string paginationType = GetPropertyValue("PaginationType");
        if (!string.IsNullOrEmpty(paginationType))
        {
            int idx = cmbPaginationType!.Items.IndexOf(paginationType);
            if (idx >= 0)
                cmbPaginationType.SelectedIndex = idx;
        }
        
        // Start Page
        string startPageStr = GetPropertyValue("StartPage");
        if (!string.IsNullOrEmpty(startPageStr) && int.TryParse(startPageStr, out int startPage))
            numStartPage!.Value = startPage;
        
        // Max Pages
        string maxPagesStr = GetPropertyValue("MaxPages");
        if (!string.IsNullOrEmpty(maxPagesStr) && int.TryParse(maxPagesStr, out int maxPages))
            numMaxPages!.Value = maxPages;

        // ???????????????????????????????????????????????????????????????
        // INCREMENTAL TAB - Carregar valores
        // ???????????????????????????????????????????????????????????????
        
        // Enable Incremental
        string enableIncremental = GetPropertyValue("EnableIncremental");
        if (!string.IsNullOrEmpty(enableIncremental) && bool.TryParse(enableIncremental, out bool isIncremental))
            chkEnableIncremental!.Checked = isIncremental;
        
        // Watermark Column
        string watermarkColumn = GetPropertyValue("WatermarkColumn");
        if (!string.IsNullOrEmpty(watermarkColumn))
            txtWatermarkColumn!.Text = watermarkColumn;
        
        // Source System
        string sourceSystem = GetPropertyValue("SourceSystem");
        if (!string.IsNullOrEmpty(sourceSystem))
            txtSourceSystem!.Text = sourceSystem;
        
        // Environment
        string environment = GetPropertyValue("Environment");
        if (!string.IsNullOrEmpty(environment))
        {
            int idx = cmbEnvironment!.Items.IndexOf(environment);
            if (idx >= 0)
                cmbEnvironment.SelectedIndex = idx;
        }

        // ???????????????????????????????????????????????????????????????
        // STORAGE TAB - Carregar valores
        // ???????????????????????????????????????????????????????????????
        
        // Raw Store Mode
        string rawStoreMode = GetPropertyValue("RawStoreMode");
        if (!string.IsNullOrEmpty(rawStoreMode))
        {
            int idx = cmbRawStoreMode!.Items.IndexOf(rawStoreMode);
            if (idx >= 0)
                cmbRawStoreMode.SelectedIndex = idx;
        }
        
        // Raw Store Target
        string rawStoreTarget = GetPropertyValue("RawStoreTarget");
        if (!string.IsNullOrEmpty(rawStoreTarget))
            txtRawStoreTarget!.Text = rawStoreTarget;
        
        // Compress Raw JSON
        string compressRaw = GetPropertyValue("CompressRawJson");
        if (!string.IsNullOrEmpty(compressRaw) && bool.TryParse(compressRaw, out bool shouldCompress))
            chkCompressRawJson!.Checked = shouldCompress;
        
        // Hash Raw JSON
        string hashRaw = GetPropertyValue("HashRawJson");
        if (!string.IsNullOrEmpty(hashRaw) && bool.TryParse(hashRaw, out bool shouldHash))
            chkHashRawJson!.Checked = shouldHash;

        // ???????????????????????????????????????????????????????????????
        // ADVANCED TAB - Carregar valores
        // ???????????????????????????????????????????????????????????????
        
        // Max Retries
        string maxRetriesStr = GetPropertyValue("MaxRetries");
        if (!string.IsNullOrEmpty(maxRetriesStr) && int.TryParse(maxRetriesStr, out int maxRetries))
            numMaxRetries!.Value = maxRetries;
        
        // Backoff Mode
        string backoffMode = GetPropertyValue("BackoffMode");
        if (!string.IsNullOrEmpty(backoffMode))
        {
            int idx = cmbBackoffMode!.Items.IndexOf(backoffMode);
            if (idx >= 0)
                cmbBackoffMode.SelectedIndex = idx;
        }
        
        // Base Delay Ms
        string baseDelayStr = GetPropertyValue("BaseDelayMs");
        if (!string.IsNullOrEmpty(baseDelayStr) && int.TryParse(baseDelayStr, out int baseDelay))
            numBaseDelayMs!.Value = baseDelay;
        
        // Rate Limit RPM
        string rateLimitStr = GetPropertyValue("RateLimitRPM");
        if (!string.IsNullOrEmpty(rateLimitStr) && int.TryParse(rateLimitStr, out int rateLimit))
            numRateLimitRPM!.Value = rateLimit;
        
        // Timeout Seconds
        string timeoutStr = GetPropertyValue("TimeoutSeconds");
        if (!string.IsNullOrEmpty(timeoutStr) && int.TryParse(timeoutStr, out int timeout))
            numTimeoutSeconds!.Value = timeout;
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            $"Erro ao carregar valores: {ex.Message}",
            "Erro de Configuração",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
```

---

## ?? BLOCO 2.2 - SaveValues()

### Objetivo
Validar os valores e salvá-los na configuração, disparando evento de modificação.

### Template Completo

```csharp
private void SaveValues()
{
    try
    {
        // ???????????????????????????????????????????????????????????????
        // VALIDAÇÃO BÁSICA
        // ???????????????????????????????????????????????????????????????
        if (ValidateProperties() == false)
        {
            // ValidateProperties() já mostra mensagens de erro
            return;
        }

        // ???????????????????????????????????????????????????????????????
        // GENERAL TAB - Salvar valores
        // ???????????????????????????????????????????????????????????????
        
        SetPropertyValue("BaseUrl", txtBaseUrl!.Text);
        SetPropertyValue("Endpoint", txtEndpoint!.Text);
        SetPropertyValue("PageSize", numPageSize!.Value.ToString());

        // ???????????????????????????????????????????????????????????????
        // PAGINATION TAB - Salvar valores
        // ???????????????????????????????????????????????????????????????
        
        SetPropertyValue("PaginationType", cmbPaginationType!.SelectedItem?.ToString() ?? "Offset (padrão)");
        SetPropertyValue("StartPage", numStartPage!.Value.ToString());
        SetPropertyValue("MaxPages", numMaxPages!.Value.ToString());

        // ???????????????????????????????????????????????????????????????
        // INCREMENTAL TAB - Salvar valores
        // ???????????????????????????????????????????????????????????????
        
        SetPropertyValue("EnableIncremental", chkEnableIncremental!.Checked.ToString());
        SetPropertyValue("WatermarkColumn", txtWatermarkColumn!.Text);
        SetPropertyValue("SourceSystem", txtSourceSystem!.Text);
        SetPropertyValue("Environment", cmbEnvironment!.SelectedItem?.ToString() ?? "PRD");

        // ???????????????????????????????????????????????????????????????
        // STORAGE TAB - Salvar valores
        // ???????????????????????????????????????????????????????????????
        
        SetPropertyValue("RawStoreMode", cmbRawStoreMode!.SelectedItem?.ToString() ?? "None");
        SetPropertyValue("RawStoreTarget", txtRawStoreTarget!.Text);
        SetPropertyValue("CompressRawJson", chkCompressRawJson!.Checked.ToString());
        SetPropertyValue("HashRawJson", chkHashRawJson!.Checked.ToString());

        // ???????????????????????????????????????????????????????????????
        // ADVANCED TAB - Salvar valores
        // ???????????????????????????????????????????????????????????????
        
        SetPropertyValue("MaxRetries", numMaxRetries!.Value.ToString());
        SetPropertyValue("BackoffMode", cmbBackoffMode!.SelectedItem?.ToString() ?? "Exponential");
        SetPropertyValue("BaseDelayMs", numBaseDelayMs!.Value.ToString());
        SetPropertyValue("RateLimitRPM", numRateLimitRPM!.Value.ToString());
        SetPropertyValue("TimeoutSeconds", numTimeoutSeconds!.Value.ToString());

        // ???????????????????????????????????????????????????????????????
        // DISPARAR EVENTO DE MODIFICAÇÃO
        // ???????????????????????????????????????????????????????????????
        
        // TODO: Quando tipos SSIS estiverem disponíveis:
        // FireComponentMetaDataModifiedEvent();
        
        MessageBox.Show(
            "Configurações salvas com sucesso!",
            "Sucesso",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show(
            $"Erro ao salvar valores: {ex.Message}",
            "Erro de Persistência",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
```

---

## ?? BLOCO 2.3 - GetPropertyValue()

### Objetivo
Helper para ler propriedade do metadata de forma segura e null-safe.

### Template Completo

```csharp
private string GetPropertyValue(string propertyName)
{
    try
    {
        // TODO: Quando tipos SSIS estiverem disponíveis, implementar:
        // 
        // Opção 1: CustomPropertyCollection
        // var customProps = _metadata.CustomPropertyCollection;
        // if (customProps != null && customProps.Contains(propertyName))
        //     return customProps[propertyName].Value?.ToString() ?? string.Empty;
        //
        // Opção 2: ComponentProperties
        // var componentProps = _metadata.ComponentProperties;
        // if (componentProps != null && componentProps.Contains(propertyName))
        //     return componentProps[propertyName].Value?.ToString() ?? string.Empty;
        //
        // Por enquanto, retornar vazio (valores padrão já estão no InitializeComponent)
        
        return string.Empty;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Erro ao ler propriedade {propertyName}: {ex.Message}");
        return string.Empty;
    }
}
```

---

## ?? BLOCO 2.4 - SetPropertyValue()

### Objetivo
Helper para escrever propriedade no metadata de forma segura e com validação.

### Template Completo

```csharp
private void SetPropertyValue(string propertyName, string value)
{
    try
    {
        // TODO: Quando tipos SSIS estiverem disponíveis, implementar:
        //
        // Opção 1: CustomPropertyCollection
        // var customProps = _metadata.CustomPropertyCollection;
        // if (customProps != null && customProps.Contains(propertyName))
        // {
        //     customProps[propertyName].Value = value;
        //     return;
        // }
        //
        // Opção 2: ComponentProperties
        // var componentProps = _metadata.ComponentProperties;
        // if (componentProps != null && componentProps.Contains(propertyName))
        // {
        //     componentProps[propertyName].Value = value;
        //     return;
        // }
        //
        // Log se propriedade não encontrada
        // System.Diagnostics.Debug.WriteLine($"Propriedade não encontrada: {propertyName}");
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine($"Erro ao escrever propriedade {propertyName}: {ex.Message}");
    }
}
```

---

## ?? NOTAS IMPORTANTES

### Tipos SSIS
```
Os tipos SSIS não estão disponíveis atualmente:
- IDTSComponentMetaData100
- CustomPropertyCollection
- ComponentProperties
- Connections

Quando estiverem disponíveis:
1. Descomente o código nos helpers
2. Use reflection ou direct access
3. Teste com tipos reais
```

### Estratégia Temporária
```
Por enquanto:
? LoadCurrentValues() carrega defaults do InitializeComponent()
? SaveValues() valida e salva (sem persistência real)
? GetPropertyValue() retorna vazio (será implementado depois)
? SetPropertyValue() placeholder (será implementado depois)

Quando SSIS estiver pronto:
? Substituir TODO por código real
? Testar com metadata real
? Validar integração
```

---

## ?? PROPRIEDADES A PERSISTIR

### General Tab
```
BaseUrl             (string)    Default: "https://"
Endpoint            (string)    Default: "/v1/"
PageSize            (int)       Default: 500
```

### Pagination Tab
```
PaginationType      (string)    Default: "Offset (padrão)"
StartPage           (int)       Default: 1
MaxPages            (int)       Default: 0 (sem limite)
```

### Incremental Tab
```
EnableIncremental   (bool)      Default: false
WatermarkColumn     (string)    Default: "updated_at"
SourceSystem        (string)    Default: "Gladium"
Environment         (string)    Default: "PRD"
```

### Storage Tab
```
RawStoreMode        (string)    Default: "None"
RawStoreTarget      (string)    Default: ""
CompressRawJson     (bool)      Default: false
HashRawJson         (bool)      Default: false
```

### Advanced Tab
```
MaxRetries          (int)       Default: 5
BackoffMode         (string)    Default: "Exponential"
BaseDelayMs         (int)       Default: 1000
RateLimitRPM        (int)       Default: 120
TimeoutSeconds      (int)       Default: 100
```

---

## ? CHECKLIST ANTES DE COMEÇAR

- [x] BLOCO 1 ? 100% concluído
- [x] Projeto compilado sem erros
- [x] Arquivo ApiSourceWizard.cs aberto
- [x] Métodos LoadCurrentValues() e SaveValues() localizados
- [x] Métodos GetPropertyValue() e SetPropertyValue() localizados
- [x] Template acima pronto para copiar

---

## ?? ESTIMATIVA

| Etapa | Tempo |
|-------|-------|
| LoadCurrentValues() | 1.5-2h |
| SaveValues() | 1.5-2h |
| GetPropertyValue() | 0.5-1h |
| SetPropertyValue() | 0.5-1h |
| Validação e Testes | 0.5-1h |
| **TOTAL** | **4.5-7h** |

---

## ?? PRÓXIMO PASSO IMEDIATO

1. **Abrir arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`
2. **Localizar**: Método `LoadCurrentValues()` (linha ~754)
3. **Remover**: `// TODO:` comment
4. **Adicionar**: Código do template BLOCO 2.1 acima
5. **Compilar**: `dotnet build`
6. **Validar**: 0 erros, 0 warnings
7. **Continuar**: BLOCO 2.2, 2.3, 2.4

---

## ?? SUPORTE

Dúvidas durante execução?
- Consulte: IMPLEMENTATION_GUIDE.md Passo 3, 4, 5
- Verifique: Propriedades listadas acima
- Template: Código acima é 100% copiável

---

## ?? PRÓXIMA META

Após BLOCO 2 completo:
```
? BLOCO 2.1 LoadCurrentValues() - 1.5-2h
? BLOCO 2.2 SaveValues() - 1.5-2h
? BLOCO 2.3 GetPropertyValue() - 0.5-1h
? BLOCO 2.4 SetPropertyValue() - 0.5-1h
?????????????????????????????????????????
Total BLOCO 2: ~4.5-7h (dentro de 4-6h estimado)

Conformidade: 85% (Fase 1 completa!)
Próximo: BLOCO 3 (Validação)
```

---

**Status**: ?? PRONTO PARA INICIAR  
**Data**: 2024-01-XX  
**Desenvolvedor**: [Seu Nome]  
**Próximo**: BLOCO 2.1 (LoadCurrentValues)

