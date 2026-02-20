# ?? BLOCO 1.6 - PRÓXIMO: Tab "Advanced"

**Status BLOCO 1.5**: ? CONCLUÍDO  
**Próximo**: BLOCO 1.6  
**Tempo Estimado**: 1.5-2 horas  
**Prioridade**: ?? CRÍTICA  

---

## ?? O QUE SERÁ FEITO

### Tab "Advanced" (Advanced Tab)
Quinta e última aba do wizard com 5 controles para configurações avançadas:

```
Forma Visual Esperada:
????????????????????????????????????????????
? Geral | Paginação | Incremental | ...    ?
????????????????????????????????????????????
?                                          ?
? Max Tentativas:     [5              ]    ?
?                                          ?
? Modo Backoff:       [? Exponential      ?
?                                          ?
? Delay Base (ms):    [1000           ]    ?
?                                          ?
? Rate Limit (rpm):   [120            ]    ?
?                                          ?
? Timeout (seg):      [100            ]    ?
?                                          ?
????????????????????????????????????????????
```

---

## ?? CONTROLES A IMPLEMENTAR

### 1?? NumericUpDown - Max Retries
```csharp
Label: "Max Tentativas:"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 5 (default)
  ?? Minimum: 0
  ?? Maximum: 10
  ?? Increment: 1
  ?? DecimalPlaces: 0
  
Propósito: Número máximo de tentativas
Default: 5
Range: 0-10 (sane limits)
Uso: Retry automático em falhas
```

### 2?? ComboBox - Backoff Mode
```csharp
Label: "Modo Backoff:"
Control: ComboBox
  ?? Width: 150px
  ?? Height: 24px
  ?? DropDownStyle: DropDownList
  ?? Items: [
       "Linear",
       "Exponential",
       "Random"
     ]
  
Propósito: Estratégia de espera entre retries
Default: "Exponential" (índice 1)
Valores:
  - Linear: Aumento linear (1s, 2s, 3s...)
  - Exponential: Exponencial (1s, 2s, 4s, 8s...)
  - Random: Aleatório
```

### 3?? NumericUpDown - Base Delay (ms)
```csharp
Label: "Delay Base (ms):"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 1000 (1 segundo)
  ?? Minimum: 100
  ?? Maximum: 60000 (1 minuto)
  ?? Increment: 100
  ?? DecimalPlaces: 0
  
Propósito: Delay base em milissegundos
Default: 1000 (1 segundo)
Range: 100-60000 ms
Uso: Multiplicado por retry count
```

### 4?? NumericUpDown - Rate Limit (rpm)
```csharp
Label: "Rate Limit (rpm):"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 120 (2 req/seg)
  ?? Minimum: 1
  ?? Maximum: 10000
  ?? Increment: 10
  ?? DecimalPlaces: 0
  
Propósito: Limite de requisições por minuto
Default: 120 (2 por segundo)
Range: 1-10000 rpm
Uso: Throttling para não sobrecarregar API
```

### 5?? NumericUpDown - Timeout (segundos)
```csharp
Label: "Timeout (seg):"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 100 (padrão)
  ?? Minimum: 10
  ?? Maximum: 600 (10 minutos)
  ?? Increment: 10
  ?? DecimalPlaces: 0
  
Propósito: Timeout máximo em segundos
Default: 100 segundos
Range: 10-600 segundos
Uso: Tempo máximo para aguardar resposta
```

---

## ?? LAYOUT DO CÓDIGO

### Estrutura Esperada:
```csharp
private void CreateAdvancedTab()
{
    var tab = new TabPage("Avançado");
    tab.Padding = new Padding(10);
    tabControl!.TabPages.Add(tab);

    // Definir constantes de layout
    int y = 10;
    const int labelWidth = 150;
    const int controlWidth = 300;
    const int numericWidth = 150;
    const int controlHeight = 24;
    const int rowHeight = 35;
    const int leftMargin = 10;
    const int labelControlGap = 10;

    // 1. Adicionar Label + NumericUpDown (MaxRetries)
    // 2. Adicionar Label + ComboBox (BackoffMode)
    // 3. Adicionar Label + NumericUpDown (BaseDelayMs)
    // 4. Adicionar Label + NumericUpDown (RateLimitRPM)
    // 5. Adicionar Label + NumericUpDown (TimeoutSeconds)
}
```

---

## ?? CÓDIGO INICIAL (COPIE E ADAPTE)

### Template Completo:
```csharp
private void CreateAdvancedTab()
{
    var tab = new TabPage("Avançado");
    tab.Padding = new Padding(10);
    tabControl!.TabPages.Add(tab);

    int y = 10;
    const int labelWidth = 150;
    const int numericWidth = 150;
    const int controlHeight = 24;
    const int rowHeight = 35;
    const int leftMargin = 10;
    const int labelControlGap = 10;

    // ???????????????????????????????????????????????????????????????
    // MAX RETRIES (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblMaxRetries = new Label
    {
        Text = "Max Tentativas:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblMaxRetries);

    numMaxRetries = new NumericUpDown
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = numericWidth,
        Height = controlHeight,
        Value = 5,
        Minimum = 0,
        Maximum = 10,
        Increment = 1,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numMaxRetries);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // BACKOFF MODE (ComboBox)
    // ???????????????????????????????????????????????????????????????
    var lblBackoffMode = new Label
    {
        Text = "Modo Backoff:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblBackoffMode);

    cmbBackoffMode = new ComboBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = numericWidth,
        Height = controlHeight,
        DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    };
    cmbBackoffMode.Items.AddRange(new object[] { "Linear", "Exponential", "Random" });
    cmbBackoffMode.SelectedIndex = 1;  // Default: Exponential
    tab.Controls.Add(cmbBackoffMode);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // BASE DELAY (ms) (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblBaseDelayMs = new Label
    {
        Text = "Delay Base (ms):",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblBaseDelayMs);

    numBaseDelayMs = new NumericUpDown
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = numericWidth,
        Height = controlHeight,
        Value = 1000,
        Minimum = 100,
        Maximum = 60000,
        Increment = 100,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numBaseDelayMs);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // RATE LIMIT (RPM) (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblRateLimitRPM = new Label
    {
        Text = "Rate Limit (rpm):",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblRateLimitRPM);

    numRateLimitRPM = new NumericUpDown
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = numericWidth,
        Height = controlHeight,
        Value = 120,
        Minimum = 1,
        Maximum = 10000,
        Increment = 10,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numRateLimitRPM);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // TIMEOUT (SEGUNDOS) (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblTimeoutSeconds = new Label
    {
        Text = "Timeout (seg):",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblTimeoutSeconds);

    numTimeoutSeconds = new NumericUpDown
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = numericWidth,
        Height = controlHeight,
        Value = 100,
        Minimum = 10,
        Maximum = 600,
        Increment = 10,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numTimeoutSeconds);
}
```

---

## ?? PROPRIEDADES A ADICIONAR

Adicionar na seção de propriedades privadas (classe):
```csharp
// Advanced Tab Controls
private NumericUpDown? numMaxRetries;
private ComboBox? cmbBackoffMode;
private NumericUpDown? numBaseDelayMs;
private NumericUpDown? numRateLimitRPM;
private NumericUpDown? numTimeoutSeconds;
```

---

## ?? REFERÊNCIAS

**Arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`  
**Método**: `CreateAdvancedTab()`  
**Linhas esperadas**: ~130 linhas

**Documentação**:
- IMPLEMENTATION_GUIDE.md - Passo 6
- CorporateApiSource.cs - Propriedades avançadas
- BLOCO_1.5_RELATORIO.md - Padrão de implementação

---

## ? CHECKLIST ANTES DE COMEÇAR

- [x] BLOCO 1.5 ? concluído
- [x] Projeto compilado sem erros
- [x] Arquivo ApiSourceWizard.cs aberto
- [x] Método CreateAdvancedTab() localizado
- [x] Template acima pronto para copiar
- [x] Documentação IMPLEMENTATION_GUIDE.md disponível

---

## ?? ESTIMATIVA

| Etapa | Tempo |
|-------|-------|
| Setup + Labels | 0.2h |
| NumericUpDown max retries | 0.15h |
| ComboBox backoff mode | 0.15h |
| NumericUpDown base delay | 0.15h |
| NumericUpDown rate limit | 0.15h |
| NumericUpDown timeout | 0.15h |
| Layout + Spacing | 0.2h |
| Compilação + Testes | 0.2h |
| **TOTAL** | **1.35h** |

---

## ?? PRÓXIMO PASSO IMEDIATO

1. **Abrir arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`
2. **Localizar**: Método `CreateAdvancedTab()` (linha ~520)
3. **Remover**: `// TODO:` comment
4. **Adicionar**: Código do template acima
5. **Adicionar propriedades**: 5 novas (4x NumericUpDown + 1x ComboBox)
6. **Compilar**: `dotnet build`
7. **Validar**: 0 erros, 0 warnings

---

## ?? NOTAS IMPORTANTES

### Campos a Guardar para LoadCurrentValues()
```csharp
private NumericUpDown? numMaxRetries;      // Para BLOCO 2.1
private ComboBox? cmbBackoffMode;          // Para BLOCO 2.1
private NumericUpDown? numBaseDelayMs;     // Para BLOCO 2.1
private NumericUpDown? numRateLimitRPM;    // Para BLOCO 2.1
private NumericUpDown? numTimeoutSeconds;  // Para BLOCO 2.1
```

### Modo Backoff
```
Linear:      1s, 2s, 3s, 4s, 5s...
Exponential: 1s, 2s, 4s, 8s, 16s... ? RECOMENDADO
Random:      Aleatório entre min e max
```

### Rate Limit
```
120 rpm = 2 requisições por segundo
600 rpm = 10 requisições por segundo
```

---

## ? PADRÃO A SEGUIR

```csharp
// Consolidar padrão final de todos os BLOCOs
// ???????????????????????????????????????????????????????????????
// NOME DO CAMPO (Tipo de Controle)
// ?????????????????????????????????????????????????????????????
var lblName = new Label { ... };
tab.Controls.Add(lblName);

var cntrlName = new ControlType { ... };
tab.Controls.Add(cntrlName);
y += rowHeight;
```

---

## ?? SUPORTE

Dúvidas durante execução?
- Consulte: IMPLEMENTATION_GUIDE.md Passo 6
- Verifique: BLOCO_1.5_RELATORIO.md (padrão similar)
- Template: Código acima é 100% copiável

---

## ?? CONCLUSÃO DO BLOCO 1

Após BLOCO 1.6 concluído:
```
? BLOCO 1.1 (InitializeComponent) - 1.0h
? BLOCO 1.2 (Tab General) - 1.5h
? BLOCO 1.3 (Tab Pagination) - 1.2h
? BLOCO 1.4 (Tab Incremental) - 1.0h
? BLOCO 1.5 (Tab Storage) - 1.0h
? BLOCO 1.6 (Tab Advanced) - 1.8h    ? ÚLTIMO
?????????????????????????????????????????????????????
Total BLOCO 1: ~7.5h (dentro de 8-10h) ?

Próximo: BLOCO 2 (Persistência) - 4-6 horas
```

---

**Status**: ?? PRONTO PARA INICIAR  
**Data**: 2024-01-XX  
**Desenvolvedor**: [Seu Nome]  
**Próximo**: BLOCO 1.6 (Tab Advanced - ÚLTIMA ABA)

