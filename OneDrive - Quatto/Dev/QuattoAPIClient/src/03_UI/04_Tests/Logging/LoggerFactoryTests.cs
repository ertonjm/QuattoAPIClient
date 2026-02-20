/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - LoggerFactory Unit Tests
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Testes unitários para a classe LoggerFactory que gerencia
loggers estruturados de forma centralizada.

COBERTURA:
- GetLogger<T>() genérico
- GetLogger(string) por categoria
- Singleton pattern
- Reset funciona corretamente
- Factory é thread-safe

═══════════════════════════════════════════════════════════════════
*/

using System;
using Microsoft.Extensions.Logging;
using QuattoAPIClient.Logging;

namespace QuattoAPIClient.Tests.Logging
{
    /// <summary>
    /// Testes para LoggerFactory
    /// </summary>
    public class LoggerFactoryTests
    {
        /// <summary>
        /// Setup: Sempre limpar factory antes de cada teste
        /// </summary>
        public LoggerFactoryTests()
        {
            LoggerFactory.Reset();
        }

        [Fact(DisplayName = "GetLogger<T> deve retornar ILogger<T>")]
        public void GetLogger_Generic_ReturnsLogger()
        {
            // Arrange
            var expectedType = typeof(ILogger<LoggerFactoryTests>);

            // Act
            var logger = LoggerFactory.GetLogger<LoggerFactoryTests>();

            // Assert
            Assert.NotNull(logger);
            Assert.IsAssignableFrom<ILogger<LoggerFactoryTests>>(logger);
        }

        [Fact(DisplayName = "GetLogger(string) deve retornar ILogger")]
        public void GetLogger_String_ReturnsLogger()
        {
            // Arrange
            string categoryName = "TestCategory";

            // Act
            var logger = LoggerFactory.GetLogger(categoryName);

            // Assert
            Assert.NotNull(logger);
            Assert.IsAssignableFrom<ILogger>(logger);
        }

        [Fact(DisplayName = "Factory deve ser Singleton")]
        public void Factory_IsSingleton()
        {
            // Arrange & Act
            var factory1 = LoggerFactory.Factory;
            var factory2 = LoggerFactory.Factory;

            // Assert
            Assert.Same(factory1, factory2);
        }

        [Fact(DisplayName = "GetLogger<T> deve retornar mesmo logger para mesma classe")]
        public void GetLogger_SameClass_ReturnsSameLogger()
        {
            // Arrange & Act
            var logger1 = LoggerFactory.GetLogger<LoggerFactoryTests>();
            var logger2 = LoggerFactory.GetLogger<LoggerFactoryTests>();

            // Assert
            Assert.Same(logger1, logger2);
        }

        [Fact(DisplayName = "Reset deve limpar factory")]
        public void Reset_ClearsFactory()
        {
            // Arrange
            var logger1 = LoggerFactory.GetLogger<LoggerFactoryTests>();
            var factory1 = LoggerFactory.Factory;

            // Act
            LoggerFactory.Reset();
            var factory2 = LoggerFactory.Factory;

            // Assert
            Assert.NotSame(factory1, factory2);
        }

        [Fact(DisplayName = "Dispose deve liberar recursos")]
        public void Dispose_ReleasesResources()
        {
            // Arrange
            var factory = LoggerFactory.Factory;

            // Act
            LoggerFactory.Dispose();

            // Assert - factory deve ser recreado após dispose
            var newFactory = LoggerFactory.Factory;
            Assert.NotNull(newFactory);
        }

        [Fact(DisplayName = "GetLogger deve ser thread-safe")]
        public void GetLogger_IsThreadSafe()
        {
            // Arrange
            LoggerFactory.Reset();
            var loggers = new System.Collections.Concurrent.ConcurrentBag<ILogger>();
            var tasks = new System.Collections.Generic.List<System.Threading.Tasks.Task>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                var task = System.Threading.Tasks.Task.Run(() =>
                {
                    var logger = LoggerFactory.GetLogger<LoggerFactoryTests>();
                    loggers.Add(logger);
                });
                tasks.Add(task);
            }
            System.Threading.Tasks.Task.WaitAll(tasks.ToArray());

            // Assert
            Assert.Equal(10, loggers.Count);
            // Todos devem ser a mesma instância
            var firstLogger = loggers.First();
            foreach (var logger in loggers)
            {
                Assert.Same(firstLogger, logger);
            }
        }
    }

    /// <summary>
    /// Testes para extensões de logging
    /// </summary>
    public class LoggerExtensionsTests
    {
        [Fact(DisplayName = "LogSuccess deve registrar como Information")]
        public void LogSuccess_LogsAsInformation()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var operation = "TestOperation";

            // Act
            mockLogger.Object.LogSuccess(operation);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact(DisplayName = "LogOperationError deve registrar exceção")]
        public void LogOperationError_LogsException()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var operation = "TestOperation";
            var ex = new InvalidOperationException("Test error");

            // Act
            mockLogger.Object.LogOperationError(operation, ex);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact(DisplayName = "LogWarning deve registrar como Warning")]
        public void LogWarning_LogsAsWarning()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var message = "Test warning";

            // Act
            mockLogger.Object.LogWarning(message);

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }

    /// <summary>
    /// Testes para LogScope
    /// </summary>
    public class LogScopeTests
    {
        [Fact(DisplayName = "LogScope deve implementar IDisposable")]
        public void LogScope_ImplementsIDisposable()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var scope = new LogScope(mockLogger.Object, "TestOp");

            // Act & Assert
            Assert.IsAssignableFrom<IDisposable>(scope);
            scope.Dispose();
        }

        [Fact(DisplayName = "LogScope deve funcionar com using")]
        public void LogScope_WorksWithUsing()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();

            // Act & Assert - não deve lançar exceção
            using (var scope = new LogScope(mockLogger.Object, "TestOp"))
            {
                Assert.NotNull(scope);
            }
        }

        [Fact(DisplayName = "LogScope pode ser criado com correlationId")]
        public void LogScope_CanHaveCorrelationId()
        {
            // Arrange
            var mockLogger = new Mock<ILogger>();
            var correlationId = Guid.NewGuid().ToString();

            // Act
            using (var scope = new LogScope(mockLogger.Object, "TestOp", correlationId))
            {
                // Assert
                Assert.NotNull(scope);
            }
        }
    }
}
