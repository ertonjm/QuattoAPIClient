/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Exemplos de Uso do Logger
═══════════════════════════════════════════════════════════════════
*/

using System;
using Microsoft.Extensions.Logging;
using QuattoAPIClient.Logging;

namespace QuattoAPIClient.Examples
{
    /// <summary>
    /// Exemplos de como usar o sistema de logging estruturado
    /// </summary>
    public class LoggingExamples
    {
        /// <example>
        /// // EXEMPLO 1: Log básico
        /// var logger = LoggerFactory.GetLogger<MyClass>();
        /// logger.LogInformation("Operação iniciada");
        /// logger.LogWarning("Aviso: recurso pode estar indisponível");
        /// logger.LogError(ex, "Erro ao processar");
        /// </example>
        public static void Example1_BasicLogging()
        {
            // Obter logger para a classe
            var logger = global::QuattoAPIClient.Logging.LoggerFactory.GetLogger<LoggingExamples>();

            // Logs simples
            logger.LogInformation("Aplicação iniciada");
            logger.LogWarning("Configuração incompleta");
            logger.LogError("Falha ao conectar ao servidor");
        }

        /// <example>
        /// // EXEMPLO 2: Log estruturado com contexto
        /// logger.LogInformation("User {UserId} logged in from {IpAddress}",
        ///     userId, ipAddress);
        /// logger.LogError(ex, "Failed to process order {OrderId}: {Message}",
        ///     orderId, ex.Message);
        /// </example>
        public static void Example2_StructuredLogging()
        {
            var logger = global::QuattoAPIClient.Logging.LoggerFactory.GetLogger<LoggingExamples>();

            // Logs com contexto estruturado
            int userId = 123;
            string ipAddress = "192.168.1.100";
            logger.LogInformation("User {UserId} logged in from {IpAddress}",
                userId, ipAddress);

            // Log de erro com contexto
            int orderId = 456;
            try
            {
                throw new InvalidOperationException("Order not found");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process order {OrderId}: {Message}",
                    orderId, ex.Message);
            }
        }

        /// <example>
        /// // EXEMPLO 3: Usar LogScope para correlacionar operações
        /// using (var scope = new LogScope(logger, "ProcessOrder", correlationId))
        /// {
        ///     // Todos os logs aqui conterão o contexto do scope
        ///     logger.LogInformation("Processing order");
        ///     // ... mais operações
        ///     logger.LogSuccess("ProcessOrder", "Order processed successfully");
        /// }
        /// </example>
        public static void Example3_LogScope()
        {
            var logger = global::QuattoAPIClient.Logging.LoggerFactory.GetLogger<LoggingExamples>();
            string correlationId = System.Guid.NewGuid().ToString();

            // Usar scope para correlacionar logs relacionados
            using (var scope = new LogScope(logger, "ProcessOrder", correlationId))
            {
                logger.LogInformation("Iniciando processamento do pedido");
                // ... lógica de processamento
                logger.LogSuccess("ProcessOrder", "Pedido processado com sucesso");
            }
        }

        /// <example>
        /// // EXEMPLO 4: Usar extensões de logging
        /// logger.LogSuccess("SaveOperation", "Dados salvos com sucesso");
        /// logger.LogOperationError("LoadData", ex, "Falha ao carregar dados");
        /// logger.LogWarning("Tentativas de conexão excedidas");
        /// </example>
        public static void Example4_LoggerExtensions()
        {
            var logger = global::QuattoAPIClient.Logging.LoggerFactory.GetLogger<LoggingExamples>();

            // Usar extensões de logging
            try
            {
                // ... operação
                logger.LogSuccess("SaveOperation", "Dados salvos com sucesso");
            }
            catch (Exception ex)
            {
                logger.LogOperationError("SaveOperation", ex);
            }

            // Log de aviso
            logger.LogWarning("Tentativas de conexão excedidas");
        }

        /// <example>
        /// // EXEMPLO 5: Níveis de log
        /// logger.LogTrace("Valor detalhado: {Value}", debugValue); // Menos comum
        /// logger.LogDebug("Debug info: {Info}", debugInfo);
        /// logger.LogInformation("Operação normal");
        /// logger.LogWarning("Possível problema");
        /// logger.LogError(ex, "Erro durante operação");
        /// logger.LogCritical(ex, "Falha crítica do sistema");
        /// </example>
        public static void Example5_LogLevels()
        {
            var logger = global::QuattoAPIClient.Logging.LoggerFactory.GetLogger<LoggingExamples>();

            // Diferentes níveis de log
            logger.LogDebug("Debug: Processando lista com {Count} itens", 42);
            logger.LogInformation("Operação de processamento iniciada");
            logger.LogWarning("CPU em alto uso: {Usage}%", 85);

            try
            {
                throw new Exception("Erro crítico");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro: Falha ao processar dados");
                logger.LogCritical(ex, "Falha crítica: Sistema pode estar instável");
            }
        }

        /// <example>
        /// // EXEMPLO 6: Em métodos de UI
        /// public bool Edit(IWin32Window parentWindow, object variables, object connections)
        /// {
        ///     using (var scope = new LogScope(_logger, nameof(Edit)))
        ///     {
        ///         try
        ///         {
        ///             _logger.LogInformation("Abrindo editor");
        ///             // ... lógica
        ///             _logger.LogSuccess("Edit", "Editor aberto com sucesso");
        ///             return true;
        ///         }
        ///         catch (Exception ex)
        ///         {
        ///             _logger.LogError(ex, "Erro ao abrir editor");
        ///             return false;
        ///         }
        ///     }
        /// }
        /// </example>

        /// <example>
        /// // EXEMPLO 7: Em métodos de ConnectionManager
        /// public bool AcquireConnection(object txn)
        /// {
        ///     _logger.LogInformation("Adquirindo conexão para {ConnectionName}", Name);
        ///     try
        ///     {
        ///         // ... lógica de conexão
        ///         _logger.LogSuccess("AcquireConnection", "Conexão adquirida");
        ///         return true;
        ///     }
        ///     catch (Exception ex)
        ///     {
        ///         _logger.LogOperationError("AcquireConnection", ex);
        ///         return false;
        ///     }
        /// }
        /// </example>
    }

    /// <summary>
    /// Configuração de Logging em Diferentes Ambientes
    /// </summary>
    public class LoggingConfiguration
    {
        /// <summary>
        /// Configuração para DEVELOPMENT
        /// </summary>
        public static void ConfigureDevelopment()
        {
            /*
            Neste ambiente:
            - Log Level: Debug (mais detalhado)
            - Output: Console + Debug window
            - Inclui: Escopos de operação, valores detalhados
            
            Exemplo de saída:
            [14:30:45] dbug: QuattoAPIClient.UI.CorporateApiSourceUI[0]
              Initialize com metadata e service provider
            [14:30:46] info: QuattoAPIClient.UI.Forms.ApiSourceWizard[0]
              Carregando valores de configuração atual
            */
        }

        /// <summary>
        /// Configuração para PRODUCTION
        /// </summary>
        public static void ConfigureProduction()
        {
            /*
            Neste ambiente:
            - Log Level: Information (mais restritivo)
            - Output: File (Serilog with rolling file)
            - Inclui: Apenas informações importantes, erros
            - Exclui: Debug, Trace
            
            Arquivo: logs/app-{date}.log
            Retenção: 30 dias
            */
        }

        /// <summary>
        /// Configuração para TESTING
        /// </summary>
        public static void ConfigureTesting()
        {
            /*
            Neste ambiente:
            - Log Level: Trace (mais detalhado para testes)
            - Output: Console + Test output
            - Inclui: Tudo, inclusive valores internos
            
            Útil para:
            - Debugar testes
            - Verificar fluxo de execução
            - Validar ordem de operações
            */
        }
    }

    /// <summary>
    /// Boas Práticas de Logging
    /// </summary>
    public class LoggingBestPractices
    {
        /*
        ═══════════════════════════════════════════════════════════════════

        ✅ BOAS PRÁTICAS

        1. SEMPRE USE ESTRUTURAÇÃO
           BOM:    logger.LogInformation("User {UserId} logged in", userId);
           RUIM:   logger.LogInformation($"User {userId} logged in");

        2. NÃO REGISTRE INFORMAÇÕES SENSÍVEIS
           BOM:    logger.LogError("Failed to authenticate user {UserId}", userId);
           RUIM:   logger.LogError($"Failed to authenticate user {userId} with password {password}");

        3. USE ESCOPOS PARA CORRELAÇÃO
           BOM:    using (var scope = new LogScope(_logger, "Operation", correlationId)) { ... }
           RUIM:   logger.LogInformation("Starting operation");

        4. REGISTRE EXCEÇÕES COM CONTEXTO
           BOM:    logger.LogError(ex, "Failed to process order {OrderId}", orderId);
           RUIM:   logger.LogError(ex, "Error occurred");

        5. ESCOLHA O NÍVEL APROPRIADO
           Debug    - Informações de debug detalhadas
           Information - Eventos normais de operação
           Warning  - Situações potencialmente problemáticas
           Error    - Erros que precisam de atenção
           Critical - Falhas graves do sistema

        6. INCLUA CONTEXTO SUFICIENTE
           BOM:    "Processando {TotalItems} itens, concluído {ProcessedItems}"
           RUIM:   "Processando itens"

        ═══════════════════════════════════════════════════════════════════
        */
    }
}
