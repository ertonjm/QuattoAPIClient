# QuattoAPIClient.UI - Guia de Troubleshooting

Guia detalhado para resolver problemas comuns ao compilar e usar o componente de UI.

---

## ?? Índice

1. [Erros de Compilação](#erros-de-compilação)
2. [Erros de Runtime](#erros-de-runtime)
3. [Problemas com SSIS](#problemas-com-ssis)
4. [Problemas com Wizard](#problemas-com-wizard)
5. [FAQ](#faq)

---

## ?? Erros de Compilação

### CS0246: "O nome do tipo ou do namespace ... não pode ser encontrado"

**Erro completo**:
```
CS0246: O nome do tipo ou do namespace "IDtsComponentUI" não pode ser encontrado
```

**Causas possíveis**:
1. ? Assemblies SSIS não estão no GAC
2. ? Referência de projeto não foi adicionada
3. ? Usando .NET Core/.NET 5+ (não suportado)

**Soluções**:

#### Solução 1: Instalar SQL Server Integration Services

```bash
# Verifique se SSIS está instalado
Get-ChildItem "C:\Program Files\Microsoft SQL Server\150\DTS\" -ErrorAction SilentlyContinue

# Se não existir, instale via SQL Server Installer ou
# Download: https://www.microsoft.com/download/details.aspx?id=100303
```

#### Solução 2: Adicionar Referências no .csproj

```xml
<ItemGroup>
  <Reference Include="Microsoft.SqlServer.DTSPipelineWrap" />
  <Reference Include="Microsoft.SqlServer.DTSRuntimeWrap" />
  <Reference Include="Microsoft.SqlServer.ManagedDTS" />
</ItemGroup>
```

#### Solução 3: Verificar Target Framework

```xml
<!-- ? Correto -->
<TargetFramework>net472</TargetFramework>

<!-- ? Incorreto -->
<TargetFramework>net6.0</TargetFramework>
<TargetFramework>net5.0</TargetFramework>
```

---

### CS0234: "O nome de tipo ou namespace 'Dts' não existe"

**Erro completo**:
```
CS0234: O nome de tipo ou namespace "Dts" não existe no namespace "Microsoft.SqlServer"
```

**Causa**: Os namespaces `Microsoft.SqlServer.Dts.*` não existem nos assemblies wrapper.

**Solução**: Use os tipos diretos das classes wrapper (já implementado como stubs):

```csharp
// ? Errado
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
private IDTSComponentMetaData100 _metadata;

// ? Correto (temporário com stubs)
private object _metadata;  // Será IDTSComponentMetaData100 quando SSIS estiver disponível
```

**Quando SSIS Estiver Disponível**:
```csharp
// ? Será possível usar tipos reais
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Runtime;
private IDTSComponentMetaData100 _metadata;
```

---

### CS0579: "Duplicar atributo"

**Erro completo**:
```
CS0579: Duplicar atributo "System.Reflection.AssemblyCompanyAttribute"
```

**Causa**: Atributos sendo gerados automaticamente pelo SDK `.NET` + atributos em `AssemblyInfo.cs`.

**Solução**:
```xml
<!-- Adicione ao PropertyGroup do .csproj -->
<PropertyGroup>
  <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
</PropertyGroup>
```

---

### CS0006: "Arquivo de origem não pode ser encontrado"

**Erro completo**:
```
CS0006: Arquivo de origem "...QuattoAPIClient.Source.dll" não pode ser encontrado
```

**Causa**: Projeto dependente não foi compilado.

**Solução**:
```bash
# Compile o projeto de origem primeiro
cd ../01_Source
dotnet build

# Depois compile o UI
cd ../03_UI
dotnet build
```

**Ou no Visual Studio**:
1. Clique direito na Solution
2. Selecione **Build Order**
3. Certifique-se que `QuattoAPIClient.Source` compila antes de `QuattoAPIClient.UI`

---

## ?? Erros de Runtime

### NullReferenceException em Initialize()

**Erro**:
```
System.NullReferenceException: Object reference not set to an instance of an object.
at QuattoAPIClient.UI.CorporateApiSourceUI.Initialize(Object dtsComponentMetadata, IServiceProvider serviceProvider)
```

**Causa**: Método `Initialize()` não foi chamado antes de `Edit()`.

**Solução**:
```csharp
// ? Correto - sempre chamar Initialize primeiro
var ui = new CorporateApiSourceUI();
ui.Initialize(metadata, serviceProvider);  // Obrigatório
ui.Edit(parentWindow, variables, connections);
```

---

### NotImplementedException em LoadCurrentValues()

**Erro**:
```
System.NotImplementedException: The method or operation is not implemented.
```

**Causa**: Métodos `LoadCurrentValues()` e `SaveValues()` ainda são stubs.

**Solução**: Implemente os métodos:

```csharp
private void LoadCurrentValues()
{
    // TODO: Implement when SSIS types are available
    // Example:
    // IDTSComponentMetaData100 metadata = (IDTSComponentMetaData100)_metadata;
    // cmbConnection.SelectedItem = GetPropertyValue("ConnectionManager");
}

private void SaveValues()
{
    // TODO: Implement when SSIS types are available
    // Example:
    // IDTSComponentMetaData100 metadata = (IDTSComponentMetaData100)_metadata;
    // SetPropertyValue("ConnectionManager", cmbConnection.SelectedItem);
}
```

---

## ?? Problemas com SSIS

### Componente não aparece na SSIS Toolbox

**Causa**: DLL não foi registrada ou não está no caminho correto.

**Solução**:

#### 1. Verificar se a DLL foi gerada
```bash
ls bin/Debug/net472/QuattoAPIClient.UI.dll
# ou no PowerShell
Get-Item "bin\Debug\net472\QuattoAPIClient.UI.dll"
```

#### 2. Registrar manualmente no SSIS Toolbox
1. No SSDT, abra **SSIS > SSIS Toolbox**
2. Clique direito na toolbox > **Choose Toolbox Items...**
3. Clique em **Browse** e selecione a DLL

#### 3. Reiniciar SSDT
```bash
# Feche SSDT completamente
taskkill /IM devenv.exe /F

# Reabra SSDT
```

---

### "Could not load file or assembly"

**Erro**:
```
Could not load file or assembly 'Microsoft.SqlServer.DTSPipelineWrap' or one of its dependencies
```

**Causa**: Assemblies SSIS não estão no GAC ou em um diretório acessível.

**Solução**:

#### Verificar GAC
```bash
# No Windows PowerShell (como administrador)
gacutil -l Microsoft.SqlServer.DTSPipelineWrap
```

#### Instalar no GAC
```bash
# Se tiver a DLL, instale-a
gacutil -i "C:\Path\To\Microsoft.SqlServer.DTSPipelineWrap.dll"
```

#### Usar Fusion Log Viewer
```bash
# Para debug detalhado
fuslogvw.exe
# Inicie binding, compile, verifique logs
```

---

## ?? Problemas com Wizard

### Wizard não abre quando clico em "Edit"

**Sintoma**: Clique direito no componente > Edit, nada acontece.

**Causa**: Exceção sendo silenciada no try-catch.

**Solução**:

#### 1. Habilitar Output Debug
```csharp
// Em Edit()
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"Erro: {ex}");
    MessageBox.Show(
        parentWindow,
        $"Erro ao abrir editor de propriedades:\n\n{ex.Message}\n\n{ex.StackTrace}",
        "Erro Detalhado",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
}
```

#### 2. Verificar Event Viewer
```bash
# Windows Event Viewer
eventvwr.msc
# Procure por erros em Windows Logs > Application
```

#### 3. Adicionar Debug Breakpoints
```csharp
public bool Edit(IWin32Window parentWindow, object variables, object connections)
{
    System.Diagnostics.Debugger.Break();  // Pausa aqui
    // ... resto do código
}
```

---

### Campos não aparecem no Wizard

**Causa**: FormBuilder ainda não foi implementado.

**Solução**: Use a versão parcial do formulário:

```csharp
// Forms/ApiSourceWizard.Designer.cs
// Adicione controles via Designer visual ou via código:

private void InitializeComponent()
{
    // Tabs
    tabControl = new TabControl { Dock = DockStyle.Fill };
    
    // General Tab
    var tabGeneral = new TabPage { Text = "Geral" };
    var lblConnection = new Label { Text = "Conexão:" };
    var cmbConnection = new ComboBox { Width = 300 };
    
    tabGeneral.Controls.Add(lblConnection);
    tabGeneral.Controls.Add(cmbConnection);
    
    tabControl.TabPages.Add(tabGeneral);
    this.Controls.Add(tabControl);
}
```

---

### Validação não funciona

**Sintoma**: Consigo salvar valores inválidos.

**Causa**: Métodos de validação não estão implementados.

**Solução**:
```csharp
private bool ValidateProperties()
{
    // Validar URL
    if (string.IsNullOrEmpty(txtBaseUrl.Text) || !Uri.IsWellFormedUriString(txtBaseUrl.Text, UriKind.Absolute))
    {
        MessageBox.Show("Base URL inválida");
        return false;
    }
    
    // Validar PageSize
    if (!int.TryParse(numPageSize.Value.ToString(), out int pageSize) || pageSize <= 0)
    {
        MessageBox.Show("Page Size deve ser um número positivo");
        return false;
    }
    
    return true;
}

private void SaveValues()
{
    if (!ValidateProperties())
        return;
        
    // Salvar propriedades
}
```

---

## ? FAQ

### P: Preciso instalar SQL Server completo?

**R**: Não obrigatoriamente. Você pode instalar apenas:
- SQL Server Express + SSIS (gratuito)
- SQL Server Developer + SSIS (gratuito para desenvolvimento)
- Visual Studio Community + SQL Server Data Tools (SSDT)

### P: Posso usar .NET 6 ou superior?

**R**: Não. Os assemblies SSIS só são suportados em .NET Framework 4.7.2+. Planos futuros incluem suporte a .NET Core via wrappers nativos.

### P: Como faço para testar a UI sem SSIS instalado?

**R**: Você pode:
1. Usar stubs para tipos SSIS (implementado)
2. Usar unit tests com mocks
3. Testar formulário Windows Forms isoladamente

### P: Por que alguns métodos estão com "TODO"?

**R**: Porque requerem tipos SSIS reais que não estavam disponíveis durante desenvolvimento. Implemente quando tiver SSIS instalado.

### P: Posso distribuir a DLL para outros usuários?

**R**: Sim, contanto que:
1. Eles tenham SQL Server 2019+ com SSIS instalado
2. Eles tenham .NET Framework 4.7.2+
3. A DLL seja copiada para `%ProgramFiles%\Microsoft SQL Server\...\DTS\PipelineComponents\`

### P: Como contribuir melhorias?

**R**: 
1. Fork do repositório
2. Crie uma branch: `feature/nome-da-feature`
3. Commit das mudanças
4. Push e envie Pull Request

---

## ?? Ferramentas Úteis

| Ferramenta | Uso | Download |
|-----------|-----|----------|
| **Fusion Log Viewer** | Debug de assembly binding | Incluído no Windows SDK |
| **Global Assembly Cache Viewer** | Gerenciar assemblies no GAC | `gacutil.exe` |
| **CFF Explorer** | Inspecionar estrutura DLL | https://ntcore.com/?page_id=345 |
| **Dependency Walker** | Verificar dependências | http://www.dependencywalker.com/ |

---

## ?? Contatos & Recursos

- ?? **Desenvolvedor**: Erton Miranda
- ?? **Email**: erton.miranda@quatto.com.br
- ?? **Documentação SSIS**: https://docs.microsoft.com/sql/integration-services/
- ?? **Microsoft Learn - SSIS**: https://learn.microsoft.com/sql/integration-services/

---

## ?? Histórico de Atualizações

| Data | Versão | Mudança |
|------|--------|---------|
| 2024-01-XX | 1.0.0 | Criação inicial com tipos stub SSIS |

