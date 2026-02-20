# ?? BLOCO 1.5 - PRÓXIMO: Tab "Storage"

**Status BLOCO 1.4**: ? CONCLUÍDO  
**Próximo**: BLOCO 1.5  
**Tempo Estimado**: 1-1.5 horas  
**Prioridade**: ?? CRÍTICA  

---

## ?? O QUE SERÁ FEITO

### Tab "Storage" (Storage Tab)
Quarta aba do wizard com 4 controles para configurar armazenamento de JSON bruto:

```
Forma Visual Esperada:
????????????????????????????????????????????
? Geral | Paginação | Incremental | ...    ?
????????????????????????????????????????????
?                                          ?
? Modo Armazenamento: [? None              ?
?                                          ?
? Alvo Armazenamento: [\\server\folder    ?
?                                          ?
? ? Compactar JSON                        ?
?                                          ?
? ? Hash JSON                             ?
?                                          ?
????????????????????????????????????????????
```

---

## ?? CONTROLES A IMPLEMENTAR

### 1?? ComboBox - Modo Armazenamento
```csharp
Label: "Modo Armazenamento:"
Control: ComboBox
  ?? Width: 300px
  ?? Height: 24px
  ?? DropDownStyle: DropDownList
  ?? Items: [
       "None",
       "SqlVarbinary",
       "FileSystem"
     ]
  
Propósito: Estratégia de armazenamento de JSON bruto
Default: "None" (nenhum armazenamento)
Valores:
  - None: Não armazena JSON bruto
  - SqlVarbinary: Armazena no SQL em coluna varbinary
  - FileSystem: Armazena em arquivos no sistema
```

### 2?? TextBox - Alvo Armazenamento
```csharp
Label: "Alvo Armazenamento:"
Control: TextBox
  ?? Width: 300px
  ?? Height: 24px
  ?? Text: ""
  ?? Multiline: False
  ?? ReadOnly: False
  
Propósito: Caminho ou coluna de armazenamento
Exemplos: 
  - "\\server\folder\data" (para FileSystem)
  - "RawData_Column" (para SqlVarbinary)
Padrão: Vazio (não aplicável)
```

### 3?? CheckBox - Compactar JSON
```csharp
Label: "Compactar JSON"
Control: CheckBox
  ?? Location: (10, y)
  ?? Width: 200px
  ?? Height: 24px
  ?? Checked: False (default)
  ?? AutoSize: False
  
Propósito: Remover espaços em branco do JSON
Default: Unchecked (não compactado)
Uso: Reduz tamanho do arquivo armazenado
```

### 4?? CheckBox - Hash JSON
```csharp
Label: "Hash JSON"
Control: CheckBox
  ?? Location: (10, y)
  ?? Width: 200px
  ?? Height: 24px
  ?? Checked: False (default)
  ?? AutoSize: False
  
Propósito: Calcular SHA256 do JSON para validação
Default: Unchecked (sem hash)
Uso: Detectar alterações no JSON
```

---

## ?? LAYOUT DO CÓDIGO

### Estrutura Esperada:
```csharp
private void CreateStorageTab()
{
    var tab = new TabPage("Armazenamento");
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

    // 1. Adicionar Label + ComboBox (Modo Armazenamento)
    // 2. Adicionar Label + TextBox (Alvo Armazenamento)
    // 3. Adicionar CheckBox (Compactar JSON)
    // 4. Adicionar CheckBox (Hash JSON)
}
```

---

## ?? CÓDIGO INICIAL (COPIE E ADAPTE)

### Template Completo:
```csharp
private void CreateStorageTab()
{
    var tab = new TabPage("Armazenamento");
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
    // MODO ARMAZENAMENTO (ComboBox)
    // ???????????????????????????????????????????????????????????????
    var lblRawStoreMode = new Label
    {
        Text = "Modo Armazenamento:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblRawStoreMode);

    cmbRawStoreMode = new ComboBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = controlWidth,
        Height = controlHeight,
        DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
    };
    cmbRawStoreMode.Items.AddRange(new object[]
    {
        "None",
        "SqlVarbinary",
        "FileSystem"
    });
    cmbRawStoreMode.SelectedIndex = 0;  // Default: None
    tab.Controls.Add(cmbRawStoreMode);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // ALVO ARMAZENAMENTO (TextBox)
    // ???????????????????????????????????????????????????????????????
    var lblRawStoreTarget = new Label
    {
        Text = "Alvo Armazenamento:",
        Location = new Point(leftMargin, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblRawStoreTarget);

    txtRawStoreTarget = new TextBox
    {
        Location = new Point(leftMargin + labelWidth + labelControlGap, y),
        Width = controlWidth,
        Height = controlHeight,
        Text = "",
        Multiline = false
    };
    tab.Controls.Add(txtRawStoreTarget);
    y += rowHeight + 10;  // Extra space before checkboxes

    // ???????????????????????????????????????????????????????????????
    // COMPACTAR JSON (CheckBox)
    // ???????????????????????????????????????????????????????????????
    chkCompressRawJson = new CheckBox
    {
        Text = "Compactar JSON",
        Location = new Point(leftMargin, y),
        Width = 200,
        Height = controlHeight,
        Checked = false,
        AutoSize = false
    };
    tab.Controls.Add(chkCompressRawJson);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // HASH JSON (CheckBox)
    // ???????????????????????????????????????????????????????????????
    chkHashRawJson = new CheckBox
    {
        Text = "Hash JSON",
        Location = new Point(leftMargin, y),
        Width = 200,
        Height = controlHeight,
        Checked = false,
        AutoSize = false
    };
    tab.Controls.Add(chkHashRawJson);
}
```

---

## ?? PROPRIEDADES A ADICIONAR

Adicionar na seção de propriedades privadas (classe):
```csharp
// Storage Tab Controls
private ComboBox? cmbRawStoreMode;
private TextBox? txtRawStoreTarget;
private CheckBox? chkCompressRawJson;
private CheckBox? chkHashRawJson;
```

---

## ?? REFERÊNCIAS

**Arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`  
**Método**: `CreateStorageTab()`  
**Linhas esperadas**: ~90 linhas

**Documentação**:
- IMPLEMENTATION_GUIDE.md - Passo 6
- CorporateApiSource.cs - Propriedades de storage
- BLOCO_1.4_RELATORIO.md - Padrão de implementação

---

## ? CHECKLIST ANTES DE COMEÇAR

- [x] BLOCO 1.4 ? concluído
- [x] Projeto compilado sem erros
- [x] Arquivo ApiSourceWizard.cs aberto
- [x] Método CreateStorageTab() localizado
- [x] Template acima pronto para copiar
- [x] Documentação IMPLEMENTATION_GUIDE.md disponível

---

## ?? ESTIMATIVA

| Etapa | Tempo |
|-------|-------|
| Setup + Labels | 0.15h |
| ComboBox modo armazenamento | 0.2h |
| TextBox alvo | 0.15h |
| CheckBox compactação | 0.1h |
| CheckBox hash | 0.1h |
| Layout + Spacing | 0.15h |
| Compilação + Testes | 0.15h |
| **TOTAL** | **1.0h** |

---

## ?? PRÓXIMO PASSO IMEDIATO

1. **Abrir arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`
2. **Localizar**: Método `CreateStorageTab()` (linha ~400)
3. **Remover**: `// TODO:` comment
4. **Adicionar**: Código do template acima
5. **Adicionar propriedades**: 4 novas (ComboBox + TextBox + 2x CheckBox)
6. **Compilar**: `dotnet build`
7. **Validar**: 0 erros, 0 warnings

---

## ?? NOTAS IMPORTANTES

### Campos a Guardar para LoadCurrentValues()
```csharp
private ComboBox? cmbRawStoreMode;      // Para BLOCO 2.1
private TextBox? txtRawStoreTarget;     // Para BLOCO 2.1
private CheckBox? chkCompressRawJson;   // Para BLOCO 2.1
private CheckBox? chkHashRawJson;       // Para BLOCO 2.1
```

### Modos de Armazenamento
```
None          - Sem armazenamento (padrão)
SqlVarbinary  - Coluna SQL com dados binários
FileSystem    - Arquivos no servidor de arquivos
```

### Extra Space Antes de CheckBoxes
```
Após TextBox: y += rowHeight + 10
Para respiração visual antes dos checkboxes
```

---

## ? PADRÃO A SEGUIR

```csharp
// Consolidar padrão dos BLOCOs anteriores
// ???????????????????????????????????????????????????????????????
// NOME DO CAMPO (Tipo de Controle)
// ???????????????????????????????????????????????????????????????
var lblName = new Label { ... };
tab.Controls.Add(lblName);

var cntrlName = new ControlType { ... };
tab.Controls.Add(cntrlName);
y += rowHeight;  // ou y += rowHeight + 10 se antes de checkboxes
```

---

## ?? SUPORTE

Dúvidas durante execução?
- Consulte: IMPLEMENTATION_GUIDE.md Passo 6
- Verifique: BLOCO_1.4_RELATORIO.md (padrão similar)
- Template: Código acima é 100% copiável

---

## ?? PRÓXIMA META

Após BLOCO 1.5 concluído:
```
? BLOCO 1.2 (Tab General) - 1.5h
? BLOCO 1.3 (Tab Pagination) - 1.2h
? BLOCO 1.4 (Tab Incremental) - 1.0h
? BLOCO 1.5 (Tab Storage) - 1.0h    ? PRÓXIMO
  ?? BLOCO 1.6 (Tab Advanced) - 1.8h
?????????????????????????????????????????????????????
Total Bloco 1: ~7.5h (dentro de 8-10h) ?
```

---

**Status**: ?? PRONTO PARA INICIAR  
**Data**: 2024-01-XX  
**Desenvolvedor**: [Seu Nome]  
**Próximo**: BLOCO 1.5 (Tab Storage)

