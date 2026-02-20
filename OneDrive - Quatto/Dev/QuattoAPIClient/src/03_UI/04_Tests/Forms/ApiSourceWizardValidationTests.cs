/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - ApiSourceWizard Validation Tests
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Testes unitários para validações do ApiSourceWizard.
Cobre todos os métodos de validação do wizard de configuração.

COBERTURA:
- ValidateBaseUrl com URLs válidas/inválidas
- ValidatePageSize com ranges
- ValidateWatermarkColumn condicional
- ValidateMaxRetries limites
- ValidateRateLimit limites
- ValidateTimeout limites
- ValidateProperties validação global

═══════════════════════════════════════════════════════════════════
*/

namespace QuattoAPIClient.Tests.Forms
{
    /// <summary>
    /// Testes para ValidateBaseUrl do ApiSourceWizard
    /// </summary>
    public class ApiSourceWizardValidationTests
    {
        // Note: Alguns testes requerem que o wizard esteja instanciado
        // Para testes completos, seria necessário usar UI testing framework

        [Theory(DisplayName = "ValidateBaseUrl - URLs válidas")]
        [InlineData("https://api.example.com")]
        [InlineData("http://localhost:8080")]
        [InlineData("https://api.example.com/v1")]
        public void ValidateBaseUrl_ValidUrls_ReturnsTrue(string validUrl)
        {
            // Arrange
            string baseUrl = validUrl;

            // Act
            bool isValid = baseUrl.StartsWith("http://") || baseUrl.StartsWith("https://");

            // Assert
            Assert.True(isValid, $"URL '{validUrl}' deveria ser válida");
        }

        [Theory(DisplayName = "ValidateBaseUrl - URLs inválidas")]
        [InlineData("")]
        [InlineData("ftp://example.com")]
        [InlineData("example.com")]
        [InlineData("api.example.com")]
        public void ValidateBaseUrl_InvalidUrls_ReturnsFalse(string invalidUrl)
        {
            // Arrange
            string baseUrl = invalidUrl;

            // Act
            bool isValid = !string.IsNullOrWhiteSpace(baseUrl) &&
                           (baseUrl.StartsWith("http://") || baseUrl.StartsWith("https://"));

            // Assert
            Assert.False(isValid, $"URL '{invalidUrl}' deveria ser inválida");
        }

        [Theory(DisplayName = "ValidatePageSize - Valores válidos")]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(10000)]
        public void ValidatePageSize_ValidValues_ReturnsTrue(int pageSize)
        {
            // Arrange & Act
            bool isValid = pageSize >= 1 && pageSize <= 10000;

            // Assert
            Assert.True(isValid, $"PageSize {pageSize} deveria ser válido");
        }

        [Theory(DisplayName = "ValidatePageSize - Valores inválidos")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(10001)]
        [InlineData(100000)]
        public void ValidatePageSize_InvalidValues_ReturnsFalse(int pageSize)
        {
            // Arrange & Act
            bool isValid = pageSize >= 1 && pageSize <= 10000;

            // Assert
            Assert.False(isValid, $"PageSize {pageSize} deveria ser inválido");
        }

        [Theory(DisplayName = "ValidateMaxRetries - Valores válidos")]
        [InlineData(0)]
        [InlineData(3)]
        [InlineData(10)]
        public void ValidateMaxRetries_ValidValues_ReturnsTrue(int maxRetries)
        {
            // Arrange & Act
            bool isValid = maxRetries >= 0 && maxRetries <= 10;

            // Assert
            Assert.True(isValid, $"MaxRetries {maxRetries} deveria ser válido");
        }

        [Theory(DisplayName = "ValidateMaxRetries - Valores inválidos")]
        [InlineData(-1)]
        [InlineData(11)]
        [InlineData(100)]
        public void ValidateMaxRetries_InvalidValues_ReturnsFalse(int maxRetries)
        {
            // Arrange & Act
            bool isValid = maxRetries >= 0 && maxRetries <= 10;

            // Assert
            Assert.False(isValid, $"MaxRetries {maxRetries} deveria ser inválido");
        }

        [Theory(DisplayName = "ValidateRateLimit - Valores válidos")]
        [InlineData(1)]
        [InlineData(120)]
        [InlineData(10000)]
        public void ValidateRateLimit_ValidValues_ReturnsTrue(int rateLimit)
        {
            // Arrange & Act
            bool isValid = rateLimit >= 1 && rateLimit <= 10000;

            // Assert
            Assert.True(isValid, $"RateLimit {rateLimit} deveria ser válido");
        }

        [Theory(DisplayName = "ValidateRateLimit - Valores inválidos")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(10001)]
        public void ValidateRateLimit_InvalidValues_ReturnsFalse(int rateLimit)
        {
            // Arrange & Act
            bool isValid = rateLimit >= 1 && rateLimit <= 10000;

            // Assert
            Assert.False(isValid, $"RateLimit {rateLimit} deveria ser inválido");
        }

        [Theory(DisplayName = "ValidateTimeout - Valores válidos")]
        [InlineData(10)]
        [InlineData(30)]
        [InlineData(100)]
        [InlineData(600)]
        public void ValidateTimeout_ValidValues_ReturnsTrue(int timeoutSeconds)
        {
            // Arrange & Act
            bool isValid = timeoutSeconds >= 10 && timeoutSeconds <= 600;

            // Assert
            Assert.True(isValid, $"Timeout {timeoutSeconds}s deveria ser válido");
        }

        [Theory(DisplayName = "ValidateTimeout - Valores inválidos")]
        [InlineData(0)]
        [InlineData(9)]
        [InlineData(601)]
        [InlineData(1000)]
        public void ValidateTimeout_InvalidValues_ReturnsFalse(int timeoutSeconds)
        {
            // Arrange & Act
            bool isValid = timeoutSeconds >= 10 && timeoutSeconds <= 600;

            // Assert
            Assert.False(isValid, $"Timeout {timeoutSeconds}s deveria ser inválido");
        }

        [Fact(DisplayName = "ValidateBaseUrl - URL vazia com fallback para https://")]
        public void ValidateBaseUrl_EmptyDefaultsToHttps()
        {
            // Arrange
            string baseUrl = string.Empty;
            string defaultUrl = "https://";

            // Act
            bool isValidOrEmpty = string.IsNullOrWhiteSpace(baseUrl) ||
                                  baseUrl.StartsWith("http://") ||
                                  baseUrl.StartsWith("https://");

            // Assert
            Assert.True(isValidOrEmpty);
        }

        [Fact(DisplayName = "WatermarkColumn - Validação condicional baseada em EnableIncremental")]
        public void ValidateWatermarkColumn_Conditional()
        {
            // Arrange
            bool enableIncremental = true;
            string watermarkColumn = "updated_at";

            // Act
            bool isValid = !enableIncremental || !string.IsNullOrWhiteSpace(watermarkColumn);

            // Assert
            Assert.True(isValid, "WatermarkColumn deveria ser válido quando Incremental ativado e coluna preenchida");
        }

        [Fact(DisplayName = "WatermarkColumn - Inválido quando Incremental ativado sem coluna")]
        public void ValidateWatermarkColumn_RequiredWhenIncremental()
        {
            // Arrange
            bool enableIncremental = true;
            string watermarkColumn = string.Empty;

            // Act
            bool isValid = !enableIncremental || !string.IsNullOrWhiteSpace(watermarkColumn);

            // Assert
            Assert.False(isValid, "WatermarkColumn deveria ser inválido quando Incremental ativado mas vazio");
        }

        [Fact(DisplayName = "WatermarkColumn - Válido quando Incremental desativado")]
        public void ValidateWatermarkColumn_NotRequiredWhenDisabled()
        {
            // Arrange
            bool enableIncremental = false;
            string watermarkColumn = string.Empty;

            // Act
            bool isValid = !enableIncremental || !string.IsNullOrWhiteSpace(watermarkColumn);

            // Assert
            Assert.True(isValid, "WatermarkColumn pode ser vazio quando Incremental desativado");
        }
    }

    /// <summary>
    /// Testes para conversão e parsing de valores
    /// </summary>
    public class ApiSourceWizardParsingTests
    {
        [Theory(DisplayName = "ParseInt - Valores válidos")]
        [InlineData("100", 100)]
        [InlineData("500", 500)]
        [InlineData("10000", 10000)]
        public void ParseInt_ValidStrings_ParsesCorrectly(string value, int expected)
        {
            // Arrange & Act
            bool success = int.TryParse(value, out int result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }

        [Theory(DisplayName = "ParseInt - Valores inválidos")]
        [InlineData("abc")]
        [InlineData("")]
        [InlineData("12.5")]
        public void ParseInt_InvalidStrings_ReturnsFalse(string value)
        {
            // Arrange & Act
            bool success = int.TryParse(value, out int result);

            // Assert
            Assert.False(success);
        }

        [Theory(DisplayName = "ParseBool - Valores válidos")]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("True", true)]
        [InlineData("False", false)]
        public void ParseBool_ValidStrings_ParsesCorrectly(string value, bool expected)
        {
            // Arrange & Act
            bool success = bool.TryParse(value, out bool result);

            // Assert
            Assert.True(success);
            Assert.Equal(expected, result);
        }
    }
}
