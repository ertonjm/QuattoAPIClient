# ?? BLOCO 1.2 - PRÓXIMO PASSO: Tab "Geral"

**Status BLOCO 1.1**: ? CONCLUÍDO  
**Próximo**: BLOCO 1.2  
**Tempo Estimado**: 1.5-2 horas  
**Prioridade**: ?? CRÍTICA  

---

## ?? O QUE SERÁ FEITO

### Tab "Geral" (General Tab)
Primeira aba do wizard com 4 controles essenciais:

```
Forma Visual Esperada:
????????????????????????????????????????????
? Geral | Paginação | Incremental | ...    ?
????????????????????????????????????????????
?                                          ?
? Conexão:        [? Connection Manager   ?
?                                          ?
? Base URL:       [https://api.example.com?
?                                          ?
? Endpoint:       [/v1/orders             ?
?                                          ?
? Tamanho Página: [500            ]       ?
?                                          ?
????????????????????????????????????????????
```

---

## ?? CONTROLES A IMPLEMENTAR

### 1?? ComboBox - Conexão API
```csharp
Label: "Conexão:"
Control: ComboBox
  ?? Width: 300px
  ?? Height: 24px
  ?? DropDownStyle: DropDownList
  ?? Items: [Carregadas do Connection Manager]
  
Propósito: Selecionar qual Connection Manager usar
Default: Primeiro item da lista
```

### 2?? TextBox - Base URL
```csharp
Label: "Base URL:"
Control: TextBox
  ?? Width: 300px
  ?? Height: 24px
  ?? Text: "https://api.exemplo.com"
  ?? Multiline: False
  ?? ReadOnly: False
  
Propósito: URL base da API
Exemplo: https://api.gladium.com
Validação: Deve começar com https://
```

### 3?? TextBox - Endpoint
```csharp
Label: "Endpoint:"
Control: TextBox
  ?? Width: 300px
  ?? Height: 24px
  ?? Text: "/v1/orders"
  ?? Multiline: False
  ?? ReadOnly: False
  
Propósito: Path do endpoint
Exemplo: /v1/orders, /v2/customers
Validação: Deve começar com /
```

### 4?? NumericUpDown - Page Size
```csharp
Label: "Tamanho Página:"
Control: NumericUpDown
  ?? Width: 150px
  ?? Height: 24px
  ?? Value: 500 (padrão)
  ?? Minimum: 1
  ?? Maximum: 10000
  ?? Increment: 100
  ?? DecimalPlaces: 0
  
Propósito: Registros por página
Padrão: 500
Mín/Máx: 1-10000
```

---

## ?? LAYOUT DO CÓDIGO

### Estrutura Esperada:
```csharp
private void CreateGeneralTab()
{
    var tab = new TabPage("Geral");
    tab.Padding = new Padding(10);
    tabControl!.TabPages.Add(tab);

    // Definir constantes de layout
    int y = 10;
    const int labelWidth = 150;
    const int controlWidth = 300;
    const int controlHeight = 24;
    const int rowHeight = 35;

    // 1. Adicionar Label + ComboBox (Conexão)
    // 2. Adicionar Label + TextBox (Base URL)
    // 3. Adicionar Label + TextBox (Endpoint)
    // 4. Adicionar Label + NumericUpDown (PageSize)
}
```

---

## ?? CÓDIGO INICIAL (COPIE E ADAPTE)

### Template Básico:
```csharp
private void CreateGeneralTab()
{
    var tab = new TabPage("Geral");
    tab.Padding = new Padding(10);
    tabControl!.TabPages.Add(tab);

    int y = 10;
    const int labelWidth = 150;
    const int controlWidth = 300;
    const int controlHeight = 24;
    const int rowHeight = 35;

    // ???????????????????????????????????????????????????????????????
    // CONEXÃO (ComboBox)
    // ???????????????????????????????????????????????????????????????
    var lblConnection = new Label
    {
        Text = "Conexão:",
        Location = new Point(10, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblConnection);

    var cmbConnection = new ComboBox
    {
        Location = new Point(10 + labelWidth + 10, y),
        Width = controlWidth,
        Height = controlHeight,
        DropDownStyle = ComboBoxDropDownStyle.DropDownList
        // TODO: Carregar itens do Connection Manager
    };
    tab.Controls.Add(cmbConnection);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // BASE URL (TextBox)
    // ???????????????????????????????????????????????????????????????
    var lblBaseUrl = new Label
    {
        Text = "Base URL:",
        Location = new Point(10, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblBaseUrl);

    var txtBaseUrl = new TextBox
    {
        Location = new Point(10 + labelWidth + 10, y),
        Width = controlWidth,
        Height = controlHeight,
        Text = "https://"
    };
    tab.Controls.Add(txtBaseUrl);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // ENDPOINT (TextBox)
    // ???????????????????????????????????????????????????????????????
    var lblEndpoint = new Label
    {
        Text = "Endpoint:",
        Location = new Point(10, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblEndpoint);

    var txtEndpoint = new TextBox
    {
        Location = new Point(10 + labelWidth + 10, y),
        Width = controlWidth,
        Height = controlHeight,
        Text = "/v1/"
    };
    tab.Controls.Add(txtEndpoint);
    y += rowHeight;

    // ???????????????????????????????????????????????????????????????
    // PAGE SIZE (NumericUpDown)
    // ???????????????????????????????????????????????????????????????
    var lblPageSize = new Label
    {
        Text = "Tamanho Página:",
        Location = new Point(10, y),
        Width = labelWidth,
        Height = controlHeight
    };
    tab.Controls.Add(lblPageSize);

    var numPageSize = new NumericUpDown
    {
        Location = new Point(10 + labelWidth + 10, y),
        Width = 150,
        Height = controlHeight,
        Value = 500,
        Minimum = 1,
        Maximum = 10000,
        Increment = 100,
        DecimalPlaces = 0
    };
    tab.Controls.Add(numPageSize);
}
```

---

## ?? REFERÊNCIAS

**Arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`  
**Método**: `CreateGeneralTab()`  
**Linhas esperadas**: ~120 linhas

**Documentação**:
- ? IMPLEMENTATION_GUIDE.md - Passo 6 (CreateGeneralTab)
- ? CorporateApiSourceUI.cs - Propriedades esperadas
- ? README.md (módulo UI) - Configuração esperada

---

## ? CHECKLIST ANTES DE COMEÇAR

- [ ] BLOCO 1.1 ? concluído
- [ ] Projeto compilado sem erros
- [ ] Arquivo ApiSourceWizard.cs aberto
- [ ] Método CreateGeneralTab() localizado
- [ ] Template acima copiado
- [ ] Documentação IMPLEMENTATION_GUIDE.md disponível

---

## ?? ESTIMATIVA

| Etapa | Tempo |
|-------|-------|
| Setup + Labels | 0.25h |
| ComboBox | 0.25h |
| TextBox Base URL | 0.25h |
| TextBox Endpoint | 0.25h |
| NumericUpDown | 0.25h |
| Layout + Spacing | 0.25h |
| Compilação + Testes | 0.25h |
| **TOTAL** | **1.75h** |

---

## ?? PRÓXIMO PASSO IMEDIATO

1. **Abrir arquivo**: `src/03_UI/Forms/ApiSourceWizard.cs`
2. **Localizar**: Método `CreateGeneralTab()` (linha ~120)
3. **Remover**: `// TODO:` comment
4. **Adicionar**: Código do template acima
5. **Compilar**: `dotnet build`
6. **Validar**: 0 erros, 0 warnings

---

## ?? NOTAS IMPORTANTES

### Campos a Guardar para LoadCurrentValues()
```csharp
private ComboBox? cmbConnection;  // Para BLOCO 2.1
private TextBox? txtBaseUrl;      // Para BLOCO 2.1
private TextBox? txtEndpoint;     // Para BLOCO 2.1
private NumericUpDown? numPageSize; // Para BLOCO 2.1
```

### Espaçamento
```
Y Position Pattern:
y = 10              (início)
y += 35             (próximo controle)
y += 35             (próximo controle)
y += 35             (próximo controle)
```

### Alinhamento
```
Labels: X = 10, Width = 150
Controls: X = 170 (10 + 150 + 10), Width = 300
```

---

## ? QUALIDADE ESPERADA

```
? Código:
  ?? Organizado em seções (???)
  ?? Comentários descritivos
  ?? Espaçamento consistente
  
? Layout:
  ?? Labels alinhados verticalmente
  ?? Controles alinhados verticalmente
  ?? Padding de 10px
  
? Compilação:
  ?? 0 erros
  ?? 0 warnings
  
? Funcionalidade:
  ?? Formulário abre sem erro
  ?? Tab "Geral" é visível
  ?? Todos os controles interativos
```

---

## ?? SUPORTE

Dúvidas durante execução?
- Consulte: IMPLEMENTATION_GUIDE.md Passo 6
- Verifique: TROUBLESHOOTING.md (seção UI)
- Template: Código acima é copiável

---

## ?? PRÓXIMA META

Após BLOCO 1.2 concluído:
```
? BLOCO 1.2 (Tab General) - 1.75h
  ?? BLOCO 1.3 (Tab Pagination) - 1.5h
      ?? BLOCO 1.4 (Tab Incremental) - 1.5h
          ?? BLOCO 1.5 (Tab Storage) - 1.5h
              ?? BLOCO 1.6 (Tab Advanced) - 2h
?????????????????????????????????????????????
Total Bloco 1: ~8.25h (dentro do estimado 8-10h)
```

---

**Status**: ?? PRONTO PARA INICIAR  
**Data**: 2024-01-XX  
**Desenvolvedor**: [Seu Nome]  
**Documento**: GUIA_BLOCO_1.2.md

