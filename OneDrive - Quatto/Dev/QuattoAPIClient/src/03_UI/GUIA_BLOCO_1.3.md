# ?? BLOCO 1.3 - PRÓXIMO: Tab "Paginação"

**Status BLOCO 1.2**: ? CONCLUÍDO  
**Próximo**: BLOCO 1.3  
**Tempo Estimado**: 1-1.5 horas  
**Prioridade**: ?? CRÍTICA  

---

## ?? O QUE SERÁ FEITO

### Tab "Paginação" (Pagination Tab)
Segunda aba do wizard com 3 controles para configurar tipo de paginação:

```
Forma Visual Esperada:
????????????????????????????????????????????
? Geral | Paginação | Incremental | ...    ?
????????????????????????????????????????????
?                                          ?
? Tipo Paginação: [? Offset               ?
?                                          ?
? Página Inicial:  [1              ]      ?
?                                          ?
? Máx Páginas:    [0              ]       ?
?                 (0 = sem limite)         ?
?                                          ?
????????????????????????????????????????????
```

---

## ?? CONTROLES A IMPLEMENTAR

### 1?? ComboBox - Tipo Paginação
```csharp
Label: "Tipo Paginação:"
Control: ComboBox
  ?? Width: 300px
  ?? Height: 24px
  ?? DropDownStyle: DropDownList
  ?? Items: [
       "Offset (padrão)",
       "Cursor",
       "Link-based",
       "None (sem paginação)"
     ]
  
Propósito: Selecionar estratégia de paginação
Default: "Offset (padrão)"
Importância: Crítica para carregamento de dados
```

### 2?? NumericUpDown - Página Inicial
```csharp
Label: "Página Inicial:"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 1
  ?? Minimum: 1
  ?? Maximum: 999999
  ?? Increment: 1
  ?? DecimalPlaces: 0
  
Propósito: Especificar página inicial de carregamento
Padrão: 1
Uso: Retomar de página específica
```

### 3?? NumericUpDown - Máx Páginas
```csharp
Label: "Máx Páginas:"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 0 (sem limite)
  ?? Minimum: 0
  ?? Maximum: 999999
  ?? Increment: 1
  ?? DecimalPlaces: 0
  
Propósito: Limitar número de páginas a carregar
Default: 0 (sem limite)
Uso: Testes, debug, limitar volume
```

---

## ?? LAYOUT DO CÓDIGO

### Estrutura Esperada:
```csharp
private void CreatePaginationTab()
{
    var tab = new TabPage("Paginação");
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

    // 1. Adicionar Label + ComboBox (Tipo Paginação)
    // 2. Adicionar Label + NumericUpDown (StartPage)
    // 3. Adicionar Label + NumericUpDown (MaxPages)
}
```

---

## ?? CÓDIGO INICIAL (COPIE E ADAPTE)

### Template Completo:
```csharp
private void CreatePaginationTab()
{
    var tab = new TabPage("Paginação");
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
    // TIPO PAGINAÇÃO (ComboBox)
    // ???????????????????????????????????????????????????????????????
    var lblPaginationType = new Label
    {
        Text = "Tipo Paginação:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblPaginationType);

    cmbPaginationType = new ComboBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = controlWidth,
        Height = controlHeight,
        DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    };
    cmbPaginationType.Items.AddRange(new object[]
    {
        "Offset (padrão)",
        "Cursor",
        "Link-based",
        "None (sem paginação)"
    });
    cmbPaginationType.SelectedIndex = 0;  // Default: Offset
    tab.Controls.Add(cmbPaginationType);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // PÁGINA INICIAL (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblStartPage = new Label
    {
        Text = "Página Inicial:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblStartPage);

    numStartPage = new NumericUpDown
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = 150,
        Height = controlHeight,
        Value = 1,
        Minimum = 1,
        Maximum = 999999,
        Increment = 1,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numStartPage);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // MÁXIMO DE PÁGINAS (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblMaxPages = new Label
    {
        Text = "Máx Páginas:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblMaxPages);

    numMaxPages = new NumericUpDown
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = 150,
        Height = controlHeight,
        Value = 0,  // 0 = sem limite
        Minimum = 0,
        Maximum = 999999,
        Increment = 1,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numMaxPages);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // NOTA INFORMATIVA (Label)
    // ???????????????????????????????????????????????????????????????
    var lblNote = new Label
    {
        Text = "Máx Páginas = 0 significa sem limite",
        Location = new Point(leftMargin, y + 10),
        Width = controlWidth,
        Height = controlHeight,
        Font = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Italic),
        ForeColor = System.Drawing.SystemColors.GrayText
    };
    tab.Controls.Add(lblNote);
}
```

---

## ?? PROPRIEDADES A ADICIONAR

Adicionar na seção de propriedades privadas (classe):
```csharp
// Pagination Tab Controls
private ComboBox? cmbPaginationType;
private NumericUpDown? numStartPage;
private NumericUpDown? numMaxPages;
```

---

## ?? REFERÊNCIAS

**Arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`  
**Método**: `CreatePaginationTab()`  
**Linhas esperadas**: ~110 linhas

**Documentação**:
- IMPLEMENTATION_GUIDE.md - Passo 6
- CorporateApiSource.cs - Propriedades de paginação
- BLOCO_1.2_RELATORIO.md - Padrão usado em General Tab

---

## ? CHECKLIST ANTES DE COMEÇAR

- [x] BLOCO 1.2 ? concluído
- [x] Projeto compilado sem erros
- [x] Arquivo ApiSourceWizard.cs aberto
- [x] Método CreatePaginationTab() localizado
- [x] Template acima pronto para copiar
- [x] Documentação IMPLEMENTATION_GUIDE.md disponível

---

## ?? ESTIMATIVA

| Etapa | Tempo |
|-------|-------|
| Setup + Labels | 0.15h |
| ComboBox tipo paginação | 0.25h |
| NumericUpDown start page | 0.2h |
| NumericUpDown max pages | 0.2h |
| Label nota informativa | 0.1h |
| Layout + Spacing | 0.15h |
| Compilação + Testes | 0.15h |
| **TOTAL** | **1.2h** |

---

## ?? PRÓXIMO PASSO IMEDIATO

1. **Abrir arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`
2. **Localizar**: Método `CreatePaginationTab()` (linha ~175)
3. **Remover**: `// TODO:` comment
4. **Adicionar**: Código do template acima
5. **Adicionar propriedades**: 3 novas (ComboBox + 2x NumericUpDown)
6. **Compilar**: `dotnet build`
7. **Validar**: 0 erros, 0 warnings

---

## ?? NOTAS IMPORTANTES

### Campos a Guardar para LoadCurrentValues()
```csharp
private ComboBox? cmbPaginationType;      // Para BLOCO 2.1
private NumericUpDown? numStartPage;      // Para BLOCO 2.1
private NumericUpDown? numMaxPages;       // Para BLOCO 2.1
```

### Tipos de Paginação
```
Offset:        Página N (page=1, pageSize=500)
Cursor:        Próximo registro (next_cursor=abc123)
Link-based:    Próximo link no header (Link: <url>; rel="next")
None:          Sem paginação (retorna todos)
```

### Default Values
```
Tipo:           "Offset (padrão)"
Página Inicial:  1
Máx Páginas:     0 (sem limite)
```

---

## ? PADRÃO A SEGUIR

```csharp
// Reusar padrão do BLOCO 1.2 (General Tab)
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
- Verifique: BLOCO_1.2_RELATORIO.md (padrão similar)
- Template: Código acima é 100% copiável

---

## ?? PRÓXIMA META

Após BLOCO 1.3 concluído:
```
? BLOCO 1.2 (Tab General) - 1.5h
? BLOCO 1.3 (Tab Pagination) - 1-1.5h    ? PRÓXIMO
  ?? BLOCO 1.4 (Tab Incremental) - 1-1.5h
      ?? BLOCO 1.5 (Tab Storage) - 1-1.5h
          ?? BLOCO 1.6 (Tab Advanced) - 1.5-2h
?????????????????????????????????????????????????????
Total Bloco 1: ~7-8h (dentro de 8-10h)
```

---

**Status**: ?? PRONTO PARA INICIAR  
**Data**: 2024-01-XX  
**Desenvolvedor**: [Seu Nome]  
**Próximo**: BLOCO 1.3 (Tab Paginação)

