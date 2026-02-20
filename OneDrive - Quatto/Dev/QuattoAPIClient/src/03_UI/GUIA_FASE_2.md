# ?? FASE 2 - Validação, UX e Testes (13-17 horas)

**Status FASE 1**: ? 100% CONCLUÍDO (9.4h / 12-16h)  
**Próximo**: FASE 2  
**Tempo Estimado**: 13-17 horas  
**Meta de Conformidade**: 95%  
**Target Framework**: .NET Framework 4.7.2  

---

## ?? OBJETIVO FASE 2

Melhorar validação, usabilidade e testes do wizard, atingindo 95% de conformidade.

```
Conformidade atual:  85% (após FASE 1)
Target FASE 2:       95%
Ganho esperado:      +10%
```

---

## BLOCO 3 - Validação Real-time (4-5 horas)

### BLOCO 3.1 - ValidateProperties() Melhorada

#### Objetivo
Expandir ValidateProperties() com validações mais completas e mensagens específicas.

#### O que adicionar
```csharp
? Validação de URL (HTTP/HTTPS, não vazia)
? Validação de PageSize (1-10000)
? Validação de WatermarkColumn (se incremental)
? Validação de Timeout (10-600)
? Validação de MaxRetries (0-10)
? Avisos (warnings) para configs extremas
? Armazenar erros para exibição
? Retornar lista de erros/warnings
```

#### Template
```csharp
private bool ValidateProperties()
{
    var errors = new List<string>();
    var warnings = new List<string>();

    // Validações...
    
    if (errors.Count > 0)
    {
        ShowValidationErrors(errors);
        return false;
    }
    
    if (warnings.Count > 0)
    {
        ShowValidationWarnings(warnings);
        // Retorna true (não bloqueia)
    }
    
    return true;
}
```

### BLOCO 3.2 - Real-time Validation

#### Objetivo
Validar ao sair do campo (Leave event) com feedback visual.

#### Controles a validar
```
BaseUrl:         Leave event
PageSize:        Leave event
WatermarkColumn: Leave event (se incremental)
Timeout:         Leave event
MaxRetries:      Leave event
```

#### Template
```csharp
private void txtBaseUrl_Leave(object? sender, EventArgs e)
{
    ValidateBaseUrl();
}

private void ValidateBaseUrl()
{
    if (string.IsNullOrWhiteSpace(txtBaseUrl!.Text))
    {
        SetFieldError(txtBaseUrl, "URL obrigatória");
        return;
    }
    
    if (!txtBaseUrl.Text.StartsWith("http"))
    {
        SetFieldError(txtBaseUrl, "Deve começar com http:// ou https://");
        return;
    }
    
    ClearFieldError(txtBaseUrl);
}

private void SetFieldError(Control control, string message)
{
    control.BackColor = Color.MistyRose;
    toolTip?.SetToolTip(control, message);
}

private void ClearFieldError(Control control)
{
    control.BackColor = SystemColors.Window;
    toolTip?.SetToolTip(control, "");
}
```

### BLOCO 3.3 - Mensagens Específicas

#### Objetivo
Criar mensagens de erro claras e acionáveis.

#### Mensagens
```
URL Vazia:
  "Base URL não pode estar vazia"

URL Inválida:
  "Base URL deve começar com https:// ou http://"

PageSize Inválido:
  "PageSize deve estar entre 1 e 10.000"

Watermark Obrigatório:
  "Coluna Watermark é obrigatória quando Incremental ativado"

Timeout Inválido:
  "Timeout deve estar entre 10 e 600 segundos"

MaxRetries Inválido:
  "Max Tentativas deve estar entre 0 e 10"
```

### BLOCO 3.4 - Avisos (Warnings)

#### Objetivo
Alertar usuário sobre configurações potencialmente problemáticas (não bloqueantes).

#### Avisos a implementar
```
RateLimit > 1000 RPM:
  "?? Taxa alta pode sobrecarregar API"

PageSize > 5000:
  "?? Pode ser lento com muitos registros"

Timeout < 30 segundos:
  "?? Pode ser curto demais para requisições longas"

BaseDelay > 30000 ms:
  "?? Delay muito alto entre retries"
```

---

## BLOCO 4 - Melhorias de UX (3-4 horas)

### BLOCO 4.1 - Labels com Tooltips

#### Objetivo
Adicionar tooltips descritivos em cada controle.

#### Tooltips
```
Connection:      "Selecione Connection Manager para API"
Base URL:        "URL base da API (ex: https://api.exemplo.com)"
Endpoint:        "Path do endpoint (ex: /v1/orders)"
PageSize:        "Quantidade de registros por página"
PaginationType:  "Estratégia de paginação"
StartPage:       "Primeira página a carregar"
MaxPages:        "0 = sem limite"
EnableIncremental: "Carregar apenas novos registros"
WatermarkColumn: "Coluna para rastrear último valor"
SourceSystem:    "Identificador do sistema"
Environment:     "Ambiente (DEV/HML/PRD)"
RawStoreMode:    "Como armazenar JSON bruto"
RawStoreTarget:  "Caminho ou coluna de destino"
CompressRawJson: "Remove espaços do JSON"
HashRawJson:     "Calcula SHA256 para validação"
MaxRetries:      "Tentativas em caso de erro"
BackoffMode:     "Estratégia de espera entre retries"
BaseDelayMs:     "Delay inicial em milissegundos"
RateLimitRPM:    "Limite de requisições por minuto"
TimeoutSeconds:  "Tempo máximo de espera"
```

### BLOCO 4.2 - Valores Padrão

#### Objetivo
Pré-preencher valores sensatos para acelerar configuração.

#### Valores já existentes
```
? PageSize: 500
? StartPage: 1
? MaxPages: 0 (sem limite)
? PaginationType: "Offset (padrão)"
? WatermarkColumn: "updated_at"
? SourceSystem: "Gladium"
? Environment: "PRD"
? RawStoreMode: "None"
? MaxRetries: 5
? BackoffMode: "Exponential"
? BaseDelayMs: 1000
? RateLimitRPM: 120
? TimeoutSeconds: 100
```

#### Adicionar em LoadCurrentValues
```csharp
// Se valor não existir no metadata, usar padrão
// Já está implementado! ?
```

### BLOCO 4.3 - Melhor Layout

#### Objetivo
Melhorar espaçamento e alinhamento profissional.

#### Improvements
```
? Padding consistente (10px)
? Alinhamento das colunas
? Height automático dos tabs
? Redimensionável

Potencial melhoria:
- Aumentar form size se necessário
- Melhorar espaçamento entre tabs
- Adicionar scroll se necessário
```

### BLOCO 4.4 - Agrupar com GroupBox

#### Objetivo
Agrupar campos relacionados visualmente.

#### Agrupamentos sugeridos
```
General Tab:
?? Conexão (ComboBox)
?? [GroupBox] Configuração da API
   ?? Base URL
   ?? Endpoint
   ?? Page Size

Incremental Tab:
?? [GroupBox] Controle Incremental
?  ?? Enable Incremental
?  ?? Watermark Column
?  ?? Source System
?? Environment

Storage Tab:
?? [GroupBox] Modo de Armazenamento
?  ?? Modo
?  ?? Alvo
?? [GroupBox] Processamento
   ?? Compactar JSON
   ?? Hash JSON
```

---

## BLOCO 5 - Testes (6-8 horas)

### BLOCO 5.1 - Compilação Release

#### Objective
Compilar em Release e validar performance.

```powershell
dotnet build -c Release
```

#### Validações
```
? 0 erros
? 0 warnings
? Build time < 30s
? DLL gerado com sucesso
```

### BLOCO 5.2 - Tipos SSIS Reais

#### Objective
Testar com assemblies SSIS reais quando disponíveis.

```csharp
// Descomente quando SSIS estiver disponível
// using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
// using Microsoft.SqlServer.Dts.Runtime;
```

### BLOCO 5.3 - Wizard em SSDT

#### Objective
Testar wizard completo no ambiente real.

```
1. Registrar componente na toolbox
2. Arrasta na data flow
3. Clica direito > Edit
4. Wizard abre sem erro
5. Carrega valores atuais
6. Permite editar
7. Salva valores
8. Valores persistem após reabrir
```

### BLOCO 5.4 - Connection Manager

#### Objective
Testar integração com Connection Manager.

```
1. Criar API Connection Manager
2. Carregar no combo do wizard
3. Validar autenticação funciona
```

### BLOCO 5.5 - Exemplos Práticos

#### Objective
Testar com exemplos reais.

```
Exemplos sugeridos:
?? Paginação offset (Gladium)
?? Paginação cursor (PortalSESC)
?? Incremental com watermark
?? Raw storage em FileSystem
?? Rate limiting e retries
```

---

## ?? ESTIMATIVA FASE 2

| Bloco | Tarefas | Tempo | Status |
|-------|---------|-------|--------|
| 3.1 | Validação | 1-1.5h | ? TODO |
| 3.2 | Real-time | 1-1.5h | ? TODO |
| 3.3 | Mensagens | 0.5-1h | ? TODO |
| 3.4 | Avisos | 0.5-1h | ? TODO |
| 4.1 | Tooltips | 0.5-1h | ? TODO |
| 4.2 | Defaults | 0.5-1h | ? TODO |
| 4.3 | Layout | 0.5-1h | ? TODO |
| 4.4 | GroupBox | 0.5-1h | ? TODO |
| 5.1-5.5 | Testes | 6-8h | ? TODO |
| **Total** | **17** | **13-17h** | **? READY** |

---

## ?? PRÓXIMO PASSO

### BLOCO 3.1 - Começar com Validação Melhorada

1. Melhorar ValidateProperties() existente
2. Adicionar coleta de erros e warnings
3. Implementar ShowValidationErrors()
4. Implementar ShowValidationWarnings()
5. Compilar e validar

**Tempo**: 1-1.5h

---

## ? PRÉ-REQUISITOS

- [x] FASE 1 ? 100% concluído
- [x] Projeto compilado sem erros
- [x] .NET Framework 4.7.2 configurado
- [x] ValidateProperties() já existe (base)
- [x] UI completa com 28 propriedades

---

**Status**: ?? PRONTO PARA INICIAR FASE 2  
**Data**: 2024-01-XX  
**Próximo**: BLOCO 3.1 (Validação Melhorada)

