/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Component UI Controller
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Controlador de UI para o componente Corporate API Source.
Implementa IDtsComponentUI para integração com SSIS Designer.

FUNCIONALIDADES:
- Wizard de configuração multi-step
- Validação de propriedades
- Preview de dados (opcional)
- Help integrado

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using QuattoAPIClient.Logging;

// NOTA IMPORTANTE: Para que este código compile corretamente, é necessário:
// 1. Instalar o SQL Server Integration Services (SSIS) 2019 ou posterior
// 2. Ter o Visual Studio com suporte a SSIS
// 3. Referenciar os assemblies SSIS:
//    - Microsoft.SqlServer.DTSPipelineWrap.dll
//    - Microsoft.SqlServer.DTSRuntimeWrap.dll
//    - Microsoft.SqlServer.ManagedDTS.dll
// 4. Esses assemblies devem estar no GAC (Global Assembly Cache)

// Certifique-se de adicionar as referências corretas ao seu projeto:
// - Microsoft.SqlServer.Dts.Pipeline.Design.dll
// - Microsoft.SqlServer.Dts.Pipeline.Wrapper.dll
// - Microsoft.SqlServer.ManagedDTS.dll
// Para projetos .NET Framework, clique com o botão direito em "Referências" > "Adicionar Referência" > "Assemblies" > "Extensões" e selecione os DLLs acima.
// Para projetos .NET Core/.NET 5+, esses assemblies não são suportados nativamente.

// Se você já adicionou as referências e o erro persiste, verifique se o Target Framework do seu projeto é ".NET Framework" (ex: 4.7.2), pois os assemblies SSIS não são compatíveis com .NET Core/.NET 5+.

// Não é necessário alterar o código-fonte, mas garanta que as referências estejam presentes no arquivo .csproj, por exemplo:
//
// <ItemGroup>
//   <Reference Include="Microsoft.SqlServer.Dts.Pipeline.Design" />
//   <Reference Include="Microsoft.SqlServer.Dts.Pipeline.Wrapper" />
//   <Reference Include="Microsoft.SqlServer.ManagedDTS" />
// </ItemGroup>
//
// Se estiver usando pacotes NuGet, procure por "Microsoft.SqlServer.Dts.Design" ou "Microsoft.SqlServer.Dts.Pipeline" (nem sempre disponível via NuGet).

namespace QuattoAPIClient.UI
{
    // Type stubs for SSIS types - replace with actual types once SSIS is properly installed
    public interface IDtsComponentUI
    {
        void Initialize(object dtsComponentMetadata, IServiceProvider serviceProvider);
        bool Edit(IWin32Window parentWindow, object variables, object connections);
        void New(IWin32Window parentWindow);
        void Delete(IWin32Window parentWindow);
        void Help(IWin32Window parentWindow);
    }

    public interface IDTSComponentMetaData100
    {
        // Add actual interface members when SSIS types are available
    }

    public interface Connections
    {
        // Add actual interface members when SSIS types are available
    }

    public interface Variables
    {
        // Add actual interface members when SSIS types are available
    }

    /// <summary>
    /// Mock wizard class - implement actual wizard when SSIS types are available
    /// </summary>
    public class ApiSourceWizard : Form
    {
        public ApiSourceWizard(object metadata, object connections, object variables)
        {
            // Initialize wizard UI here
        }
    }

    /// <summary>
    /// Controlador de UI para Corporate API Source
    /// Implementa a interface IDtsComponentUI para integração com SSIS Designer
    /// </summary>
    public class CorporateApiSourceUI : IDtsComponentUI
    {
        private object? _metadata;  // Should be IDTSComponentMetaData100
        private IServiceProvider? _serviceProvider;
        private object? _connections;  // Should be Connections
        private object? _variables;  // Should be Variables
        private readonly ILogger<CorporateApiSourceUI> _logger;

        /// <summary>
        /// Construtor que inicializa o logger
        /// </summary>
        public CorporateApiSourceUI()
        {
            _logger = LoggerFactory.GetLogger<CorporateApiSourceUI>();
            _logger.LogInformation("CorporateApiSourceUI instância criada");
        }

        /// <summary>
        /// Inicializa o editor de UI com metadados do componente e provedor de serviços
        /// </summary>
        /// <param name="dtsComponentMetadata">Metadados do componente SSIS</param>
        /// <param name="serviceProvider">Provedor de serviços do SSIS Designer</param>
        /// <exception cref="ArgumentNullException">Se dtsComponentMetadata ou serviceProvider forem null</exception>
        public void Initialize(object dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            try
            {
                _logger.LogInformation("Iniciando Initialize com metadata e service provider");
                _metadata = dtsComponentMetadata ?? throw new ArgumentNullException(nameof(dtsComponentMetadata), "Component metadata cannot be null");
                _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider), "Service provider cannot be null");
                _logger.LogSuccess("Initialize completado", "Metadados e provider inicializados");
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogOperationError("Initialize", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado em Initialize: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        /// <summary>
        /// Abre editor de propriedades do componente (wizard de configuração)
        /// </summary>
        /// <param name="parentWindow">Janela pai do modal</param>
        /// <param name="variables">Variáveis SSIS disponíveis</param>
        /// <param name="connections">Connection managers disponíveis</param>
        /// <returns>True se usuário clicou OK, False se cancelou</returns>
        /// <exception cref="InvalidOperationException">Se Initialize não foi chamado antes</exception>
        public bool Edit(IWin32Window parentWindow, object variables, object connections)
        {
            using (var scope = new LogScope(_logger, nameof(Edit)))
            {
                if (_metadata == null)
                {
                    _logger.LogError("Metadata não foi inicializado - Initialize deve ser chamado antes");
                    throw new InvalidOperationException("Component metadata not initialized. Call Initialize method first before editing.");
                }

                try
                {
                    _logger.LogInformation("Abrindo Editor com wizard de configuração");
                    _variables = variables ?? throw new ArgumentNullException(nameof(variables), "Variables collection cannot be null");
                    _connections = connections ?? throw new ArgumentNullException(nameof(connections), "Connections collection cannot be null");

                    using (var wizard = new ApiSourceWizard(_metadata, _connections, _variables))
                    {
                        DialogResult result = wizard.ShowDialog(parentWindow);

                        if (result == DialogResult.OK)
                        {
                            _logger.LogSuccess("Editor", "Configuração salva com sucesso");
                            // Wizard saved changes - trigger metadata refresh
                            // TODO: Call FireComponentMetaDataModifiedEvent on metadata when SSIS types are available
                            // _metadata.FireComponentMetaDataModifiedEvent();
                            return true;
                        }

                        _logger.LogInformation("Usuário cancelou editor de propriedades");
                        return false;
                    }
                }
                catch (ArgumentNullException ex)
                {
                    _logger.LogOperationError("Edit", ex, "Parâmetro nulo: {Parameter}", ex.ParamName);
                    MessageBox.Show(
                        parentWindow,
                        $"Erro: Um parâmetro obrigatório é nulo:\n\n{ex.Message}",
                        "Quatto API Client - Erro de Validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao abrir editor de propriedades: {ErrorMessage}", ex.Message);
                    MessageBox.Show(
                        parentWindow,
                        $"Erro ao abrir editor de propriedades:\n\n{ex.Message}",
                        "Quatto API Client - Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return false;
                }
            }
        }

        /// <summary>
        /// Inicializa novo componente com valores padrão
        /// Chamado quando usuário arrasta o componente para o design surface
        /// </summary>
        /// <param name="parentWindow">Janela pai para diálogos modais</param>
        public void New(IWin32Window parentWindow)
        {
            // Opcional: Mostrar diálogo de boas-vindas ou iniciar wizard de setup
            // Por padrão, o componente está pronto para uso após drag-drop
        }

        /// <summary>
        /// Limpa recursos do componente antes da exclusão
        /// Chamado quando usuário remove o componente do design surface
        /// </summary>
        /// <param name="parentWindow">Janela pai para diálogos modais</param>
        public void Delete(IWin32Window parentWindow)
        {
            // Opcional: Confirmar exclusão ou limpar recursos
            // SSIS gerencia automaticamente a exclusão do componente
        }

        /// <summary>
        /// Exibe ajuda do componente em uma janela modal
        /// Chamado quando usuário clica no botão Help no wizard
        /// </summary>
        /// <param name="parentWindow">Janela pai para o diálogo de ajuda</param>
        public void Help(IWin32Window parentWindow)
        {
            _logger.LogInformation("Exibindo diálogo de ajuda");
            try
            {
                string helpText = @"
═══════════════════════════════════════════════════════════════════
QUATTO CORPORATE API SOURCE - AJUDA RÁPIDA
═══════════════════════════════════════════════════════════════════

CONFIGURAÇÃO BÁSICA:
1. Connection Manager: Selecione ou crie um 'API Connection'
2. BaseUrl: URL base da API (ex: https://api.gladium.com)
3. Endpoint: Path do endpoint (ex: /v1/orders)
4. PageSize: Registros por página (padrão: 500)

AUTENTICAÇÃO:
Configure no Connection Manager:
- Bearer Token (fixo)
- API Key (header customizado)
- OAuth2 Client Credentials (automático)

EXTRAÇÃO INCREMENTAL:
1. EnableIncremental: true
2. WatermarkColumn: campo de data/ID (ex: updatedAt)
3. SourceSystem: nome do sistema (ex: Gladium)
4. Environment: DEV, HML ou PRD

SCHEMA MAPPING:
Defina em JSON como mapear campos da API para colunas:
{
  ""columns"": [
    {""name"": ""order_id"", ""path"": ""$.id"", ""type"": ""DT_WSTR"", ""length"": 50},
    {""name"": ""updated_at"", ""path"": ""$.updatedAt"", ""type"": ""DT_DBTIMESTAMP2""}
  ]
}

TROUBLESHOOTING:
- Componente não aparece: Reinicie Visual Studio
- Erro de autenticação: Verifique tokens no Connection Manager
- JSON não armazenado: Verifique tabela dbo.API_RawPayloads
- Performance lenta: Aumente PageSize ou RateLimitRPM

DOCUMENTAÇÃO COMPLETA:
C:\Dev\QuattoAPIClient\docs\

SUPORTE:
erton.miranda@quatto.com.br

═══════════════════════════════════════════════════════════════════
";

                MessageBox.Show(
                    parentWindow,
                    helpText,
                    "Quatto Corporate API Source - Ajuda",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                _logger.LogSuccess("Help", "Diálogo de ajuda exibido com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exibir ajuda: {ErrorMessage}", ex.Message);
                MessageBox.Show(
                    parentWindow,
                    $"Erro ao exibir ajuda: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}