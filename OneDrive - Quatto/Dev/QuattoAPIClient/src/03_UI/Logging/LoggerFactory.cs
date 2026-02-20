/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Structured Logging Factory
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Factory para criar e gerenciar loggers estruturados usando
Microsoft.Extensions.Logging com suporte a múltiplos providers.

FUNCIONALIDADES:
- Centralizar configuração de logging
- Suporte a Console, Debug e File output
- Contexto estruturado (correlation ID, user, etc)
- Níveis de log configuráveis
- Singleton pattern para eficiência

═══════════════════════════════════════════════════════════════════
*/

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace QuattoAPIClient.Logging
{
    /// <summary>
    /// Factory para criar e gerenciar loggers estruturados
    /// Implementa padrão Singleton para eficiência
    /// </summary>
    public static class LoggerFactory
    {
        private static ILoggerFactory? _loggerFactory;
        private static readonly object _lock = new object();

        /// <summary>
        /// Obtém a instância única do ILoggerFactory
        /// Inicializa se ainda não foi criada
        /// </summary>
        public static ILoggerFactory Factory
        {
            get
            {
                if (_loggerFactory == null)
                {
                    lock (_lock)
                    {
                        if (_loggerFactory == null)
                        {
                            _loggerFactory = CreateLoggerFactory();
                        }
                    }
                }
                return _loggerFactory;
            }
        }

        /// <summary>
        /// Cria um novo logger para um tipo específico
        /// </summary>
        /// <typeparam name="T">Tipo para o qual criar o logger (usado como categoria)</typeparam>
        /// <returns>Logger tipado</returns>
        public static ILogger<T> GetLogger<T>() where T : class
        {
            return Factory.CreateLogger<T>();
        }

        /// <summary>
        /// Cria um novo logger com categoria customizada
        /// </summary>
        /// <param name="categoryName">Nome da categoria (ex: "QuattoAPIClient.ConnectionManager")</param>
        /// <returns>Logger para a categoria</returns>
        public static ILogger GetLogger(string categoryName)
        {
            return Factory.CreateLogger(categoryName);
        }

        /// <summary>
        /// Cria a instância do ILoggerFactory com configuração padrão
        /// </summary>
        /// <returns>ILoggerFactory configurado</returns>
        private static ILoggerFactory CreateLoggerFactory()
        {
            var services = new ServiceCollection();

            // Configurar logging
            services.AddLogging(builder =>
            {
                // Nível de log padrão: Information
                builder.SetMinimumLevel(LogLevel.Information);

                // Adicionar Console logger
                builder.AddConsole(options =>
                {
                    options.IncludeScopes = true;  // Mostrar escopos
                    options.TimestampFormat = "[yyyy-MM-dd HH:mm:ss.fff] ";
                    options.UseUtcTimestamp = true;
                });

                // Adicionar Debug logger
                builder.AddDebug();

                // Configurar formatação customizada
                builder.AddSimpleConsole(options =>
                {
                    options.UseUtcTimestamp = true;
                    options.IncludeScopes = true;
                    options.SingleLine = false;
                    options.TimestampFormat = "HH:mm:ss";
                });
            });

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider.GetRequiredService<ILoggerFactory>();
        }

        /// <summary>
        /// Reinicializa o factory (útil para testes)
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _loggerFactory?.Dispose();
                _loggerFactory = null;
            }
        }

        /// <summary>
        /// Dispõe o factory e libera recursos
        /// </summary>
        public static void Dispose()
        {
            lock (_lock)
            {
                _loggerFactory?.Dispose();
                _loggerFactory = null;
            }
        }
    }

    /// <summary>
    /// Extensão para adicionar logging estruturado com escopos
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Log com contexto estruturado
        /// </summary>
        /// <example>
        /// logger.LogInformation("User {UserId} performed action {Action}",
        ///     userId, "Save Configuration");
        /// </example>
        public static void LogWithContext(
            this ILogger logger,
            LogLevel logLevel,
            Exception? exception,
            string message,
            params object?[] args)
        {
            logger.Log(logLevel, exception, message, args);
        }

        /// <summary>
        /// Log de sucesso operacional
        /// </summary>
        public static void LogSuccess(
            this ILogger logger,
            string operation,
            params object?[] args)
        {
            logger.LogInformation("✓ SUCCESS: {Operation}", $"{operation}");
        }

        /// <summary>
        /// Log de erro operacional
        /// </summary>
        public static void LogOperationError(
            this ILogger logger,
            string operation,
            Exception ex,
            params object?[] args)
        {
            logger.LogError(ex, "✗ ERROR in {Operation}: {ExceptionMessage}",
                operation, ex.Message);
        }

        /// <summary>
        /// Log de aviso
        /// </summary>
        public static void LogWarning(
            this ILogger logger,
            string message,
            params object?[] args)
        {
            logger.LogWarning("⚠ WARNING: {Message}", message);
        }
    }

    /// <summary>
    /// Scope para correlacionar logs relacionados
    /// Útil para rastrear requisições ou operações completas
    /// </summary>
    public class LogScope : IDisposable
    {
        private readonly IDisposable _scope;
        private readonly ILogger _logger;

        public LogScope(ILogger logger, string operation, string? correlationId = null)
        {
            _logger = logger;
            var state = new { operation, correlationId = correlationId ?? Guid.NewGuid().ToString() };
            _scope = _logger.BeginScope(state);
        }

        public void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
