# Quatto API Client v1.0

Componente customizado para SSIS — Consumo avançado de APIs REST

***

## 🚀 Visão Geral

O **Quatto API Client v1.0** é um componente para SQL Server Integration Services (SSIS) que permite consumir APIs REST com recursos avançados, ideal para projetos de Data Warehouse corporativos.

### **Principais recursos**

*   Paginação automática
*   Retry com backoff exponencial
*   Autenticação OAuth2, Bearer, ApiKey
*   Extração incremental (watermark)
*   Armazenamento de JSON bruto (auditoria)
*   Rate limiting configurável
*   Telemetria detalhada

***

## 🗂 Estrutura do Projeto

    C:\Dev\QuattoAPIClient\
    │
    ├─ QuattoAPIClient.sln
    ├─ src/
    │  ├─ 01_Source/
    │  │  ├─ QuattoAPIClient.Source.csproj
    │  │  ├─ Components/
    │  │  │  └─ CorporateApiSource.cs
    │  │  ├─ Helpers/
    │  │  │  ├─ HttpHelper.cs
    │  │  │  ├─ PaginationEngine.cs
    │  │  │  ├─ WatermarkManager.cs
    │  │  │  ├─ RawStorageManager.cs
    │  │  │  └─ SchemaMapper.cs
    │  │  └─ Properties/
    │  │     └─ AssemblyInfo.cs
    │  ├─ 02_ConnectionManager/
    │  │  ├─ QuattoAPIClient.ConnectionManager.csproj
    │  │  ├─ Connection/
    │  │  │  ├─ ApiConnectionManager.cs
    │  │  │  ├─ OAuth2TokenManager.cs
    │  │  │  └─ TokenRefreshHandler.cs
    │  │  └─ Properties/
    │  │     └─ AssemblyInfo.cs
    │  └─ 03_UI/
    │     ├─ QuattoAPIClient.UI.csproj
    │     ├─ CorporateApiSourceUI.cs
    │     ├─ Forms/
    │     │  ├─ ApiSourceWizard.cs
    │     │  └─ ApiSourceWizard.Designer.cs
    │     └─ Properties/
    │        └─ AssemblyInfo.cs
    ├─ database/
    │  ├─ 01_Tables.sql
    │  ├─ 02_StoredProcedures.sql
    │  ├─ 03_Dashboard_Queries.sql
    │  └─ 04_TestData.sql
    ├─ deployment/
    │  ├─ 01_Deploy_Database.ps1
    │  ├─ 02_Deploy_Component.ps1
    │  ├─ 03_Uninstall_Component.ps1
    │  └─ 04_Validate_Installation.ps1
    ├─ examples/
    │  ├─ SchemaMapping_Gladium.json
    │  ├─ SchemaMapping_PortalSESC.json
    │  ├─ SSISDB_Parameters.json
    │  └─ Sample_Package_Structure.txt
    └─ docs/
       ├─ 01_INSTALLATION.md
       ├─ 02_CONFIGURATION.md
       ├─ 03_USAGE.md
       ├─ 04_TROUBLESHOOTING.md
       └─ 05_API_REFERENCE.md

***

## 📋 Instalação e Uso

1.  **Criar estrutura de pastas** conforme acima.
2.  **Copiar arquivos .cs.txt da pasta SourceCode/** e remover a extensão `.txt`.
3.  **Renomear arquivos \_csproj.txt para .csproj**.
4.  **Executar scripts SQL** da pasta database/ na ordem (01, 02, 03, 04) e ajustar o nome do banco para SESCDF\_DW.
5.  **Compilar o projeto** no Visual Studio 2019+ (restaurar pacotes NuGet, Build Solution).
6.  **Deploy no SSIS**: execute `deployment/02_Deploy_Component.ps1` como administrador e reinicie o Visual Studio (SSDT).
7.  **Criar o primeiro pacote SSIS** usando o exemplo em `examples/Sample_Package_Structure.txt`, configurar o Connection Manager e adicionar o componente no Data Flow.

***

## ⚙️ Configuração do ambiente de desenvolvimento

Pré-requisitos mínimos:

- Windows 10/11 ou Windows Server compatível
- Visual Studio 2019 ou superior com SSDT/Integration Services
- .NET Framework 4.7.2 Developer Pack
- Assemblies SSIS (`Microsoft.SqlServer.ManagedDTS.dll`) correspondentes à versão do SQL Server
- .NET SDK (dotnet CLI) opcional para builds via linha de comando

Fluxo de configuração:

1. Instale o .NET Framework 4.7.2 Developer Pack.
2. Instale a extensão SSIS/SSDT no Visual Studio para registrar os assemblies SSIS.
3. Verifique a existência de `Microsoft.SqlServer.ManagedDTS.dll` em uma das pastas abaixo e ajuste `SqlServerVersion` no `.csproj` conforme desejado:
   - `C:\Program Files\Microsoft SQL Server\140\DTS\Binn\`
   - `C:\Program Files\Microsoft SQL Server\150\DTS\Binn\`
   - `C:\Program Files\Microsoft SQL Server\160\DTS\Binn\`
4. Restaure pacotes NuGet e execute build:
   - `dotnet restore src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj`
   - `dotnet build src/02_ConnectionManager/QuattoAPIClient.ConnectionManager.csproj -c Release`
5. Se ocorrer duplicação de atributos de assembly (CS0579), defina `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` no `.csproj` ou remova/ajuste arquivos `AssemblyInfo.cs` conflitantes.
6. Atualize dependências sensíveis conforme política de segurança (ex.: `System.Text.Json` com aviso de vulnerabilidade).

Dicas:

- Para desenvolvimento sem SSIS instalado, utilize stubs/mocks ou builds condicionais que evitem referências diretas a `Microsoft.SqlServer.ManagedDTS`.
- Ajuste `SqlServerVersion` no `.csproj` para selecionar automaticamente o hint path correto.

***

## 📚 Documentação

A documentação completa está disponível na pasta `docs/`, incluindo:

*   Instalação
*   Configuração
*   Uso
*   Troubleshooting
*   Referência de API

***

## 🆘 Suporte

Problemas comuns:

*   Erro de compilação: verifique referências SSIS nos arquivos .csproj
*   Componente não aparece: execute o script de deploy como administrador
*   Erro de runtime: confira se os scripts SQL foram executados corretamente

Contato interno:  
Erton Miranda <erton.miranda@quatto.com.br>

***

## 📊 Estatísticas do Projeto

*   Total de arquivos: 32
*   Linhas de código C#: \~2.500
*   Linhas de SQL: \~1.300
*   Linhas de PowerShell: \~750
*   Linhas de documentação: \~2.000
*   Total geral: \~6.550 linhas

Versão: 1.0.0  
Data: Fevereiro 2026  
Projeto: SESC-DF Data Warehouse  
Empresa: Quatto Consultoria

***

## ✔️ Observações Finais

*   Siga rigorosamente a estrutura e o mapeamento de arquivos para garantir compatibilidade e facilidade de manutenção.
*   Consulte sempre a documentação oficial antes de abrir chamados de suporte.
*   Atualize este README.md conforme evoluções do projeto e novas versões do componente.

***


