# ?? BLOCO 1.4 - PRÓXIMO: Tab "Incremental"

**Status BLOCO 1.3**: ? CONCLUÍDO  
**Próximo**: BLOCO 1.4  
**Tempo Estimado**: 1-1.5 horas  
**Prioridade**: ?? CRÍTICA  

---

## ?? O QUE SERÁ FEITO

### Tab "Incremental" (Incremental Tab)
Terceira aba do wizard com 4 controles para configurar carregamento incremental:

```
Forma Visual Esperada:
????????????????????????????????????????????
? Geral | Paginação | Incremental | ...    ?
????????????????????????????????????????????
?                                          ?
? ? Ativar Incremental                    ?
?                                          ?
? Coluna Watermark: [updated_at        ]   ?
?                                          ?
? Sistema:         [Gladium            ]   ?
?                                          ?
? Ambiente:        [? PRD                 ?
?                                          ?
????????????????????????????????????????????
```

---

## ?? CONTROLES A IMPLEMENTAR

### 1?? CheckBox - Ativar Incremental
```csharp
Label: "Ativar Incremental"
Control: CheckBox
  ?? Location: (10, 10)
  ?? Width: 200px
  ?? Height: 24px
  ?? Checked: False (default)
  ?? AutoSize: False
  
Propósito: Habilitar carregamento incremental
Default: Unchecked (desativado)
Importância: Crítica para retomar de ponto anterior
```

### 2?? TextBox - Coluna Watermark
```csharp
Label: "Coluna Watermark:"
Control: TextBox
  ?? Width: 300px
  ?? Height: 24px
  ?? Text: "updated_at"
  ?? Multiline: False
  ?? ReadOnly: False
  
Propósito: Campo usado como marca d'água (último valor)
Exemplo: "updated_at", "modified_date", "id"
Padrão: "updated_at"
Uso: Armazena último valor processado
```

### 3?? TextBox - Sistema
```csharp
Label: "Sistema:"
Control: TextBox
  ?? Width: 300px
  ?? Height: 24px
  ?? Text: "Gladium"
  ?? Multiline: False
  ?? ReadOnly: False
  
Propósito: Identificar sistema de origem
Exemplo: "Gladium", "PortalSESC", "APIv2"
Padrão: "Gladium"
Uso: Chave para armazenar watermark por sistema
```

### 4?? ComboBox - Ambiente
```csharp
Label: "Ambiente:"
Control: ComboBox
  ?? Width: 150px
  ?? Height: 24px
  ?? DropDownStyle: DropDownList
  ?? Items: ["DEV", "HML", "PRD"]
  
Propósito: Ambiente de execução
Valores: DEV (desenvolvimento), HML (homolog), PRD (prod)
Default: "PRD" (índice 2)
Uso: Chave para armazenar watermark por ambiente
```

---

## ?? LAYOUT DO CÓDIGO

### Estrutura Esperada:
```csharp
private void CreateIncrementalTab()
{
    var tab = new TabPage("Incremental");
    tab.Padding = new Padding(10);
    tabControl!.TabPages.Add(tab);

    // Definir constantes de layout
    int y = 10;
    const int labelWidth = 150;
    const int controlWidth = 300;
    const int controlHeight = 24;
    const int rowHeight = 35;
    const int leftMargin = 10;
    const int labelControlGap = 10;

    // 1. Adicionar CheckBox (EnableIncremental)
    // 2. Adicionar Label + TextBox (WatermarkColumn)
    // 3. Adicionar Label + TextBox (SourceSystem)
    // 4. Adicionar Label + ComboBox (Environment)
}
```

---

## ?? CÓDIGO INICIAL (COPIE E ADAPTE)

### Template Completo:
```csharp
private void CreateIncrementalTab()
{
    var tab = new TabPage("Incremental");
    tab.Padding = new Padding(10);
    tabControl!.TabPages.Add(tab);

    int y = 10;
    const int labelWidth = 150;
    const int controlWidth = 300;
    const int controlHeight = 24;
    const int rowHeight = 35;
    const int leftMargin = 10;
    const int labelControlGap = 10;

    // ???????????????????????????????????????????????????????????????
    // ATIVAR INCREMENTAL (CheckBox)
    // ???????????????????????????????????????????????????????????????
    chkEnableIncremental = new CheckBox
    {
        Text = "Ativar Incremental",
        Location = new Point(leftMargin, y),
        Width = 200,
        Height = controlHeight,
        Checked = false,
        AutoSize = false
    };
    tab.Controls.Add(chkEnableIncremental);
    y += rowHeight + 10;  // Extra space after checkbox

    // ???????????????????????????????????????????????????????????????
    // COLUNA WATERMARK (TextBox)
    // ???????????????????????????????????????????????????????????????
    var lblWatermarkColumn = new Label
    {
        Text = "Coluna Watermark:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblWatermarkColumn);

    txtWatermarkColumn = new TextBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = controlWidth,
        Height = controlHeight,
        Text = "updated_at",
        Multiline = false
    };
    tab.Controls.Add(txtWatermarkColumn);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // SISTEMA (TextBox)
    // ???????????????????????????????????????????????????????????????
    var lblSourceSystem = new Label
    {
        Text = "Sistema:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblSourceSystem);

    txtSourceSystem = new TextBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = controlWidth,
        Height = controlHeight,
        Text = "Gladium",
        Multiline = false
    };
    tab.Controls.Add(txtSourceSystem);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // AMBIENTE (ComboBox)
    // ???????????????????????????????????????????????????????????????
    var lblEnvironment = new Label
    {
        Text = "Ambiente:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblEnvironment);

    cmbEnvironment = new ComboBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = 150,
        Height = controlHeight,
        DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    };
    cmbEnvironment.Items.AddRange(new object[] { "DEV", "HML", "PRD" });
    cmbEnvironment.SelectedIndex = 2;  // Default: PRD
    tab.Controls.Add(cmbEnvironment);
}
```

---

## ?? PROPRIEDADES A ADICIONAR

Adicionar na seção de propriedades privadas (classe):
```csharp
// Incremental Tab Controls
private CheckBox? chkEnableIncremental;
private TextBox? txtWatermarkColumn;
private TextBox? txtSourceSystem;
private ComboBox? cmbEnvironment;
```

---

## ?? REFERÊNCIAS

**Arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`  
**Método**: `CreateIncrementalTab()`  
**Linhas esperadas**: ~95 linhas

**Documentação**:
- IMPLEMENTATION_GUIDE.md - Passo 6
- CorporateApiSource.cs - Propriedades incrementais
- BLOCO_1.3_RELATORIO.md - Padrão de implementação

---

## ? CHECKLIST ANTES DE COMEÇAR

- [x] BLOCO 1.3 ? concluído
- [x] Projeto compilado sem erros
- [x] Arquivo ApiSourceWizard.cs aberto
- [x] Método CreateIncrementalTab() localizado
- [x] Template acima pronto para copiar
- [x] Documentação IMPLEMENTATION_GUIDE.md disponível

---

## ?? ESTIMATIVA

| Etapa | Tempo |
|-------|-------|
| Setup + Labels | 0.15h |
| CheckBox enable | 0.1h |
| TextBox watermark | 0.15h |
| TextBox system | 0.15h |
| ComboBox environment | 0.2h |
| Layout + Spacing | 0.1h |
| Compilação + Testes | 0.15h |
| **TOTAL** | **1.0h** |

---

## ?? PRÓXIMO PASSO IMEDIATO

1. **Abrir arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`
2. **Localizar**: Método `CreateIncrementalTab()` (linha ~295)
3. **Remover**: `// TODO:` comment
4. **Adicionar**: Código do template acima
5. **Adicionar propriedades**: 4 novas (CheckBox + 2x TextBox + ComboBox)
6. **Compilar**: `dotnet build`
7. **Validar**: 0 erros, 0 warnings

---

## ?? NOTAS IMPORTANTES

### Campos a Guardar para LoadCurrentValues()
```csharp
private CheckBox? chkEnableIncremental;      // Para BLOCO 2.1
private TextBox? txtWatermarkColumn;         // Para BLOCO 2.1
private TextBox? txtSourceSystem;            // Para BLOCO 2.1
private ComboBox? cmbEnvironment;            // Para BLOCO 2.1
```

### Ambientes Suportados
```
DEV   - Desenvolvimento (testes locais)
HML   - Homologação (testes antes de produção)
PRD   - Produção (ambiente real)
```

### Usar como Chave
```
Watermark Key = Sistema + Ambiente + Endpoint
Exemplo: "Gladium_PRD_/v1/orders"
```

### Extra Space Depois de CheckBox
```
Normal: y += rowHeight
Após CheckBox: y += rowHeight + 10
```

---

## ? PADRÃO A SEGUIR

```csharp
// Consolidar padrão dos BLOCOs 1.2 e 1.3
// ???????????????????????????????????????????????????????????????
// NOME DO CAMPO (Tipo de Controle)
// ???????????????????????????????????????????????????????????????
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
- Verifique: BLOCO_1.3_RELATORIO.md (padrão similar)
- Template: Código acima é 100% copiável

---

## ?? PRÓXIMA META

Após BLOCO 1.4 concluído:
```
? BLOCO 1.2 (Tab General) - 1.5h
? BLOCO 1.3 (Tab Pagination) - 1.2h
? BLOCO 1.4 (Tab Incremental) - 1.0h    ? PRÓXIMO
  ?? BLOCO 1.5 (Tab Storage) - 1-1.5h
      ?? BLOCO 1.6 (Tab Advanced) - 1.5-2h
?????????????????????????????????????????????????????
Total Bloco 1: ~7.9-8.9h (dentro de 8-10h) ?
```

---

**Status**: ?? PRONTO PARA INICIAR  
**Data**: 2024-01-XX  
**Desenvolvedor**: [Seu Nome]  
**Próximo**: BLOCO 1.4 (Tab Incremental)

