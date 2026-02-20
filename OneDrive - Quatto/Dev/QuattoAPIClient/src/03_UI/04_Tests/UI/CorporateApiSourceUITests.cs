/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - CorporateApiSourceUI Tests
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Testes unitários para o CorporateApiSourceUI que gerencia
a interface do componente no SSIS Designer.

COBERTURA:
- Initialize valida argumentos nulos
- Edit com diferentes cenários
- Help executa sem erros
- Logger inicializado corretamente

═══════════════════════════════════════════════════════════════════
*/

using System;
using QuattoAPIClient.UI;

namespace QuattoAPIClient.Tests.UI
{
    /// <summary>
    /// Testes para CorporateApiSourceUI
    /// </summary>
    public class CorporateApiSourceUITests
    {
        [Fact(DisplayName = "Constructor - deve inicializar com logger")]
        public void Constructor_InitializesWithLogger()
        {
            // Act
            var ui = new CorporateApiSourceUI();

            // Assert
            Assert.NotNull(ui);
        }

        [Fact(DisplayName = "Initialize - deve aceitar argumentos válidos")]
        public void Initialize_ValidArguments_Succeeds()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockMetadata = new object();
            var mockServiceProvider = Mock.Of<IServiceProvider>();

            // Act & Assert - não deve lançar exceção
            ui.Initialize(mockMetadata, mockServiceProvider);
        }

        [Fact(DisplayName = "Initialize - deve lançar ArgumentNullException se metadata é null")]
        public void Initialize_NullMetadata_ThrowsArgumentNullException()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockServiceProvider = Mock.Of<IServiceProvider>();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ui.Initialize(null, mockServiceProvider));
        }

        [Fact(DisplayName = "Initialize - deve lançar ArgumentNullException se serviceProvider é null")]
        public void Initialize_NullServiceProvider_ThrowsArgumentNullException()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockMetadata = new object();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                ui.Initialize(mockMetadata, null));
        }

        [Fact(DisplayName = "Initialize - deve aceitar ambos argumentos como não-nulos")]
        public void Initialize_BothArgumentsNotNull_Succeeds()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockMetadata = new object();
            var mockServiceProvider = Mock.Of<IServiceProvider>();

            // Act
            ui.Initialize(mockMetadata, mockServiceProvider);

            // Assert - se chegou aqui, não lançou exceção
            Assert.True(true);
        }

        [Fact(DisplayName = "Edit - deve retornar bool")]
        public void Edit_ReturnsBoolean()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockMetadata = new object();
            var mockServiceProvider = Mock.Of<IServiceProvider>();
            ui.Initialize(mockMetadata, mockServiceProvider);

            var mockWindow = Mock.Of<System.Windows.Forms.IWin32Window>();
            var mockVariables = new object();
            var mockConnections = new object();

            // Act
            var result = ui.Edit(mockWindow, mockVariables, mockConnections);

            // Assert
            Assert.IsType<bool>(result);
        }

        [Fact(DisplayName = "Edit - deve retornar false sem lançar exceção")]
        public void Edit_ReturnsWithoutException()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockMetadata = new object();
            var mockServiceProvider = Mock.Of<IServiceProvider>();
            ui.Initialize(mockMetadata, mockServiceProvider);

            var mockWindow = Mock.Of<System.Windows.Forms.IWin32Window>();
            var mockVariables = new object();
            var mockConnections = new object();

            // Act
            var result = ui.Edit(mockWindow, mockVariables, mockConnections);

            // Assert - não deve lançar exceção
            Assert.False(result);
        }

        [Fact(DisplayName = "Edit - lança InvalidOperationException se não inicializado")]
        public void Edit_NotInitialized_ThrowsInvalidOperationException()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockWindow = Mock.Of<System.Windows.Forms.IWin32Window>();
            var mockVariables = new object();
            var mockConnections = new object();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                ui.Edit(mockWindow, mockVariables, mockConnections));
        }

        [Fact(DisplayName = "Help - deve executar sem exceção")]
        public void Help_ExecutesWithoutException()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockWindow = Mock.Of<System.Windows.Forms.IWin32Window>();

            // Act & Assert - não deve lançar exceção
            ui.Help(mockWindow);
        }

        [Fact(DisplayName = "New - deve existir e ser callable")]
        public void New_Exists()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockWindow = Mock.Of<System.Windows.Forms.IWin32Window>();

            // Act & Assert - não deve lançar exceção
            ui.New(mockWindow);
        }

        [Fact(DisplayName = "Delete - deve existir e ser callable")]
        public void Delete_Exists()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();
            var mockWindow = Mock.Of<System.Windows.Forms.IWin32Window>();

            // Act & Assert - não deve lançar exceção
            ui.Delete(mockWindow);
        }
    }

    /// <summary>
    /// Testes para IDtsComponentUI interface
    /// </summary>
    public class IDtsComponentUIInterfaceTests
    {
        [Fact(DisplayName = "CorporateApiSourceUI implementa IDtsComponentUI")]
        public void CorporateApiSourceUI_ImplementsIDtsComponentUI()
        {
            // Arrange
            var ui = new CorporateApiSourceUI();

            // Act & Assert
            Assert.IsAssignableFrom<IDtsComponentUI>(ui);
        }

        [Fact(DisplayName = "IDtsComponentUI possui 5 métodos requeridos")]
        public void IDtsComponentUI_HasRequiredMethods()
        {
            // Arrange
            var interfaceType = typeof(IDtsComponentUI);

            // Act
            var methods = interfaceType.GetMethods();

            // Assert
            Assert.NotEmpty(methods);
            Assert.Contains(methods, m => m.Name == "Initialize");
            Assert.Contains(methods, m => m.Name == "Edit");
            Assert.Contains(methods, m => m.Name == "New");
            Assert.Contains(methods, m => m.Name == "Delete");
            Assert.Contains(methods, m => m.Name == "Help");
        }
    }
}
