using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.Forms;

/// <summary>
/// Partial class containing Settings panel form layout and logic.
/// </summary>
public partial class MainForm
{
    // Settings form controls
    private TextBox? _siteCodeTextBox;
    private TextBox? _siteDisplayNameTextBox;
    private ComboBox? _languageComboBox;
    private NumericUpDown? _idleTimeoutNumeric;
    private NumericUpDown? _badgeValidityNumeric;
    private NumericUpDown? _badgeExpiryWarningNumeric;
    private TextBox? _orgTitleTextBox;
    private TextBox? _orgPhoneTextBox;
    private ComboBox? _logLevelComboBox;
    private NumericUpDown? _logRetentionNumeric;
    private CheckBox? _autoBackupCheckBox;
    private NumericUpDown? _backupIntervalNumeric;
    private NumericUpDown? _backupRetentionNumeric;
    private TextBox? _backupPathTextBox;
    private Button? _browseBackupPathButton;
    private CheckBox? _backupIncludePhotosCheckBox;
    private NumericUpDown? _auditRetentionNumeric;
    private NumericUpDown? _visitRetentionNumeric;

    // Badge Issuing Office fields
    private TextBox? _badgeOfficeNameTextBox;
    private TextBox? _badgeOfficePhoneTextBox;
    private TextBox? _badgeOfficeHeadFullNameTextBox;
    private TextBox? _badgeOfficeHeadJobTitleTextBox;
    private TextBox? _badgeOfficeHeadRankTextBox;
    private TextBox? _badgeOfficeNotesTextBox;

    // Network settings controls (admin only)
    private CheckBox? _networkEnabledCheckBox;
    private NumericUpDown? _networkPortNumeric;
    private TextBox? _apiKeyTextBox;
    private Button? _generateApiKeyButton;
    private DataGridView? _remoteSitesGrid;
    private Button? _addRemoteSiteButton;
    private Button? _removeRemoteSiteButton;
    private Label? _networkSectionHeader;
    private Label? _remoteSitesSectionHeader;
    private FlowLayoutPanel? _networkRow;
    private FlowLayoutPanel? _remoteSitesButtonPanel;

    /// <summary>
    /// Sets up the Settings panel with a 3-column form layout for compact display.
    /// </summary>
    private void SetupSettingsFormView()
    {
        // Clear all existing controls - we'll create buttons programmatically
        settingsPanel.Controls.Clear();

        var labelFont = new Font("Segoe UI", 9F);
        var inputFont = new Font("Segoe UI", 9F);
        var headerFont = new Font("Segoe UI", 10F, FontStyle.Bold);
        var buttonFont = new Font("Segoe UI", 9F);
        const int fieldWidth = 200; // Consistent width for all fields - increased for localized labels

        // Create bottom button panel with proper layout
        var bottomPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 45,
            ColumnCount = 5,
            RowCount = 1,
            Padding = new Padding(10, 5, 10, 5)
        };
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Spacer
        bottomPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var catalogsBtn = new Button { Text = Strings.Button_Catalogs, Font = buttonFont, AutoSize = true, Padding = new Padding(6, 2, 6, 2) };
        var backupBtn = new Button { Text = Strings.Button_Backup, Font = buttonFont, AutoSize = true, Padding = new Padding(6, 2, 6, 2) };
        var restoreBtn = new Button { Text = Strings.Button_Restore, Font = buttonFont, AutoSize = true, Padding = new Padding(6, 2, 6, 2) };
        var saveBtn = new Button { Text = Strings.Button_Save, Font = buttonFont, AutoSize = true, Padding = new Padding(6, 2, 6, 2), ForeColor = Color.Blue };

        catalogsBtn.Click += OnOpenCatalogsClick;
        backupBtn.Click += OnBackupClick;
        restoreBtn.Click += OnRestoreClick;
        saveBtn.Click += OnSaveSettingsClick;

        bottomPanel.Controls.Add(catalogsBtn, 0, 0);
        bottomPanel.Controls.Add(backupBtn, 1, 0);
        bottomPanel.Controls.Add(restoreBtn, 2, 0);
        bottomPanel.Controls.Add(saveBtn, 4, 0);

        settingsPanel.Controls.Add(bottomPanel);

        // Create scrollable container
        var scrollPanel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(5)
        };

        // 3-column layout: 6 columns total (Label, Field) x 3
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 6,
            Padding = new Padding(3, 0, 3, 0)
        };
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Label 1 - increased for Greek
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, fieldWidth + 5)); // Field 1
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Label 2 - increased for Greek
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, fieldWidth + 5)); // Field 2
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150)); // Label 3 - increased for Greek
        mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, fieldWidth + 5)); // Field 3

        int row = 0;

        // === Site Identity Section ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_SiteIdentity, headerFont, ref row);

        _siteCodeTextBox = new TextBox { Width = fieldWidth, Font = inputFont, ReadOnly = true };
        AddFormField3Col(mainLayout, Strings.Settings_SiteCode, _siteCodeTextBox, labelFont, row, 0);

        _siteDisplayNameTextBox = new TextBox { Width = fieldWidth, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_SiteDisplayName, _siteDisplayNameTextBox, labelFont, row, 2);

        _languageComboBox = new ComboBox { Width = fieldWidth, Font = inputFont, DropDownStyle = ComboBoxStyle.DropDownList };
        _languageComboBox.Items.AddRange(new[] { "el-GR", "en-US" });
        AddFormField3Col(mainLayout, Strings.Settings_Language, _languageComboBox, labelFont, row, 4);
        row++;

        // Show portable mode indicator if enabled
        if (IsPortableMode())
        {
            var portableIndicator = new Label
            {
                Text = "⚡ " + Strings.Settings_PortableModeActive,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                ForeColor = Color.DarkGreen,
                Padding = new Padding(0, 2, 0, 2)
            };
            mainLayout.Controls.Add(portableIndicator, 0, row);
            mainLayout.SetColumnSpan(portableIndicator, 6);
            row++;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        // === Organization Section ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_Organization, headerFont, ref row);

        _orgTitleTextBox = new TextBox { Width = fieldWidth, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_OrgTitle, _orgTitleTextBox, labelFont, row, 0);

        _orgPhoneTextBox = new TextBox { Width = fieldWidth, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_OrgPhone, _orgPhoneTextBox, labelFont, row, 2);

        _idleTimeoutNumeric = new NumericUpDown { Width = fieldWidth, Font = inputFont, Minimum = 0, Maximum = 120, Value = 0 };
        AddFormField3Col(mainLayout, Strings.Settings_IdleTimeout, _idleTimeoutNumeric, labelFont, row, 4);
        row++;

        // === Badge Settings Section ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_Badge, headerFont, ref row);

        _badgeValidityNumeric = new NumericUpDown { Width = fieldWidth, Font = inputFont, Minimum = -1, Maximum = 120, Value = -1 };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeValidity, _badgeValidityNumeric, labelFont, row, 0);

        _badgeExpiryWarningNumeric = new NumericUpDown { Width = fieldWidth, Font = inputFont, Minimum = 0, Maximum = 365, Value = 30 };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeExpiryWarning, _badgeExpiryWarningNumeric, labelFont, row, 2);
        row++;

        // === Badge Issuing Office Section ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_BadgeIssuingOffice, headerFont, ref row);

        _badgeOfficeNameTextBox = new TextBox { Width = fieldWidth * 2, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeOfficeName, _badgeOfficeNameTextBox, labelFont, row, 0);

        _badgeOfficePhoneTextBox = new TextBox { Width = fieldWidth, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeOfficePhone, _badgeOfficePhoneTextBox, labelFont, row, 2);
        row++;

        _badgeOfficeHeadFullNameTextBox = new TextBox { Width = fieldWidth * 2, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeOfficeHeadFullName, _badgeOfficeHeadFullNameTextBox, labelFont, row, 0);
        row++;

        _badgeOfficeHeadJobTitleTextBox = new TextBox { Width = fieldWidth, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeOfficeHeadJobTitle, _badgeOfficeHeadJobTitleTextBox, labelFont, row, 0);

        _badgeOfficeHeadRankTextBox = new TextBox { Width = fieldWidth, Font = inputFont };
        AddFormField3Col(mainLayout, Strings.Settings_BadgeOfficeHeadRank, _badgeOfficeHeadRankTextBox, labelFont, row, 2);
        row++;

        _badgeOfficeNotesTextBox = new TextBox { Width = fieldWidth * 3, Font = inputFont, Multiline = true, Height = 50 };
        mainLayout.Controls.Add(new Label { Text = Strings.Settings_BadgeOfficeNotes, AutoSize = true, Font = labelFont }, 0, row);
        mainLayout.Controls.Add(_badgeOfficeNotesTextBox, 1, row);
        mainLayout.SetColumnSpan(_badgeOfficeNotesTextBox, 5);
        row++;

        // === Logging Section ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_Logging, headerFont, ref row);

        _logLevelComboBox = new ComboBox { Width = fieldWidth, Font = inputFont, DropDownStyle = ComboBoxStyle.DropDownList };
        _logLevelComboBox.Items.AddRange(new[] { "Verbose", "Debug", "Information", "Warning", "Error", "Fatal" });
        AddFormField3Col(mainLayout, Strings.Settings_LogLevel, _logLevelComboBox, labelFont, row, 0);

        _logRetentionNumeric = new NumericUpDown { Width = fieldWidth, Font = inputFont, Minimum = 1, Maximum = 365, Value = 30 };
        AddFormField3Col(mainLayout, Strings.Settings_LogRetention, _logRetentionNumeric, labelFont, row, 2);
        row++;

        // === Backup Section - two rows ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_Backup, headerFont, ref row);

        // Row 1: Auto backup, Include photos, Interval
        var backupRow1 = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        _autoBackupCheckBox = new CheckBox { Text = Strings.Settings_AutoBackupEnabled, AutoSize = true, Font = inputFont };
        _backupIncludePhotosCheckBox = new CheckBox { Text = Strings.Settings_BackupIncludePhotos, AutoSize = true, Font = inputFont, Margin = new Padding(15, 0, 0, 0) };
        _backupIntervalNumeric = new NumericUpDown { Width = 60, Font = inputFont, Minimum = 1, Maximum = 168, Value = 24 };

        backupRow1.Controls.Add(_autoBackupCheckBox);
        backupRow1.Controls.Add(_backupIncludePhotosCheckBox);
        backupRow1.Controls.Add(new Label { Text = Strings.Settings_BackupInterval + ":", AutoSize = true, Font = labelFont, Margin = new Padding(15, 4, 5, 0) });
        backupRow1.Controls.Add(_backupIntervalNumeric);

        mainLayout.Controls.Add(backupRow1, 0, row);
        mainLayout.SetColumnSpan(backupRow1, 6);
        row++;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

        // Row 2: Retention, Path
        var backupRow2 = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        _backupRetentionNumeric = new NumericUpDown { Width = 50, Font = inputFont, Minimum = 1, Maximum = 100, Value = 7 };
        _backupPathTextBox = new TextBox { Width = 400, Font = inputFont };
        _browseBackupPathButton = new Button { Text = "...", Font = inputFont, Width = 28, Height = 23 };
        _browseBackupPathButton.Click += OnBrowseBackupPathClick;

        backupRow2.Controls.Add(new Label { Text = Strings.Settings_BackupRetention + ":", AutoSize = true, Font = labelFont, Margin = new Padding(0, 4, 5, 0) });
        backupRow2.Controls.Add(_backupRetentionNumeric);
        backupRow2.Controls.Add(new Label { Text = Strings.Settings_BackupPath + ":", AutoSize = true, Font = labelFont, Margin = new Padding(15, 4, 5, 0) });
        backupRow2.Controls.Add(_backupPathTextBox);
        backupRow2.Controls.Add(_browseBackupPathButton);

        // In portable mode, make backup path read-only to guarantee all data stays in app folder
        if (IsPortableMode())
        {
            _backupPathTextBox.ReadOnly = true;
            _browseBackupPathButton.Enabled = false;
        }

        mainLayout.Controls.Add(backupRow2, 0, row);
        mainLayout.SetColumnSpan(backupRow2, 6);
        row++;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

        // === Data Retention Section ===
        AddSectionHeader3Col(mainLayout, Strings.Settings_DataRetention, headerFont, ref row);

        _auditRetentionNumeric = new NumericUpDown { Width = fieldWidth, Font = inputFont, Minimum = 1, Maximum = 10, Value = 3 };
        AddFormField3Col(mainLayout, Strings.Settings_AuditRetention, _auditRetentionNumeric, labelFont, row, 0);

        _visitRetentionNumeric = new NumericUpDown { Width = fieldWidth, Font = inputFont, Minimum = 1, Maximum = 10, Value = 3 };
        AddFormField3Col(mainLayout, Strings.Settings_VisitRetention, _visitRetentionNumeric, labelFont, row, 2);
        row++;

        // === Network Settings Section - all in one row ===
        _networkSectionHeader = AddSectionHeader3ColWithRef(mainLayout, Strings.Settings_Network, headerFont, ref row);

        // Row with all network settings including API key
        _networkRow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false };
        var networkRow = _networkRow;
        _networkEnabledCheckBox = new CheckBox { Text = Strings.Settings_NetworkEnabled, AutoSize = true, Font = inputFont };
        _networkPortNumeric = new NumericUpDown { Width = 70, Font = inputFont, Minimum = 1024, Maximum = 65535, Value = 8080 };
        _apiKeyTextBox = new TextBox { Width = 340, Font = inputFont, ReadOnly = true };
        _generateApiKeyButton = new Button { Text = Strings.Button_RegenerateApiKey, Font = inputFont, AutoSize = true, Height = 23 };
        _generateApiKeyButton.Click += OnGenerateApiKeyClick;

        networkRow.Controls.Add(_networkEnabledCheckBox);
        networkRow.Controls.Add(new Label { Text = Strings.Settings_NetworkPort + ":", AutoSize = true, Font = labelFont, Margin = new Padding(15, 4, 5, 0) });
        networkRow.Controls.Add(_networkPortNumeric);
        networkRow.Controls.Add(new Label { Text = Strings.Settings_ApiKey + ":", AutoSize = true, Font = labelFont, Margin = new Padding(15, 4, 5, 0) });
        networkRow.Controls.Add(_apiKeyTextBox);
        networkRow.Controls.Add(_generateApiKeyButton);

        mainLayout.Controls.Add(networkRow, 0, row);
        mainLayout.SetColumnSpan(networkRow, 6);
        row++;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));

        // Remote Sites Section Header
        _remoteSitesSectionHeader = AddSectionHeader3ColWithRef(mainLayout, Strings.Settings_RemoteSites, headerFont, ref row);

        // Remote Sites Grid
        _remoteSitesGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = true,
            AllowUserToDeleteRows = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Height = 120
        };
        _remoteSitesGrid.Columns.Add("SiteCode", Strings.RemoteSite_SiteCode);
        _remoteSitesGrid.Columns.Add("DisplayName", Strings.RemoteSite_DisplayName);
        _remoteSitesGrid.Columns.Add("Host", Strings.RemoteSite_Host);
        _remoteSitesGrid.Columns.Add("Port", Strings.RemoteSite_Port);
        _remoteSitesGrid.Columns.Add("ApiKey", Strings.RemoteSite_ApiKey);
        mainLayout.Controls.Add(_remoteSitesGrid, 0, row);
        mainLayout.SetColumnSpan(_remoteSitesGrid, 6);
        row++;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 170));

        // Remote Sites Buttons
        _remoteSitesButtonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        _addRemoteSiteButton = new Button { Text = Strings.Button_AddSite, Font = inputFont, AutoSize = true };
        _removeRemoteSiteButton = new Button { Text = Strings.Button_RemoveSite, Font = inputFont, AutoSize = true, ForeColor = Color.Red };
        _addRemoteSiteButton.Click += OnAddRemoteSiteClick;
        _removeRemoteSiteButton.Click += OnRemoveRemoteSiteClick;
        _remoteSitesButtonPanel.Controls.Add(_addRemoteSiteButton);
        _remoteSitesButtonPanel.Controls.Add(_removeRemoteSiteButton);
        mainLayout.Controls.Add(_remoteSitesButtonPanel, 0, row);
        mainLayout.SetColumnSpan(_remoteSitesButtonPanel, 6);
        row++;
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

        scrollPanel.Controls.Add(mainLayout);
        settingsPanel.Controls.Add(scrollPanel);
    }

    private void OnGenerateApiKeyClick(object? sender, EventArgs e)
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var apiKey = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        _apiKeyTextBox!.Text = apiKey;
        MessageBox.Show(Strings.ApiKey_Generated, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnBrowseBackupPathClick(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = Strings.Settings_BackupPath,
            SelectedPath = _backupPathTextBox!.Text,
            ShowNewFolderButton = true
        };

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            _backupPathTextBox.Text = dialog.SelectedPath;
        }
    }

    private static bool IsPortableMode()
    {
        var portableMarkerPath = Path.Combine(AppContext.BaseDirectory, "portable.mode");
        return File.Exists(portableMarkerPath);
    }

    private static string GetDefaultBackupPath()
    {
        if (IsPortableMode())
        {
            return Path.Combine(AppContext.BaseDirectory, "PylaeData", "Backups");
        }

        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        return Path.Combine(documents, "Pylae", "Backups");
    }

    private void OnAddRemoteSiteClick(object? sender, EventArgs e)
    {
        // Add a new row to the grid - the user will fill in the values
        _remoteSitesGrid!.Rows.Add("", "", "192.168.1.x", "8080", "");
    }

    private void OnRemoveRemoteSiteClick(object? sender, EventArgs e)
    {
        if (_remoteSitesGrid!.CurrentRow != null && !_remoteSitesGrid.CurrentRow.IsNewRow)
        {
            _remoteSitesGrid.Rows.Remove(_remoteSitesGrid.CurrentRow);
        }
    }

    private void AddSectionHeader(TableLayoutPanel layout, string text, Font font, ref int row)
    {
        var header = new Label
        {
            Text = text,
            Font = font,
            ForeColor = Color.DarkBlue,
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 15, 0, 5),
            AutoSize = true
        };
        layout.Controls.Add(header, 0, row);
        layout.SetColumnSpan(header, 4);
        row++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
    }

    private void AddFormField(TableLayoutPanel layout, string labelText, Control control, Font font, int row, int col)
    {
        var label = new Label
        {
            Text = labelText + ":",
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Fill,
            Font = font
        };
        layout.Controls.Add(label, col, row);
        layout.Controls.Add(control, col + 1, row);

        if (layout.RowStyles.Count <= row)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        }
    }

    private void AddSectionHeader3Col(TableLayoutPanel layout, string text, Font font, ref int row)
    {
        AddSectionHeader3ColWithRef(layout, text, font, ref row);
    }

    private Label AddSectionHeader3ColWithRef(TableLayoutPanel layout, string text, Font font, ref int row)
    {
        var header = new Label
        {
            Text = text,
            Font = font,
            ForeColor = Color.DarkBlue,
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 8, 0, 3),
            AutoSize = true
        };
        layout.Controls.Add(header, 0, row);
        layout.SetColumnSpan(header, 6);
        row++;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        return header;
    }

    private void AddFormField3Col(TableLayoutPanel layout, string labelText, Control control, Font font, int row, int col)
    {
        var label = new Label
        {
            Text = labelText + ":",
            TextAlign = ContentAlignment.MiddleRight,
            Dock = DockStyle.Fill,
            Font = font
        };
        layout.Controls.Add(label, col, row);
        layout.Controls.Add(control, col + 1, row);

        if (layout.RowStyles.Count <= row)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
        }
    }

    private async Task LoadSettingsToFormAsync()
    {
        if (_settingsViewModel.Settings == null || _settingsViewModel.Settings.Count == 0)
        {
            await _settingsViewModel.LoadAsync();
        }

        var settings = _settingsViewModel.Settings?.ToDictionary(s => s.Key, s => s.Value)
            ?? new Dictionary<string, string>();

        _siteCodeTextBox!.Text = GetSettingValue(settings, SettingKeys.SiteCode);
        _siteDisplayNameTextBox!.Text = GetSettingValue(settings, SettingKeys.SiteDisplayName);
        _languageComboBox!.SelectedItem = GetSettingValue(settings, SettingKeys.PrimaryLanguage, "el-GR");
        _orgTitleTextBox!.Text = GetSettingValue(settings, SettingKeys.OrgBusinessTitle);
        _orgPhoneTextBox!.Text = GetSettingValue(settings, SettingKeys.OrgBusinessTel);

        if (int.TryParse(GetSettingValue(settings, SettingKeys.IdleTimeoutMinutes, "0"), out var idleTimeout))
            _idleTimeoutNumeric!.Value = Math.Clamp(idleTimeout, 0, 120);

        if (int.TryParse(GetSettingValue(settings, SettingKeys.BadgeValidityMonths, "-1"), out var badgeValidity))
            _badgeValidityNumeric!.Value = Math.Clamp(badgeValidity, -1, 120);

        if (int.TryParse(GetSettingValue(settings, SettingKeys.BadgeExpiryWarningDays, "30"), out var badgeWarning))
            _badgeExpiryWarningNumeric!.Value = Math.Clamp(badgeWarning, 0, 365);

        _logLevelComboBox!.SelectedItem = GetSettingValue(settings, SettingKeys.LogLevel, "Information");

        if (int.TryParse(GetSettingValue(settings, SettingKeys.LogRetentionDays, "30"), out var logRetention))
            _logRetentionNumeric!.Value = Math.Clamp(logRetention, 1, 365);

        _autoBackupCheckBox!.Checked = GetSettingValue(settings, SettingKeys.AutoBackupEnabled, "0") == "1";
        _backupIncludePhotosCheckBox!.Checked = GetSettingValue(settings, SettingKeys.AutoBackupIncludePhotos, "1") == "1";

        if (int.TryParse(GetSettingValue(settings, SettingKeys.AutoBackupIntervalHours, "24"), out var backupInterval))
            _backupIntervalNumeric!.Value = Math.Clamp(backupInterval, 1, 168);

        if (int.TryParse(GetSettingValue(settings, SettingKeys.AutoBackupRetentionCount, "7"), out var backupRetention))
            _backupRetentionNumeric!.Value = Math.Clamp(backupRetention, 1, 100);

        var backupPath = GetSettingValue(settings, SettingKeys.AutoBackupPath);
        _backupPathTextBox!.Text = string.IsNullOrEmpty(backupPath) ? GetDefaultBackupPath() : backupPath;

        if (int.TryParse(GetSettingValue(settings, SettingKeys.AuditRetentionYears, "3"), out var auditRetention))
            _auditRetentionNumeric!.Value = Math.Clamp(auditRetention, 1, 10);

        if (int.TryParse(GetSettingValue(settings, SettingKeys.VisitRetentionYears, "3"), out var visitRetention))
            _visitRetentionNumeric!.Value = Math.Clamp(visitRetention, 1, 10);

        // Badge Issuing Office settings
        _badgeOfficeNameTextBox!.Text = GetSettingValue(settings, SettingKeys.BadgeOfficeName);
        _badgeOfficePhoneTextBox!.Text = GetSettingValue(settings, SettingKeys.BadgeOfficePhone);
        _badgeOfficeHeadFullNameTextBox!.Text = GetSettingValue(settings, SettingKeys.BadgeOfficeHeadFullName);
        _badgeOfficeHeadJobTitleTextBox!.Text = GetSettingValue(settings, SettingKeys.BadgeOfficeHeadJobTitle);
        _badgeOfficeHeadRankTextBox!.Text = GetSettingValue(settings, SettingKeys.BadgeOfficeHeadRank);
        _badgeOfficeNotesTextBox!.Text = GetSettingValue(settings, SettingKeys.BadgeOfficeNotes);

        // Network settings (admin only)
        _networkEnabledCheckBox!.Checked = GetSettingValue(settings, SettingKeys.NetworkEnabled, "0") == "1";

        if (int.TryParse(GetSettingValue(settings, SettingKeys.NetworkPort, "8080"), out var port))
            _networkPortNumeric!.Value = Math.Clamp(port, 1024, 65535);

        var apiKey = GetSettingValue(settings, SettingKeys.NetworkApiKey);
        if (string.IsNullOrEmpty(apiKey))
        {
            // Auto-generate API key on first run
            var bytes = RandomNumberGenerator.GetBytes(32);
            apiKey = Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }
        _apiKeyTextBox!.Text = apiKey;

        // Load remote sites from database
        var remoteSiteService = _serviceProvider.GetRequiredService<IRemoteSiteService>();
        var remoteSites = await remoteSiteService.GetAllAsync();

        // Populate remote sites grid
        _remoteSitesGrid!.Rows.Clear();
        foreach (var site in remoteSites)
        {
            _remoteSitesGrid.Rows.Add(site.SiteCode ?? "", site.DisplayName ?? "", site.Host ?? "", site.Port, site.ApiKey ?? "");
        }

        // Show/hide network section based on user role
        var isAdmin = _mainViewModel.CurrentUser?.Role == UserRole.Admin;
        SetNetworkSectionVisibility(isAdmin);
    }

    private void SetNetworkSectionVisibility(bool visible)
    {
        // Hide entire network section including headers for non-admin users
        _networkSectionHeader!.Visible = visible;
        _networkRow!.Visible = visible;
        _networkEnabledCheckBox!.Visible = visible;
        _networkPortNumeric!.Visible = visible;
        _apiKeyTextBox!.Visible = visible;
        _generateApiKeyButton!.Visible = visible;

        // Remote sites section
        _remoteSitesSectionHeader!.Visible = visible;
        _remoteSitesGrid!.Visible = visible;
        _remoteSitesButtonPanel!.Visible = visible;
        _addRemoteSiteButton!.Visible = visible;
        _removeRemoteSiteButton!.Visible = visible;
    }

    private string GetSettingValue(Dictionary<string, string> settings, string key, string defaultValue = "")
    {
        return settings.TryGetValue(key, out var value) ? value : defaultValue;
    }

    private async Task SaveSettingsFromFormAsync()
    {
        var updates = new Dictionary<string, string>
        {
            { SettingKeys.SiteDisplayName, _siteDisplayNameTextBox!.Text },
            { SettingKeys.PrimaryLanguage, _languageComboBox!.SelectedItem?.ToString() ?? "el-GR" },
            { SettingKeys.OrgBusinessTitle, _orgTitleTextBox!.Text },
            { SettingKeys.OrgBusinessTel, _orgPhoneTextBox!.Text },
            { SettingKeys.IdleTimeoutMinutes, ((int)_idleTimeoutNumeric!.Value).ToString() },
            { SettingKeys.BadgeValidityMonths, ((int)_badgeValidityNumeric!.Value).ToString() },
            { SettingKeys.BadgeExpiryWarningDays, ((int)_badgeExpiryWarningNumeric!.Value).ToString() },
            { SettingKeys.LogLevel, _logLevelComboBox!.SelectedItem?.ToString() ?? "Information" },
            { SettingKeys.LogRetentionDays, ((int)_logRetentionNumeric!.Value).ToString() },
            { SettingKeys.AutoBackupEnabled, _autoBackupCheckBox!.Checked ? "1" : "0" },
            { SettingKeys.AutoBackupIncludePhotos, _backupIncludePhotosCheckBox!.Checked ? "1" : "0" },
            { SettingKeys.AutoBackupIntervalHours, ((int)_backupIntervalNumeric!.Value).ToString() },
            { SettingKeys.AutoBackupRetentionCount, ((int)_backupRetentionNumeric!.Value).ToString() },
            { SettingKeys.AutoBackupPath, _backupPathTextBox!.Text },
            { SettingKeys.AuditRetentionYears, ((int)_auditRetentionNumeric!.Value).ToString() },
            { SettingKeys.VisitRetentionYears, ((int)_visitRetentionNumeric!.Value).ToString() },
            { SettingKeys.BadgeOfficeName, _badgeOfficeNameTextBox!.Text },
            { SettingKeys.BadgeOfficePhone, _badgeOfficePhoneTextBox!.Text },
            { SettingKeys.BadgeOfficeHeadFullName, _badgeOfficeHeadFullNameTextBox!.Text },
            { SettingKeys.BadgeOfficeHeadJobTitle, _badgeOfficeHeadJobTitleTextBox!.Text },
            { SettingKeys.BadgeOfficeHeadRank, _badgeOfficeHeadRankTextBox!.Text },
            { SettingKeys.BadgeOfficeNotes, _badgeOfficeNotesTextBox!.Text },
            { SettingKeys.NetworkEnabled, _networkEnabledCheckBox!.Checked ? "1" : "0" },
            { SettingKeys.NetworkPort, ((int)_networkPortNumeric!.Value).ToString() },
            { SettingKeys.NetworkApiKey, _apiKeyTextBox!.Text }
        };

        // Update the Settings list in the view model
        foreach (var kvp in updates)
        {
            var existing = _settingsViewModel.Settings.FirstOrDefault(s => s.Key == kvp.Key);
            if (existing != null)
            {
                existing.Value = kvp.Value;
            }
            else
            {
                _settingsViewModel.Settings.Add(new Core.Models.Setting { Key = kvp.Key, Value = kvp.Value });
            }
        }

        await _settingsViewModel.SaveAsync();

        // Save remote sites to database (admin only)
        if (_mainViewModel.CurrentUser?.Role == UserRole.Admin)
        {
            var remoteSiteService = _serviceProvider.GetRequiredService<IRemoteSiteService>();
            var existingSites = await remoteSiteService.GetAllAsync();

            // Track which sites are in the grid
            var gridSiteCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (DataGridViewRow row in _remoteSitesGrid!.Rows)
            {
                if (row.IsNewRow) continue;

                var siteCode = row.Cells["SiteCode"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(siteCode)) continue;

                gridSiteCodes.Add(siteCode);

                var existingSite = existingSites.FirstOrDefault(s =>
                    string.Equals(s.SiteCode, siteCode, StringComparison.OrdinalIgnoreCase));

                var site = new Core.Models.RemoteSite
                {
                    Id = existingSite?.Id ?? 0,
                    SiteCode = siteCode,
                    DisplayName = row.Cells["DisplayName"].Value?.ToString(),
                    Host = row.Cells["Host"].Value?.ToString() ?? "localhost",
                    Port = int.TryParse(row.Cells["Port"].Value?.ToString(), out var p) ? p : 8080,
                    ApiKey = row.Cells["ApiKey"].Value?.ToString() ?? ""
                };

                if (existingSite != null)
                {
                    await remoteSiteService.UpdateAsync(site);
                }
                else
                {
                    await remoteSiteService.CreateAsync(site);
                }
            }

            // Remove sites that are no longer in the grid
            foreach (var existingSite in existingSites)
            {
                if (!gridSiteCodes.Contains(existingSite.SiteCode))
                {
                    await remoteSiteService.DeleteAsync(existingSite.Id);
                }
            }
        }

        UpdateStatus(Strings.Status_SettingsSaved);
    }
}
