/*
═══════════════════════════════════════════════════════════════════
Quatto API Client - Configuration Wizard
Versão: 1.0.0
Autor: Erton Miranda / Quatto Consultoria
═══════════════════════════════════════════════════════════════════

DESCRIÇÃO:
Wizard de configuração do componente com interface visual.
Permite configurar todas as propriedades de forma intuitiva.

═══════════════════════════════════════════════════════════════════
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using QuattoAPIClient.Logging;

// NOTA: Substituir com tipos SSIS reais quando disponíveis:
// using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
// using Microsoft.SqlServer.Dts.Runtime;

namespace QuattoAPIClient.UI.Forms
{
    /// <summary>
    /// Configuration wizard for Corporate API Source component
    /// </summary>
    public partial class ApiSourceWizard : Form
    {
        private readonly object _metadata;  // Should be IDTSComponentMetaData100
        private readonly object _connections;  // Should be Connections
        private readonly object _variables;  // Should be Variables
        private readonly ILogger<ApiSourceWizard> _logger;

        // Controls
        private TabControl? tabControl;
        private Button? btnOk;
        private Button? btnCancel;
        private Button? btnApply;
        private ToolTip? toolTip;  // For help tooltips

        // General Tab Controls
        private ComboBox? cmbConnection;
        private TextBox? txtBaseUrl;
        private TextBox? txtEndpoint;
        private NumericUpDown? numPageSize;

        // Pagination Tab Controls
        private ComboBox? cmbPaginationType;
        private NumericUpDown? numStartPage;
        private NumericUpDown? numMaxPages;

        // Incremental Tab Controls
        private CheckBox? chkEnableIncremental;
        private TextBox? txtWatermarkColumn;
        private TextBox? txtSourceSystem;
        private ComboBox? cmbEnvironment;

        // Storage Tab Controls
        private ComboBox? cmbRawStoreMode;
        private TextBox? txtRawStoreTarget;
        private CheckBox? chkCompressRawJson;
        private CheckBox? chkHashRawJson;

        // Advanced Tab Controls
        private NumericUpDown? numMaxRetries;
        private ComboBox? cmbBackoffMode;
        private NumericUpDown? numBaseDelayMs;
        private NumericUpDown? numRateLimitRPM;
        private NumericUpDown? numTimeoutSeconds;

        /// <summary>
        /// Initializes a new instance of the ApiSourceWizard class
        /// </summary>
        public ApiSourceWizard(
            object metadata,
            object connections,
            object variables)
        {
            _metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            _connections = connections ?? throw new ArgumentNullException(nameof(connections));
            _variables = variables ?? throw new ArgumentNullException(nameof(variables));
            _logger = LoggerFactory.GetLogger<ApiSourceWizard>();

            _logger.LogInformation("ApiSourceWizard inicializado com metadados, conexões e variáveis");

            InitializeComponent();
            LoadCurrentValues();
        }

        private void InitializeComponent()
        {
            // ═══════════════════════════════════════════════════════════════
            // FORM SETUP
            // ═══════════════════════════════════════════════════════════════
            this.Text = "Quatto Corporate API Source - Configuração";
            this.Size = new Size(900, 700);  // Increased for better spacing (BLOCO 4.3)
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowIcon = false;
            this.Font = new Font("Segoe UI", 9f);

            // ═══════════════════════════════════════════════════════════════
            // MAIN PANEL LAYOUT
            // ═══════════════════════════════════════════════════════════════
            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };
            this.Controls.Add(mainPanel);

            // ═══════════════════════════════════════════════════════════════
            // TOOLTIP SETUP
            // ═══════════════════════════════════════════════════════════════
            toolTip = new ToolTip
            {
                ShowAlways = false,
                AutoPopDelay = 5000,
                InitialDelay = 500
            };

            // ═══════════════════════════════════════════════════════════════
            // TAB CONTROL - MAIN AREA
            // ═══════════════════════════════════════════════════════════════
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 10)
            };
            mainPanel.Controls.Add(tabControl);

            // Create 5 tabs
            CreateGeneralTab();
            CreatePaginationTab();
            CreateIncrementalTab();
            CreateStorageTab();
            CreateAdvancedTab();

            // ═══════════════════════════════════════════════════════════════
            // BUTTON PANEL - BOTTOM
            // ═══════════════════════════════════════════════════════════════
            var btnPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                Padding = new Padding(10),
                BackColor = SystemColors.Control
            };
            mainPanel.Controls.Add(btnPanel);

            // OK Button
            btnOk = new Button
            {
                Text = "OK",
                Width = 80,
                Height = 32,
                Location = new Point(660, 10),
                DialogResult = DialogResult.OK,
                BackColor = SystemColors.Control
            };
            btnOk.Click += (s, e) => SaveValues();
            btnPanel.Controls.Add(btnOk);

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Cancelar",
                Width = 80,
                Height = 32,
                Location = new Point(750, 10),
                DialogResult = DialogResult.Cancel,
                BackColor = SystemColors.Control
            };
            btnPanel.Controls.Add(btnCancel);

            // Apply Button
            btnApply = new Button
            {
                Text = "Aplicar",
                Width = 80,
                Height = 32,
                Location = new Point(570, 10),
                BackColor = SystemColors.Control
            };
            btnApply.Click += (s, e) => SaveValues();
            btnPanel.Controls.Add(btnApply);

            // Set default buttons
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;

            // Adicionar tooltips aos controles (BLOCO 4.1)
            AddToolTips();
        }

        /// <summary>
        /// Creates the General tab (Connection, URL, Endpoint, PageSize)
        /// </summary>
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
            const int leftMargin = 10;
            const int labelControlGap = 10;

            // ═══════════════════════════════════════════════════════════════
            // GROUP BOX: API CONNECTION (BLOCO 4.4)
            // ═══════════════════════════════════════════════════════════════
            var grpApiConnection = new GroupBox
            {
                Text = "Configuração da API",
                Location = new Point(leftMargin, y),
                Width = tab.Width - (2 * leftMargin) - 20,
                Height = 180,
                ForeColor = Color.DarkBlue,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            tab.Controls.Add(grpApiConnection);

            int groupY = 20;

            // ═══════════════════════════════════════════════════════════════
            // CONEXÃO (ComboBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblConnection = new Label
            {
                Text = "Conexão:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpApiConnection.Controls.Add(lblConnection);

            cmbConnection = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = controlWidth,
                Height = controlHeight,
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };
            grpApiConnection.Controls.Add(cmbConnection);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // BASE URL (TextBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblBaseUrl = new Label
            {
                Text = "Base URL:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpApiConnection.Controls.Add(lblBaseUrl);

            txtBaseUrl = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = controlWidth,
                Height = controlHeight,
                Text = "https://",
                Multiline = false
            };
            txtBaseUrl.Leave += (s, e) => ValidateBaseUrl();
            grpApiConnection.Controls.Add(txtBaseUrl);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // ENDPOINT (TextBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblEndpoint = new Label
            {
                Text = "Endpoint:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpApiConnection.Controls.Add(lblEndpoint);

            txtEndpoint = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = controlWidth,
                Height = controlHeight,
                Text = "/v1/",
                Multiline = false
            };
            grpApiConnection.Controls.Add(txtEndpoint);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // PAGE SIZE (NumericUpDown - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblPageSize = new Label
            {
                Text = "Tamanho Página:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpApiConnection.Controls.Add(lblPageSize);

            numPageSize = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = 150,
                Height = controlHeight,
                Value = 500,
                Minimum = 1,
                Maximum = 10000,
                Increment = 100,
                DecimalPlaces = 0
            };
            numPageSize.Leave += (s, e) => ValidatePageSize();
            grpApiConnection.Controls.Add(numPageSize);
        }

        /// <summary>
        /// Creates the Pagination tab (Type, StartPage, MaxPages)
        /// </summary>
        private void CreatePaginationTab()
        {
            var tab = new TabPage("Paginação");
            tab.Padding = new Padding(10);
            tabControl!.TabPages.Add(tab);

            int y = 10;
            const int labelWidth = 150;
            const int controlWidth = 300;
            const int controlHeight = 24;
            const int rowHeight = 35;
            const int leftMargin = 10;
            const int labelControlGap = 10;

            // ═══════════════════════════════════════════════════════════════
            // TIPO DE PAGINAÇÃO (ComboBox)
            // ═══════════════════════════════════════════════════════════════
            var lblPaginationType = new Label
            {
                Text = "Tipo Paginação:",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblPaginationType);

            cmbPaginationType = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = controlWidth,
                Height = controlHeight,
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };
            cmbPaginationType.Items.AddRange(new object[]
            {
                "Offset (padrão)",
                "Cursor",
                "Link-based",
                "None (sem paginação)"
            });
            cmbPaginationType.SelectedIndex = 0;  // Default: Offset
            tab.Controls.Add(cmbPaginationType);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // PÁGINA INICIAL (NumericUpDown)
            // ═══════════════════════════════════════════════════════════════
            var lblStartPage = new Label
            {
                Text = "Página Inicial:",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblStartPage);

            numStartPage = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = 150,
                Height = controlHeight,
                Value = 1,
                Minimum = 1,
                Maximum = 999999,
                Increment = 1,
                DecimalPlaces = 0
            };
            tab.Controls.Add(numStartPage);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // MÁXIMO DE PÁGINAS (NumericUpDown)
            // ═══════════════════════════════════════════════════════════════
            var lblMaxPages = new Label
            {
                Text = "Máx Páginas:",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblMaxPages);

            numMaxPages = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = 150,
                Height = controlHeight,
                Value = 0,  // 0 = sem limite
                Minimum = 0,
                Maximum = 999999,
                Increment = 1,
                DecimalPlaces = 0
            };
            tab.Controls.Add(numMaxPages);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // NOTA INFORMATIVA (Label)
            // ═══════════════════════════════════════════════════════════════
            var lblNote = new Label
            {
                Text = "Máx Páginas = 0 significa sem limite",
                Location = new Point(leftMargin, y + 10),
                Width = controlWidth,
                Height = controlHeight,
                Font = new System.Drawing.Font("Segoe UI", 8f, System.Drawing.FontStyle.Italic),
                ForeColor = System.Drawing.SystemColors.GrayText
            };
            tab.Controls.Add(lblNote);
        }

        /// <summary>
        /// Creates the Incremental tab (EnableIncremental, WatermarkColumn, SourceSystem, Environment)
        /// </summary>
        private void CreateIncrementalTab()
        {
            var tab = new TabPage("Incremental");
            tab.Padding = new Padding(10);
            tabControl!.TabPages.Add(tab);

            int y = 10;
            const int labelWidth = 150;
            const int controlWidth = 300;
            const int controlHeight = 24;
            const int rowHeight = 35;
            const int leftMargin = 10;
            const int labelControlGap = 10;

            // ═══════════════════════════════════════════════════════════════
            // GROUP BOX: INCREMENTAL CONFIG (BLOCO 4.4)
            // ═══════════════════════════════════════════════════════════════
            var grpIncrementalConfig = new GroupBox
            {
                Text = "Configuração Incremental",
                Location = new Point(leftMargin, y),
                Width = tab.Width - (2 * leftMargin) - 20,
                Height = 150,
                ForeColor = Color.DarkGreen,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            tab.Controls.Add(grpIncrementalConfig);

            int groupY = 20;

            // ═══════════════════════════════════════════════════════════════
            // ATIVAR INCREMENTAL (CheckBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            chkEnableIncremental = new CheckBox
            {
                Text = "Ativar Incremental",
                Location = new Point(leftMargin, groupY),
                Width = 200,
                Height = controlHeight,
                Checked = false,
                AutoSize = false
            };
            grpIncrementalConfig.Controls.Add(chkEnableIncremental);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // COLUNA WATERMARK (TextBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblWatermarkColumn = new Label
            {
                Text = "Coluna Watermark:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpIncrementalConfig.Controls.Add(lblWatermarkColumn);

            txtWatermarkColumn = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = controlWidth,
                Height = controlHeight,
                Text = "updated_at",
                Multiline = false
            };
            txtWatermarkColumn.Leave += (s, e) => ValidateWatermarkColumn();
            grpIncrementalConfig.Controls.Add(txtWatermarkColumn);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // SISTEMA (TextBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblSourceSystem = new Label
            {
                Text = "Sistema:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpIncrementalConfig.Controls.Add(lblSourceSystem);

            txtSourceSystem = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = controlWidth,
                Height = controlHeight,
                Text = "Gladium",
                Multiline = false
            };
            grpIncrementalConfig.Controls.Add(txtSourceSystem);

            y = grpIncrementalConfig.Bottom + 20;

            // ═══════════════════════════════════════════════════════════════
            // AMBIENTE (ComboBox - fora do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblEnvironment = new Label
            {
                Text = "Ambiente:",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblEnvironment);

            cmbEnvironment = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = 150,
                Height = controlHeight,
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };
            cmbEnvironment.Items.AddRange(new object[] { "DEV", "HML", "PRD" });
            cmbEnvironment.SelectedIndex = 2;
            tab.Controls.Add(cmbEnvironment);
        }

        /// <summary>
        /// Creates the Storage tab (RawStoreMode, RawStoreTarget, Compression, Hashing)
        /// </summary>
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

            // ═══════════════════════════════════════════════════════════════
            // GROUP BOX 1: ARMAZENAMENTO (BLOCO 4.4)
            // ═══════════════════════════════════════════════════════════════
            var grpArmazenamento = new GroupBox
            {
                Text = "Modo de Armazenamento",
                Location = new Point(leftMargin, y),
                Width = tab.Width - (2 * leftMargin) - 20,
                Height = 120,
                ForeColor = Color.DarkOrange,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            tab.Controls.Add(grpArmazenamento);

            int groupY = 20;

            // ═══════════════════════════════════════════════════════════════
            // MODO ARMAZENAMENTO (ComboBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblRawStoreMode = new Label
            {
                Text = "Modo:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpArmazenamento.Controls.Add(lblRawStoreMode);

            cmbRawStoreMode = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
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
            cmbRawStoreMode.SelectedIndex = 0;
            grpArmazenamento.Controls.Add(cmbRawStoreMode);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // ALVO ARMAZENAMENTO (TextBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            var lblRawStoreTarget = new Label
            {
                Text = "Alvo:",
                Location = new Point(leftMargin, groupY),
                Width = labelWidth,
                Height = controlHeight
            };
            grpArmazenamento.Controls.Add(lblRawStoreTarget);

            txtRawStoreTarget = new TextBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, groupY),
                Width = controlWidth,
                Height = controlHeight,
                Text = "",
                Multiline = false
            };
            grpArmazenamento.Controls.Add(txtRawStoreTarget);

            y = grpArmazenamento.Bottom + 20;

            // ═══════════════════════════════════════════════════════════════
            // GROUP BOX 2: OPÇÕES DE PROCESSAMENTO (BLOCO 4.4)
            // ═══════════════════════════════════════════════════════════════
            var grpProcessamento = new GroupBox
            {
                Text = "Opções de Processamento",
                Location = new Point(leftMargin, y),
                Width = tab.Width - (2 * leftMargin) - 20,
                Height = 100,
                ForeColor = Color.DarkRed,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };
            tab.Controls.Add(grpProcessamento);

            groupY = 20;

            // ═══════════════════════════════════════════════════════════════
            // COMPACTAR JSON (CheckBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            chkCompressRawJson = new CheckBox
            {
                Text = "Compactar JSON",
                Location = new Point(leftMargin, groupY),
                Width = 200,
                Height = controlHeight,
                Checked = false,
                AutoSize = false
            };
            grpProcessamento.Controls.Add(chkCompressRawJson);
            groupY += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // HASH JSON (CheckBox - dentro do GroupBox)
            // ═══════════════════════════════════════════════════════════════
            chkHashRawJson = new CheckBox
            {
                Text = "Hash JSON",
                Location = new Point(leftMargin, groupY),
                Width = 200,
                Height = controlHeight,
                Checked = false,
                AutoSize = false
            };
            grpProcessamento.Controls.Add(chkHashRawJson);
        }

        /// <summary>
        /// Creates the Advanced tab (MaxRetries, BackoffMode, RateLimitRPM, TimeoutSeconds)
        /// </summary>
        private void CreateAdvancedTab()
        {
            var tab = new TabPage("Avançado");
            tab.Padding = new Padding(10);
            tabControl!.TabPages.Add(tab);

            int y = 10;
            const int labelWidth = 150;
            const int numericWidth = 150;
            const int controlHeight = 24;
            const int rowHeight = 35;
            const int leftMargin = 10;
            const int labelControlGap = 10;

            // ═══════════════════════════════════════════════════════════════
            // MAX RETRIES (NumericUpDown)
            // ═══════════════════════════════════════════════════════════════
            var lblMaxRetries = new Label
            {
                Text = "Max Tentativas:",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblMaxRetries);

            numMaxRetries = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = numericWidth,
                Height = controlHeight,
                Value = 5,
                Minimum = 0,
                Maximum = 10,
                Increment = 1,
                DecimalPlaces = 0
            };
            numMaxRetries.Leave += (s, e) => ValidateMaxRetries();  // Real-time validation
            tab.Controls.Add(numMaxRetries);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // BACKOFF MODE (ComboBox)
            // ═══════════════════════════════════════════════════════════════
            var lblBackoffMode = new Label
            {
                Text = "Modo Backoff:",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblBackoffMode);

            cmbBackoffMode = new ComboBox
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = numericWidth,
                Height = controlHeight,
                DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
            };
            cmbBackoffMode.Items.AddRange(new object[] { "Linear", "Exponential", "Random" });
            cmbBackoffMode.SelectedIndex = 1;  // Default: Exponential
            tab.Controls.Add(cmbBackoffMode);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // BASE DELAY (ms) (NumericUpDown)
            // ═══════════════════════════════════════════════════════════════
            var lblBaseDelayMs = new Label
            {
                Text = "Delay Base (ms):",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblBaseDelayMs);

            numBaseDelayMs = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = numericWidth,
                Height = controlHeight,
                Value = 1000,
                Minimum = 100,
                Maximum = 60000,
                Increment = 100,
                DecimalPlaces = 0
            };
            tab.Controls.Add(numBaseDelayMs);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // RATE LIMIT (RPM) (NumericUpDown)
            // ═══════════════════════════════════════════════════════════════
            var lblRateLimitRPM = new Label
            {
                Text = "Rate Limit (rpm):",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblRateLimitRPM);

            numRateLimitRPM = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = numericWidth,
                Height = controlHeight,
                Value = 120,
                Minimum = 1,
                Maximum = 10000,
                Increment = 10,
                DecimalPlaces = 0
            };
            numRateLimitRPM.Leave += (s, e) => ValidateRateLimit();  // Real-time validation
            tab.Controls.Add(numRateLimitRPM);
            y += rowHeight;

            // ═══════════════════════════════════════════════════════════════
            // TIMEOUT (SEGUNDOS) (NumericUpDown)
            // ═══════════════════════════════════════════════════════════════
            var lblTimeoutSeconds = new Label
            {
                Text = "Timeout (seg):",
                Location = new Point(leftMargin, y),
                Width = labelWidth,
                Height = controlHeight
            };
            tab.Controls.Add(lblTimeoutSeconds);

            numTimeoutSeconds = new NumericUpDown
            {
                Location = new Point(leftMargin + labelWidth + labelControlGap, y),
                Width = numericWidth,
                Height = controlHeight,
                Value = 100,
                Minimum = 10,
                Maximum = 600,
                Increment = 10,
                DecimalPlaces = 0
            };
            numTimeoutSeconds.Leave += (s, e) => ValidateTimeout();  // Real-time validation
            tab.Controls.Add(numTimeoutSeconds);
        }

        private void LoadCurrentValues()
        {
            _logger.LogInformation("Carregando valores de configuração atual");
            try
            {
                // ═══════════════════════════════════════════════════════════════
                // GENERAL TAB - Carregar valores
                // ═══════════════════════════════════════════════════════════════

                // Base URL
                string baseUrl = GetPropertyValue("BaseUrl");
                if (!string.IsNullOrEmpty(baseUrl))
                    txtBaseUrl!.Text = baseUrl;

                // Endpoint
                string endpoint = GetPropertyValue("Endpoint");
                if (!string.IsNullOrEmpty(endpoint))
                    txtEndpoint!.Text = endpoint;

                // Page Size
                string pageSizeStr = GetPropertyValue("PageSize");
                if (!string.IsNullOrEmpty(pageSizeStr) && int.TryParse(pageSizeStr, out int pageSize))
                    numPageSize!.Value = pageSize;

                // ═══════════════════════════════════════════════════════════════
                // PAGINATION TAB - Carregar valores
                // ═══════════════════════════════════════════════════════════════

                // Pagination Type
                string paginationType = GetPropertyValue("PaginationType");
                if (!string.IsNullOrEmpty(paginationType))
                {
                    int idx = cmbPaginationType!.Items.IndexOf(paginationType);
                    if (idx >= 0)
                        cmbPaginationType.SelectedIndex = idx;
                }

                // Start Page
                string startPageStr = GetPropertyValue("StartPage");
                if (!string.IsNullOrEmpty(startPageStr) && int.TryParse(startPageStr, out int startPage))
                    numStartPage!.Value = startPage;

                // Max Pages
                string maxPagesStr = GetPropertyValue("MaxPages");
                if (!string.IsNullOrEmpty(maxPagesStr) && int.TryParse(maxPagesStr, out int maxPages))
                    numMaxPages!.Value = maxPages;

                // ═══════════════════════════════════════════════════════════════
                // INCREMENTAL TAB - Carregar valores
                // ═══════════════════════════════════════════════════════════════

                // Enable Incremental
                string enableIncremental = GetPropertyValue("EnableIncremental");
                if (!string.IsNullOrEmpty(enableIncremental) && bool.TryParse(enableIncremental, out bool isIncremental))
                    chkEnableIncremental!.Checked = isIncremental;

                // Watermark Column
                string watermarkColumn = GetPropertyValue("WatermarkColumn");
                if (!string.IsNullOrEmpty(watermarkColumn))
                    txtWatermarkColumn!.Text = watermarkColumn;

                // Source System
                string sourceSystem = GetPropertyValue("SourceSystem");
                if (!string.IsNullOrEmpty(sourceSystem))
                    txtSourceSystem!.Text = sourceSystem;

                // Environment
                string environment = GetPropertyValue("Environment");
                if (!string.IsNullOrEmpty(environment))
                {
                    int idx = cmbEnvironment!.Items.IndexOf(environment);
                    if (idx >= 0)
                        cmbEnvironment.SelectedIndex = idx;
                }

                // ═══════════════════════════════════════════════════════════════
                // STORAGE TAB - Carregar valores
                // ═══════════════════════════════════════════════════════════════

                // Raw Store Mode
                string rawStoreMode = GetPropertyValue("RawStoreMode");
                if (!string.IsNullOrEmpty(rawStoreMode))
                {
                    int idx = cmbRawStoreMode!.Items.IndexOf(rawStoreMode);
                    if (idx >= 0)
                        cmbRawStoreMode.SelectedIndex = idx;
                }

                // Raw Store Target
                string rawStoreTarget = GetPropertyValue("RawStoreTarget");
                if (!string.IsNullOrEmpty(rawStoreTarget))
                    txtRawStoreTarget!.Text = rawStoreTarget;

                // Compress Raw JSON
                string compressRaw = GetPropertyValue("CompressRawJson");
                if (!string.IsNullOrEmpty(compressRaw) && bool.TryParse(compressRaw, out bool shouldCompress))
                    chkCompressRawJson!.Checked = shouldCompress;

                // Hash Raw JSON
                string hashRaw = GetPropertyValue("HashRawJson");
                if (!string.IsNullOrEmpty(hashRaw) && bool.TryParse(hashRaw, out bool shouldHash))
                    chkHashRawJson!.Checked = shouldHash;

                // ═══════════════════════════════════════════════════════════════
                // ADVANCED TAB - Carregar valores
                // ═══════════════════════════════════════════════════════════════

                // Max Retries
                string maxRetriesStr = GetPropertyValue("MaxRetries");
                if (!string.IsNullOrEmpty(maxRetriesStr) && int.TryParse(maxRetriesStr, out int maxRetries))
                    numMaxRetries!.Value = maxRetries;

                // Backoff Mode
                string backoffMode = GetPropertyValue("BackoffMode");
                if (!string.IsNullOrEmpty(backoffMode))
                {
                    int idx = cmbBackoffMode!.Items.IndexOf(backoffMode);
                    if (idx >= 0)
                        cmbBackoffMode.SelectedIndex = idx;
                }

                // Base Delay Ms
                string baseDelayStr = GetPropertyValue("BaseDelayMs");
                if (!string.IsNullOrEmpty(baseDelayStr) && int.TryParse(baseDelayStr, out int baseDelay))
                    numBaseDelayMs!.Value = baseDelay;

                // Rate Limit RPM
                string rateLimitStr = GetPropertyValue("RateLimitRPM");
                if (!string.IsNullOrEmpty(rateLimitStr) && int.TryParse(rateLimitStr, out int rateLimit))
                    numRateLimitRPM!.Value = rateLimit;

                // Timeout Seconds
                string timeoutStr = GetPropertyValue("TimeoutSeconds");
                if (!string.IsNullOrEmpty(timeoutStr) && int.TryParse(timeoutStr, out int timeout))
                    numTimeoutSeconds!.Value = timeout;

                _logger.LogSuccess("LoadCurrentValues", "Configurações carregadas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar valores: {ErrorMessage}", ex.Message);
                MessageBox.Show(
                    $"Erro ao carregar valores: {ex.Message}",
                    "Erro de Configuração",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SaveValues()
        {
            using (var scope = new LogScope(_logger, nameof(SaveValues)))
            {
                try
                {
                    _logger.LogInformation("Iniciando validação e salvamento de configurações");

                    // ═══════════════════════════════════════════════════════════════
                    // VALIDAÇÃO BÁSICA
                    // ═══════════════════════════════════════════════════════════════
                    if (ValidateProperties() == false)
                    {
                        _logger.LogWarning("Validação de propriedades falhou - operação cancelada");
                        // ValidateProperties() já mostra mensagens de erro
                        return;
                    }

                // ═══════════════════════════════════════════════════════════════
                // GENERAL TAB - Salvar valores
                // ═══════════════════════════════════════════════════════════════

                SetPropertyValue("BaseUrl", txtBaseUrl!.Text);
                SetPropertyValue("Endpoint", txtEndpoint!.Text);
                SetPropertyValue("PageSize", numPageSize!.Value.ToString());

                // ═══════════════════════════════════════════════════════════════
                // PAGINATION TAB - Salvar valores
                // ═══════════════════════════════════════════════════════════════

                SetPropertyValue("PaginationType", cmbPaginationType!.SelectedItem?.ToString() ?? "Offset (padrão)");
                SetPropertyValue("StartPage", numStartPage!.Value.ToString());
                SetPropertyValue("MaxPages", numMaxPages!.Value.ToString());

                // ═══════════════════════════════════════════════════════════════
                // INCREMENTAL TAB - Salvar valores
                // ═══════════════════════════════════════════════════════════════

                SetPropertyValue("EnableIncremental", chkEnableIncremental!.Checked.ToString());
                SetPropertyValue("WatermarkColumn", txtWatermarkColumn!.Text);
                SetPropertyValue("SourceSystem", txtSourceSystem!.Text);
                SetPropertyValue("Environment", cmbEnvironment!.SelectedItem?.ToString() ?? "PRD");

                // ═══════════════════════════════════════════════════════════════
                // STORAGE TAB - Salvar valores
                // ═══════════════════════════════════════════════════════════════

                SetPropertyValue("RawStoreMode", cmbRawStoreMode!.SelectedItem?.ToString() ?? "None");
                SetPropertyValue("RawStoreTarget", txtRawStoreTarget!.Text);
                SetPropertyValue("CompressRawJson", chkCompressRawJson!.Checked.ToString());
                SetPropertyValue("HashRawJson", chkHashRawJson!.Checked.ToString());

                // ═══════════════════════════════════════════════════════════════
                // ADVANCED TAB - Salvar valores
                // ═══════════════════════════════════════════════════════════════

                SetPropertyValue("MaxRetries", numMaxRetries!.Value.ToString());
                SetPropertyValue("BackoffMode", cmbBackoffMode!.SelectedItem?.ToString() ?? "Exponential");
                SetPropertyValue("BaseDelayMs", numBaseDelayMs!.Value.ToString());
                SetPropertyValue("RateLimitRPM", numRateLimitRPM!.Value.ToString());
                SetPropertyValue("TimeoutSeconds", numTimeoutSeconds!.Value.ToString());

                // ═══════════════════════════════════════════════════════════════
                // DISPARAR EVENTO DE MODIFICAÇÃO (implementar quando SSIS disponível)
                // ═══════════════════════════════════════════════════════════════

                // FireComponentMetaDataModifiedEvent();

                _logger.LogSuccess("SaveValues", "Todas as configurações salvas com sucesso");

                MessageBox.Show(
                    "Configurações salvas com sucesso!",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar configurações: {ErrorMessage}", ex.Message);
                MessageBox.Show(
                    $"Erro ao salvar valores: {ex.Message}",
                    "Erro de Persistência",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            }
        }

        /// <summary>
        /// Validar propriedades antes de salvar (com warnings)
        /// </summary>
        private bool ValidateProperties()
        {
            try
            {
                var errors = new System.Collections.Generic.List<string>();
                var warnings = new System.Collections.Generic.List<string>();

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE URL (OBRIGATÓRIA)
                // ═══════════════════════════════════════════════════════════════
                if (string.IsNullOrWhiteSpace(txtBaseUrl!.Text))
                {
                    errors.Add("• Base URL não pode estar vazia");
                }
                else if (!txtBaseUrl.Text.StartsWith("http://") && !txtBaseUrl.Text.StartsWith("https://"))
                {
                    errors.Add("• Base URL deve começar com http:// ou https://");
                }

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE PAGE SIZE (OBRIGATÓRIA)
                // ═══════════════════════════════════════════════════════════════
                if (numPageSize!.Value <= 0 || numPageSize.Value > 10000)
                {
                    errors.Add("• Page Size deve estar entre 1 e 10.000");
                }
                else if (numPageSize.Value > 5000)
                {
                    warnings.Add("⚠ Page Size > 5.000 pode ser lento com muitos registros");
                }

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE INCREMENTAL (CONDICIONAL)
                // ═══════════════════════════════════════════════════════════════
                if (chkEnableIncremental!.Checked)
                {
                    if (string.IsNullOrWhiteSpace(txtWatermarkColumn!.Text))
                    {
                        errors.Add("• Coluna Watermark é obrigatória quando Incremental ativado");
                    }
                }

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE TIMEOUT (OBRIGATÓRIA)
                // ═══════════════════════════════════════════════════════════════
                if (numTimeoutSeconds!.Value < 10 || numTimeoutSeconds.Value > 600)
                {
                    errors.Add("• Timeout deve estar entre 10 e 600 segundos");
                }
                else if (numTimeoutSeconds.Value < 30)
                {
                    warnings.Add("⚠ Timeout < 30s pode ser curto demais para requisições longas");
                }

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE MAX RETRIES (OBRIGATÓRIA)
                // ═══════════════════════════════════════════════════════════════
                if (numMaxRetries!.Value < 0 || numMaxRetries.Value > 10)
                {
                    errors.Add("• Max Tentativas deve estar entre 0 e 10");
                }

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE RATE LIMIT (OBRIGATÓRIA)
                // ═══════════════════════════════════════════════════════════════
                if (numRateLimitRPM!.Value < 1 || numRateLimitRPM.Value > 10000)
                {
                    errors.Add("• Rate Limit deve estar entre 1 e 10.000 rpm");
                }
                else if (numRateLimitRPM.Value > 1000)
                {
                    warnings.Add("⚠ Taxa > 1.000 rpm pode sobrecarregar a API");
                }

                // ═══════════════════════════════════════════════════════════════
                // VALIDAÇÃO DE BASE DELAY (OBRIGATÓRIA)
                // ═══════════════════════════════════════════════════════════════
                if (numBaseDelayMs!.Value < 100 || numBaseDelayMs.Value > 60000)
                {
                    errors.Add("• Delay Base deve estar entre 100 e 60.000 ms");
                }
                else if (numBaseDelayMs.Value > 30000)
                {
                    warnings.Add("⚠ Delay > 30s é muito longo entre retries");
                }

                // ═══════════════════════════════════════════════════════════════
                // EXIBIR ERROS (BLOQUEANTES)
                // ═══════════════════════════════════════════════════════════════
                if (errors.Count > 0)
                {
                    string errorMessage = "Erros encontrados na configuração:\n\n" + string.Join("\n", errors);
                    MessageBox.Show(
                        errorMessage,
                        "Validação - Erros",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }

                // ═══════════════════════════════════════════════════════════════
                // EXIBIR WARNINGS (NÃO-BLOQUEANTES)
                // ═══════════════════════════════════════════════════════════════
                if (warnings.Count > 0)
                {
                    string warningMessage = "Avisos sobre a configuração:\n\n" + 
                                            string.Join("\n", warnings) + 
                                            "\n\nDeseja continuar mesmo assim?";
                    
                    DialogResult result = MessageBox.Show(
                        warningMessage,
                        "Validação - Avisos",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);
                    
                    if (result != DialogResult.Yes)
                        return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro durante validação: {ex.Message}",
                    "Erro de Validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Helper para ler propriedade do metadata
        /// </summary>
        private string GetPropertyValue(string propertyName)
        {
            try
            {
                // TODO: Quando tipos SSIS estiverem disponíveis, implementar:
                // 
                // Opção 1: CustomPropertyCollection
                // var customProps = _metadata.CustomPropertyCollection;
                // if (customProps != null && customProps.Contains(propertyName))
                //     return customProps[propertyName].Value?.ToString() ?? string.Empty;
                //
                // Opção 2: ComponentProperties
                // var componentProps = _metadata.ComponentProperties;
                // if (componentProps != null && componentProps.Contains(propertyName))
                //     return componentProps[propertyName].Value?.ToString() ?? string.Empty;
                //
                // Por enquanto, retornar vazio (valores padrão já estão no InitializeComponent)

                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao ler propriedade {propertyName}: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Helper para escrever propriedade no metadata
        /// </summary>
        private void SetPropertyValue(string propertyName, string value)
        {
            try
            {
                // TODO: Quando tipos SSIS estiverem disponíveis, implementar:
                //
                // Opção 1: CustomPropertyCollection
                // var customProps = _metadata.CustomPropertyCollection;
                // if (customProps != null && customProps.Contains(propertyName))
                // {
                //     customProps[propertyName].Value = value;
                //     return;
                // }
                //
                // Opção 2: ComponentProperties
                // var componentProps = _metadata.ComponentProperties;
                // if (componentProps != null && componentProps.Contains(propertyName))
                // {
                //     componentProps[propertyName].Value = value;
                //     return;
                // }
                //
                // Log se propriedade não encontrada
                // System.Diagnostics.Debug.WriteLine($"Propriedade não encontrada: {propertyName}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao escrever propriedade {propertyName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Validação em tempo real - BaseUrl (BLOCO 3.2)
        /// </summary>
        private void ValidateBaseUrl()
        {
            if (txtBaseUrl == null)
                return;

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtBaseUrl.Text))
            {
                SetFieldError(txtBaseUrl, "URL obrigatória");
                isValid = false;
            }
            else if (!txtBaseUrl.Text.StartsWith("http://") && !txtBaseUrl.Text.StartsWith("https://"))
            {
                SetFieldError(txtBaseUrl, "Deve começar com http:// ou https://");
                isValid = false;
            }

            if (isValid)
                ClearFieldError(txtBaseUrl);
        }

        /// <summary>
        /// Validação em tempo real - PageSize (BLOCO 3.2)
        /// </summary>
        private void ValidatePageSize()
        {
            if (numPageSize == null)
                return;

            bool isValid = true;

            if (numPageSize.Value <= 0 || numPageSize.Value > 10000)
            {
                SetFieldError(numPageSize, "Deve estar entre 1 e 10.000");
                isValid = false;
            }

            if (isValid)
                ClearFieldError(numPageSize);
        }

        /// <summary>
        /// Validação em tempo real - WatermarkColumn (BLOCO 3.2)
        /// </summary>
        private void ValidateWatermarkColumn()
        {
            if (txtWatermarkColumn == null)
                return;

            // Só valida se incremental está ativado
            if (!chkEnableIncremental!.Checked)
            {
                ClearFieldError(txtWatermarkColumn);
                return;
            }

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtWatermarkColumn.Text))
            {
                SetFieldError(txtWatermarkColumn, "Obrigatório quando Incremental ativado");
                isValid = false;
            }

            if (isValid)
                ClearFieldError(txtWatermarkColumn);
        }

        /// <summary>
        /// Validação em tempo real - Timeout (BLOCO 3.2)
        /// </summary>
        private void ValidateTimeout()
        {
            if (numTimeoutSeconds == null)
                return;

            bool isValid = true;

            if (numTimeoutSeconds.Value < 10 || numTimeoutSeconds.Value > 600)
            {
                SetFieldError(numTimeoutSeconds, "Deve estar entre 10 e 600 segundos");
                isValid = false;
            }

            if (isValid)
                ClearFieldError(numTimeoutSeconds);
        }

        /// <summary>
        /// Validação em tempo real - MaxRetries (BLOCO 3.2)
        /// </summary>
        private void ValidateMaxRetries()
        {
            if (numMaxRetries == null)
                return;

            bool isValid = true;

            if (numMaxRetries.Value < 0 || numMaxRetries.Value > 10)
            {
                SetFieldError(numMaxRetries, "Deve estar entre 0 e 10");
                isValid = false;
            }

            if (isValid)
                ClearFieldError(numMaxRetries);
        }

        /// <summary>
        /// Validação em tempo real - RateLimit (BLOCO 3.2)
        /// </summary>
        private void ValidateRateLimit()
        {
            if (numRateLimitRPM == null)
                return;

            bool isValid = true;

            if (numRateLimitRPM.Value < 1 || numRateLimitRPM.Value > 10000)
            {
                SetFieldError(numRateLimitRPM, "Deve estar entre 1 e 10.000 rpm");
                isValid = false;
            }

            if (isValid)
                ClearFieldError(numRateLimitRPM);
        }

        /// <summary>
        /// Definir feedback visual de erro no controle
        /// </summary>
        private void SetFieldError(Control control, string message)
        {
            control.BackColor = Color.FromArgb(255, 240, 245);  // MistyRose
            if (toolTip != null)
                toolTip.SetToolTip(control, $"❌ {message}");
        }

        /// <summary>
        /// Limpar feedback visual de erro no controle
        /// </summary>
        private void ClearFieldError(Control control)
        {
            control.BackColor = SystemColors.Window;
            // Manter o tooltip original
        }

        /// <summary>
        /// Adicionar tooltips aos controles (BLOCO 4.1)
        /// </summary>
        private void AddToolTips()
        {
            if (toolTip == null)
                return;

            // ═══════════════════════════════════════════════════════════════
            // GENERAL TAB
            // ═══════════════════════════════════════════════════════════════
            if (cmbConnection != null)
                toolTip.SetToolTip(cmbConnection, "Selecione o Connection Manager para conectar à API");

            if (txtBaseUrl != null)
                toolTip.SetToolTip(txtBaseUrl, "URL base da API (ex: https://api.exemplo.com)");

            if (txtEndpoint != null)
                toolTip.SetToolTip(txtEndpoint, "Path do endpoint (ex: /v1/orders)");

            if (numPageSize != null)
                toolTip.SetToolTip(numPageSize, "Quantidade de registros carregados por página (1-10000)");

            // ═══════════════════════════════════════════════════════════════
            // PAGINATION TAB
            // ═══════════════════════════════════════════════════════════════
            if (cmbPaginationType != null)
                toolTip.SetToolTip(cmbPaginationType, "Estratégia de paginação suportada pela API");

            if (numStartPage != null)
                toolTip.SetToolTip(numStartPage, "Primeira página a carregar (geralmente 1 ou 0)");

            if (numMaxPages != null)
                toolTip.SetToolTip(numMaxPages, "Número máximo de páginas (0 = sem limite)");

            // ═══════════════════════════════════════════════════════════════
            // INCREMENTAL TAB
            // ═══════════════════════════════════════════════════════════════
            if (chkEnableIncremental != null)
                toolTip.SetToolTip(chkEnableIncremental, "Carregar apenas novos registros desde último carregamento");

            if (txtWatermarkColumn != null)
                toolTip.SetToolTip(txtWatermarkColumn, "Coluna usada para rastrear o último valor carregado");

            if (txtSourceSystem != null)
                toolTip.SetToolTip(txtSourceSystem, "Identificador do sistema de origem (ex: Gladium)");

            if (cmbEnvironment != null)
                toolTip.SetToolTip(cmbEnvironment, "Ambiente onde a API está rodando (DEV/HML/PRD)");

            // ═══════════════════════════════════════════════════════════════
            // STORAGE TAB
            // ═══════════════════════════════════════════════════════════════
            if (cmbRawStoreMode != null)
                toolTip.SetToolTip(cmbRawStoreMode, "Como armazenar o JSON bruto da resposta");

            if (txtRawStoreTarget != null)
                toolTip.SetToolTip(txtRawStoreTarget, "Caminho ou coluna de destino para armazenamento");

            if (chkCompressRawJson != null)
                toolTip.SetToolTip(chkCompressRawJson, "Remove espaços em branco do JSON para economizar espaço");

            if (chkHashRawJson != null)
                toolTip.SetToolTip(chkHashRawJson, "Calcula SHA256 do JSON para detectar alterações");

            // ═══════════════════════════════════════════════════════════════
            // ADVANCED TAB
            // ═══════════════════════════════════════════════════════════════
            if (numMaxRetries != null)
                toolTip.SetToolTip(numMaxRetries, "Número de tentativas automáticas em caso de erro");

            if (cmbBackoffMode != null)
                toolTip.SetToolTip(cmbBackoffMode, "Estratégia de espera entre retries (Linear/Exponential/Random)");

            if (numBaseDelayMs != null)
                toolTip.SetToolTip(numBaseDelayMs, "Delay inicial em milissegundos entre retries");

            if (numRateLimitRPM != null)
                toolTip.SetToolTip(numRateLimitRPM, "Limite máximo de requisições por minuto");

            if (numTimeoutSeconds != null)
                toolTip.SetToolTip(numTimeoutSeconds, "Tempo máximo de espera por resposta em segundos");
        }
    }
}
