using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Helpers;
using Pylae.Desktop.Resources;
using Pylae.Desktop.Services;
using Pylae.Desktop.ViewModels;
using Pylae.Sync.Hosting;

namespace Pylae.Desktop.Forms;

public partial class MainForm : Form
{
    private readonly MainViewModel _mainViewModel;
    private readonly GateViewModel _gateViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly MembersViewModel _membersViewModel;
    private readonly VisitsViewModel _visitsViewModel;
    private readonly AuditLogViewModel _auditLogViewModel;
    private readonly SyncServer _syncServer;
    private readonly SyncServerOptions _syncOptions;
    private readonly IExportService _exportService;
    private readonly IBadgeRenderer _badgeRenderer;
    private readonly IServiceProvider _serviceProvider;
    private readonly CatalogsViewModel _catalogsViewModel;
    private readonly UsersViewModel _usersViewModel;
    private readonly CurrentUserService _currentUserService;
    private readonly IBackupService _backupService;
    private readonly IAppSettings _appSettings;

    private BindingList<Setting> _settingsBinding = new();
    private BindingList<Member>? _membersBinding;
    private BindingList<Visit>? _visitsBinding;
    private BindingList<AuditEntry>? _auditBinding;
    private BindingList<User>? _usersBinding;
    private VisitDirection? _visitsDirectionFilter;
    private string? _visitsSearchText;

    // Lazy loading flags for tabs
    private bool _membersLoaded;
    private bool _visitsLoaded;
    private bool _auditLoaded;
    private bool _usersLoaded;
    private bool _settingsLoaded;

    public MainForm(
        MainViewModel mainViewModel,
        GateViewModel gateViewModel,
        SettingsViewModel settingsViewModel,
        MembersViewModel membersViewModel,
        VisitsViewModel visitsViewModel,
        AuditLogViewModel auditLogViewModel,
        SyncServer syncServer,
        SyncServerOptions syncOptions,
        IExportService exportService,
        IBadgeRenderer badgeRenderer,
        IServiceProvider serviceProvider,
        CatalogsViewModel catalogsViewModel,
        UsersViewModel usersViewModel,
        CurrentUserService currentUserService,
        IBackupService backupService,
        IAppSettings appSettings)
    {
        InitializeComponent();
        _mainViewModel = mainViewModel;
        _gateViewModel = gateViewModel;
        _settingsViewModel = settingsViewModel;
        _membersViewModel = membersViewModel;
        _visitsViewModel = visitsViewModel;
        _auditLogViewModel = auditLogViewModel;
        _syncServer = syncServer;
        _syncOptions = syncOptions;
        _exportService = exportService;
        _badgeRenderer = badgeRenderer;
        _serviceProvider = serviceProvider;
        _catalogsViewModel = catalogsViewModel;
        _usersViewModel = usersViewModel;
        _currentUserService = currentUserService;
        _backupService = backupService;
        _appSettings = appSettings;

        Text = $"{Strings.App_Title} - {Strings.App_Subtitle}";
        welcomeLabel.Text = Strings.Main_Welcome;

        // Wire up data binding events to localize grid column headers
        visitsGrid.DataBindingComplete += OnVisitsGridDataBindingComplete;
        visitsGrid.CellFormatting += OnVisitsGridCellFormatting;
        auditGrid.DataBindingComplete += OnAuditGridDataBindingComplete;

        // Wire up recent logs grid selection
        recentLogsGrid.SelectionChanged += OnRecentLogsGridSelectionChanged;

        // Show gate panel by default
        ShowPanel(gatePanel);
    }

    public void SetCurrentUser(User user)
    {
        _mainViewModel.CurrentUser = user;
        _currentUserService.CurrentUser = user;
        UpdateWelcomeLabel(user.LastName);

        // Enable/disable admin menu based on role
        var isAdmin = user.Role == UserRole.Admin;
        adminMenu.Enabled = isAdmin;
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Set up UI elements synchronously (no data loading)
        SetupSettingsFormView();
        SetupMembersMasterDetailView();
        SetupUsersMasterDetailView();

        // Enable double buffering for better grid performance
        visitsGrid.EnableDoubleBuffering();
        auditGrid.EnableDoubleBuffering();

        // Initialize date pickers with default values
        visitsFromPicker.Value = DateTime.Today.AddDays(-7);
        visitsToPicker.Value = DateTime.Today;
        auditFromPicker.Value = DateTime.Today.AddDays(-7);
        auditToPicker.Value = DateTime.Today;

        // Refresh date pickers to use app locale
        RefreshDatePickerFormats();
    }

    protected override async void OnShown(EventArgs e)
    {
        base.OnShown(e);

        // Initialize after form is visible - prevents UI blocking during first DB access
        await _mainViewModel.InitializeAsync();
        siteLabel.Text = _mainViewModel.SiteDisplayName ?? _mainViewModel.SiteCode ?? "";
        UpdateWelcomeLabel(_mainViewModel.CurrentUser?.LastName);

        // Refresh date pickers again after culture is loaded
        RefreshDatePickerFormats();

        // Load recent visits for the gate panel
        _ = RefreshRecentLogsAsync();

        // Start sync server in background
        _ = TryStartSyncAsync();
    }

    private async Task PopulateMemberComboBoxes()
    {
        // Load catalogs (member types)
        await _catalogsViewModel.LoadAsync();

        _memberTypeComboBox!.DataSource = _catalogsViewModel.MemberTypes.ToList();
        _memberTypeComboBox.DisplayMember = "DisplayName";
        _memberTypeComboBox.ValueMember = "Id";
        _memberTypeComboBox.SelectedIndex = -1;
    }

    private async Task TryStartSyncAsync()
    {
        _syncOptions.NetworkEnabled = _appSettings.GetBool(Core.Constants.SettingKeys.NetworkEnabled);
        _syncOptions.ApiKey = _appSettings.GetValue(Core.Constants.SettingKeys.NetworkApiKey);
        _syncOptions.Port = _appSettings.GetInt(Core.Constants.SettingKeys.NetworkPort, _syncOptions.Port);
        _syncOptions.SiteCode = _mainViewModel.SiteCode;
        _syncOptions.SiteDisplayName = _mainViewModel.SiteDisplayName;

        await _syncServer.StartAsync(_syncOptions);
    }

    private async void OnLogVisitClick(object sender, EventArgs e)
    {
        if (_mainViewModel.CurrentUser is null)
        {
            MessageBox.Show(Strings.App_UserNotAuthenticated, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _gateViewModel.MemberNumberInput = memberNumberTextBox.Text;
        _gateViewModel.Notes = notesTextBox.Text;
        _gateViewModel.SelectedDirection = entryRadio.Checked ? VisitDirection.Entry : VisitDirection.Exit;

        await _gateViewModel.LogVisitAsync(_mainViewModel.CurrentUser, _mainViewModel.SiteCode, Environment.MachineName);

        // Update status bar
        if (_gateViewModel.LastLoggedMember != null)
        {
            UpdateStatus(Strings.Status_VisitLogged);
        }

        // Update result text
        lastResultValueLabel.Text = _gateViewModel.LastResult ?? string.Empty;

        // Update direction and time display
        if (_gateViewModel.LastLoggedDirection.HasValue && _gateViewModel.LastLoggedTime.HasValue)
        {
            var isEntry = _gateViewModel.LastLoggedDirection == VisitDirection.Entry;
            var directionText = isEntry ? Strings.Gate_Entry : Strings.Gate_Exit;
            lastLogDirectionLabel.Text = directionText;
            lastLogDirectionLabel.ForeColor = isEntry ? Color.DarkGreen : Color.DarkRed;
            lastLogTimeLabel.Text = $"{Strings.Gate_Logged}: {_gateViewModel.LastLoggedTime.Value.ToLocalTime():HH:mm:ss}";
        }
        else
        {
            lastLogDirectionLabel.Text = string.Empty;
            lastLogTimeLabel.Text = string.Empty;
        }

        // Update member details display
        var member = _gateViewModel.LastLoggedMember;
        if (member != null)
        {
            // Rank on its own row
            lastMemberRankLabel.Text = member.BusinessRank ?? string.Empty;

            // First Last name
            lastMemberNameLabel.Text = $"{member.FirstName} {member.LastName}";

            // Status: PERMANENT or TEMPORARY (with label)
            var statusValue = member.IsPermanentStaff ? Strings.Member_PermanentStaff : Strings.Member_TemporaryStaff;
            lastMemberStatusLabel.Text = $"{Strings.Member_Status}: {statusValue}";

            // Badge Status: Active/Inactive (with label)
            var activeValue = member.IsActive ? Strings.Member_Active : Strings.Member_Inactive;
            lastMemberActiveLabel.Text = $"{Strings.Member_BadgeStatus}: {activeValue}";

            // Badge Type (with label)
            var typeValue = member.MemberType?.DisplayName ?? string.Empty;
            lastMemberTypeLabel.Text = !string.IsNullOrWhiteSpace(typeValue)
                ? $"{Strings.Member_BadgeType}: {typeValue}"
                : string.Empty;

            // Identification Number
            lastMemberPersonalIdLabel.Text = !string.IsNullOrWhiteSpace(member.PersonalIdNumber)
                ? $"{Strings.Member_IdentificationNumber}: {member.PersonalIdNumber}"
                : string.Empty;

            // Organization Identification
            lastMemberOrgIdLabel.Text = !string.IsNullOrWhiteSpace(member.BusinessIdNumber)
                ? $"{Strings.Member_OrganizationId}: {member.BusinessIdNumber}"
                : string.Empty;

            // Issue Date
            lastMemberIssueDateLabel.Text = member.BadgeIssueDate.HasValue
                ? $"{Strings.Member_BadgeIssue}: {member.BadgeIssueDate:d}"
                : string.Empty;

            // Expiry Date
            lastMemberExpiryDateLabel.Text = member.BadgeExpiryDate.HasValue
                ? $"{Strings.Member_BadgeExpiry}: {member.BadgeExpiryDate:d}"
                : string.Empty;

            // Photo
            LoadMemberPhoto(member);
        }
        else
        {
            // Clear member display on error
            lastMemberRankLabel.Text = string.Empty;
            lastMemberNameLabel.Text = string.Empty;
            lastMemberStatusLabel.Text = string.Empty;
            lastMemberActiveLabel.Text = string.Empty;
            lastMemberTypeLabel.Text = string.Empty;
            lastMemberPersonalIdLabel.Text = string.Empty;
            lastMemberOrgIdLabel.Text = string.Empty;
            lastMemberIssueDateLabel.Text = string.Empty;
            lastMemberExpiryDateLabel.Text = string.Empty;
            lastMemberPhotoBox.Image = null;
        }

        // Badge warning
        var badgeStatus = _gateViewModel.LastBadgeStatus;
        var warningText = _gateViewModel.BadgeWarning ?? string.Empty;
        badgeWarningLabel.Text = warningText;
        badgeWarningLabel.ForeColor = badgeStatus?.IsExpired == true
            ? Color.DarkRed
            : badgeStatus?.IsWarning == true
                ? Color.DarkOrange
                : SystemColors.ControlText;
        badgeWarningLabel.Visible = !string.IsNullOrWhiteSpace(warningText);

        // Refresh visits data for the visits panel
        await RefreshVisitsAsync();

        // Refresh recent logs grid
        await RefreshRecentLogsAsync();

        memberNumberTextBox.Text = string.Empty;
        notesTextBox.Text = string.Empty;
        memberNumberTextBox.Focus();
    }

    private void LoadMemberPhoto(Member member)
    {
        lastMemberPhotoBox.Image = null;
        if (string.IsNullOrEmpty(member.PhotoFileName)) return;

        var photoPath = member.PhotoFileName;
        if (!Path.IsPathRooted(photoPath))
        {
            var photosPath = _serviceProvider.GetRequiredService<Data.Context.DatabaseOptions>().GetPhotosPath();
            photoPath = Path.Combine(photosPath, photoPath);
        }

        if (File.Exists(photoPath))
        {
            try
            {
                lastMemberPhotoBox.Image = Image.FromFile(photoPath);
            }
            catch
            {
                lastMemberPhotoBox.Image = null;
            }
        }
    }

    private async Task RefreshVisitsAsync()
    {
        // Refresh visits data if binding exists
        if (_visitsBinding != null)
        {
            await _visitsViewModel.LoadAsync(visitsFromPicker.Value, visitsToPicker.Value);
            _visitsBinding = new BindingList<Visit>(_visitsViewModel.Visits);
            visitsGrid.DataSource = _visitsBinding;
        }
    }

    private async Task RefreshRecentLogsAsync()
    {
        // Load last 6 visits for the recent logs grid
        var todayStart = DateTime.UtcNow.Date;
        var todayEnd = todayStart.AddDays(1);
        await _visitsViewModel.LoadAsync(todayStart, todayEnd);

        var recentLogs = _visitsViewModel.Visits
            .OrderByDescending(v => v.TimestampUtc)
            .Take(6)
            .Select(v => new
            {
                DateTime = v.TimestampLocal,
                Rank = v.MemberBusinessRank ?? string.Empty,
                FirstName = v.MemberFirstName,
                LastName = v.MemberLastName,
                Id = v.MemberPersonalIdNumber ?? string.Empty,
                Direction = v.Direction == VisitDirection.Entry ? Strings.Gate_Entry : Strings.Gate_Exit
            })
            .ToList();

        recentLogsGrid.DataSource = recentLogs;

        // Set column headers and enable sorting
        if (recentLogsGrid.Columns.Count > 0)
        {
            if (recentLogsGrid.Columns["DateTime"] is { } dateTimeCol)
            {
                dateTimeCol.HeaderText = Strings.Grid_Timestamp;
                dateTimeCol.DefaultCellStyle.Format = "g"; // Short date + short time
                dateTimeCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            if (recentLogsGrid.Columns["Rank"] is { } rankCol)
            {
                rankCol.HeaderText = Strings.Member_BusinessRank;
                rankCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            if (recentLogsGrid.Columns["FirstName"] is { } firstNameCol)
            {
                firstNameCol.HeaderText = Strings.Member_FirstName;
                firstNameCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            if (recentLogsGrid.Columns["LastName"] is { } lastNameCol)
            {
                lastNameCol.HeaderText = Strings.Member_LastName;
                lastNameCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            if (recentLogsGrid.Columns["Id"] is { } idCol)
            {
                idCol.HeaderText = Strings.Member_IdentificationNumber;
                idCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            if (recentLogsGrid.Columns["Direction"] is { } dirCol)
            {
                dirCol.HeaderText = Strings.Gate_Direction;
                dirCol.SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }

        // Default sort by DateTime descending (newest first)
        recentLogsGrid.Sort(recentLogsGrid.Columns["DateTime"]!, System.ComponentModel.ListSortDirection.Descending);
    }

    private async void OnRecentLogsGridSelectionChanged(object? sender, EventArgs e)
    {
        if (recentLogsGrid.SelectedRows.Count == 0) return;

        // Get the selected visit from the underlying data
        var selectedIndex = recentLogsGrid.SelectedRows[0].Index;
        var recentVisits = _visitsViewModel.Visits
            .OrderByDescending(v => v.TimestampUtc)
            .Take(6)
            .ToList();

        if (selectedIndex < 0 || selectedIndex >= recentVisits.Count) return;

        var visit = recentVisits[selectedIndex];

        // Update the display using visit data (member data is embedded in visit)
        lastMemberRankLabel.Text = visit.MemberBusinessRank ?? string.Empty;
        lastMemberNameLabel.Text = $"{visit.MemberFirstName} {visit.MemberLastName}";

        var statusValue = visit.MemberIsPermanentStaff ? Strings.Member_PermanentStaff : Strings.Member_TemporaryStaff;
        lastMemberStatusLabel.Text = $"{Strings.Member_Status}: {statusValue}";

        // Badge status not available in visit, show type instead
        lastMemberActiveLabel.Text = string.Empty;

        var typeValue = visit.MemberTypeName ?? string.Empty;
        lastMemberTypeLabel.Text = !string.IsNullOrWhiteSpace(typeValue)
            ? $"{Strings.Member_BadgeType}: {typeValue}"
            : string.Empty;

        lastMemberPersonalIdLabel.Text = !string.IsNullOrWhiteSpace(visit.MemberPersonalIdNumber)
            ? $"{Strings.Member_IdentificationNumber}: {visit.MemberPersonalIdNumber}"
            : string.Empty;

        lastMemberOrgIdLabel.Text = !string.IsNullOrWhiteSpace(visit.MemberBusinessIdNumber)
            ? $"{Strings.Member_OrganizationId}: {visit.MemberBusinessIdNumber}"
            : string.Empty;

        // Issue/expiry dates not available in visit
        lastMemberIssueDateLabel.Text = string.Empty;
        lastMemberExpiryDateLabel.Text = string.Empty;

        // Update direction and time from the visit
        var isEntry = visit.Direction == VisitDirection.Entry;
        lastLogDirectionLabel.Text = isEntry ? Strings.Gate_Entry : Strings.Gate_Exit;
        lastLogDirectionLabel.ForeColor = isEntry ? Color.DarkGreen : Color.DarkRed;
        lastLogTimeLabel.Text = $"{Strings.Gate_Logged}: {visit.TimestampLocal:HH:mm:ss}";

        // Load member photo from database (not embedded in visit)
        await LoadMemberPhotoAsync(visit.MemberId);
    }

    private async Task LoadMemberPhotoAsync(string memberId)
    {
        try
        {
            var member = await _membersViewModel.GetByIdAsync(memberId);
            if (member?.PhotoFileName != null)
            {
                var photosPath = _serviceProvider.GetRequiredService<Data.Context.DatabaseOptions>().GetPhotosPath();
                var photoPath = Path.Combine(photosPath, member.PhotoFileName);
                if (File.Exists(photoPath))
                {
                    using var fs = new FileStream(photoPath, FileMode.Open, FileAccess.Read);
                    lastMemberPhotoBox.Image = Image.FromStream(fs);
                    return;
                }
            }
        }
        catch
        {
            // Ignore photo loading errors
        }

        lastMemberPhotoBox.Image = null;
    }

    private void OnMemberNumberKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            OnLogVisitClick(sender!, e);
        }
    }

    private void OnMainFormKeyDown(object? sender, KeyEventArgs e)
    {
        // Only handle shortcuts when gate panel is visible
        if (!gatePanel.Visible) return;

        switch (e.KeyCode)
        {
            case Keys.F1:
                entryRadio.Checked = true;
                e.Handled = true;
                break;
            case Keys.F2:
                exitRadio.Checked = true;
                e.Handled = true;
                break;
            case Keys.F5:
                OnLogVisitClick(sender!, e);
                e.Handled = true;
                break;
        }
    }

    private async void OnSaveSettingsClick(object? sender, EventArgs e)
    {
        await SaveSettingsFromFormAsync();
    }

    private async void OnBackupClick(object? sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "Zip files (*.zip)|*.zip",
            FileName = $"pylae_backup_{DateTime.Now:yyyyMMddHHmm}.zip"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            Cursor.Current = Cursors.WaitCursor;
            var bytes = await _backupService.CreateBackupAsync(includePhotos: true);
            await File.WriteAllBytesAsync(dialog.FileName, bytes);
            UpdateStatus(Strings.Status_BackupCreated);
            MessageBox.Show(Strings.Backup_Saved, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch
        {
            MessageBox.Show(Strings.Backup_Failed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor.Current = Cursors.Default;
        }
    }

    private async void OnRestoreClick(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Zip files (*.zip)|*.zip",
            Title = Strings.Button_Restore
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (MessageBox.Show(Strings.Backup_RestoreConfirm, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        try
        {
            Cursor.Current = Cursors.WaitCursor;
            var result = await _backupService.RestoreBackupAsync(dialog.FileName);
            if (!result.IsValid)
            {
                var message = result.Reason is null
                    ? Strings.Backup_Invalid
                    : string.Format(Strings.Backup_InvalidDetail, result.Reason);
                MessageBox.Show(message, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            UpdateStatus(Strings.Status_BackupRestored);
            MessageBox.Show(Strings.Backup_Restored, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (MessageBox.Show(Strings.Backup_RestartPrompt, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Restart();
            }
        }
        catch
        {
            MessageBox.Show(Strings.Backup_Failed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor.Current = Cursors.Default;
        }
    }

    private async void OnRefreshVisitsClick(object sender, EventArgs e)
    {
        // Use filter dates, converting local to UTC for database query
        await FilterAndRefreshVisits();
    }

    private async void OnFilterVisitsClick(object sender, EventArgs e)
    {
        await FilterAndRefreshVisits();
    }

    private async void OnFilterVisitsEntryClick(object sender, EventArgs e)
    {
        _visitsDirectionFilter = VisitDirection.Entry;
        await FilterAndRefreshVisits();
    }

    private async void OnFilterVisitsExitClick(object sender, EventArgs e)
    {
        _visitsDirectionFilter = VisitDirection.Exit;
        await FilterAndRefreshVisits();
    }

    private async void OnFilterVisitsAllClick(object sender, EventArgs e)
    {
        _visitsDirectionFilter = null;
        await FilterAndRefreshVisits();
    }

    private async void OnVisitsSearchTextChanged(object? sender, EventArgs e)
    {
        _visitsSearchText = visitsSearchTextBox.Text.Trim();
        await FilterAndRefreshVisits();
    }

    private async Task FilterAndRefreshVisits()
    {
        // Convert local date picker values to UTC for database query
        // fromUtc: Start of day in local time, converted to UTC
        var fromLocal = DateTime.SpecifyKind(visitsFromPicker.Value.Date, DateTimeKind.Local);
        var fromUtc = fromLocal.ToUniversalTime();

        // toUtc: End of day in local time, converted to UTC
        var toLocal = DateTime.SpecifyKind(visitsToPicker.Value.Date, DateTimeKind.Local).AddDays(1).AddTicks(-1);
        var toUtc = toLocal.ToUniversalTime();

        await _visitsViewModel.LoadAsync(fromUtc, toUtc);

        // Apply direction filter if set
        IEnumerable<Visit> filteredVisits = _visitsDirectionFilter.HasValue
            ? _visitsViewModel.Visits.Where(v => v.Direction == _visitsDirectionFilter.Value)
            : _visitsViewModel.Visits;

        // Apply search filter if text is provided
        if (!string.IsNullOrWhiteSpace(_visitsSearchText))
        {
            var searchLower = _visitsSearchText.ToLowerInvariant();
            filteredVisits = filteredVisits.Where(v =>
                (v.MemberFirstName?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.MemberLastName?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.MemberBusinessRank?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.MemberOfficeName?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.MemberTypeName?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.MemberPersonalIdNumber?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.MemberBusinessIdNumber?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                v.MemberNumber.ToString().Contains(searchLower) ||
                (v.SiteCode?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.Username?.ToLowerInvariant().Contains(searchLower) ?? false) ||
                (v.UserDisplayName?.ToLowerInvariant().Contains(searchLower) ?? false));
        }

        // Sort by timestamp descending (newest first)
        var resultList = filteredVisits.OrderByDescending(v => v.TimestampLocal).ToList();

        _visitsBinding = new BindingList<Visit>(resultList);
        visitsGrid.DataSource = _visitsBinding;
        UpdateStatus($"{Strings.Status_VisitsLoaded}: {resultList.Count}");
    }

    private async void visitsGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 || _mainViewModel.CurrentUser is null)
        {
            return;
        }

        var visit = visitsGrid.Rows[e.RowIndex].DataBoundItem as Visit;
        if (visit is null)
        {
            return;
        }

        var input = Microsoft.VisualBasic.Interaction.InputBox(Strings.Visits_UpdateNotesPrompt, Strings.Visits_UpdateNotesTitle, visit.Notes ?? string.Empty);
        await _visitsViewModel.UpdateNotesAsync(visit.Id, string.IsNullOrWhiteSpace(input) ? null : input, _mainViewModel.CurrentUser.Id);
        await _visitsViewModel.LoadAsync(visitsFromPicker.Value, visitsToPicker.Value);
        _visitsBinding = new BindingList<Visit>(_visitsViewModel.Visits);
        visitsGrid.DataSource = _visitsBinding;
    }

    private async void OnMenuRemoteSitesClick(object? sender, EventArgs e)
    {
        var form = _serviceProvider.GetRequiredService<RemoteSitesForm>();
        await form.ShowDialogAsync(this);
    }

    private async void OnResetPasswordClick(object? sender, EventArgs e)
    {
        if (!EnsureAdmin())
        {
            return;
        }

        var user = GetSelectedUser();
        if (user is null)
        {
            MessageBox.Show(Strings.Users_SelectUser, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var input = Microsoft.VisualBasic.Interaction.InputBox(Strings.Users_ResetPasswordPrompt, Strings.App_Title, string.Empty);
        if (string.IsNullOrWhiteSpace(input))
        {
            return;
        }

        await _usersViewModel.ChangePasswordAsync(user.Id, input);
        MessageBox.Show(Strings.Users_PasswordUpdated, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private async void OnResetQuickCodeClick(object? sender, EventArgs e)
    {
        if (!EnsureAdmin())
        {
            return;
        }

        var user = GetSelectedUser();
        if (user is null)
        {
            MessageBox.Show(Strings.Users_SelectUser, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (user.Role == UserRole.Admin)
        {
            MessageBox.Show(Strings.Users_QuickCodeNotAllowed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var input = Microsoft.VisualBasic.Interaction.InputBox(Strings.Users_ResetQuickCodePrompt, Strings.App_Title, string.Empty);
        var quickCode = string.IsNullOrWhiteSpace(input) ? null : input.Trim();
        await _usersViewModel.SetQuickCodeAsync(user.Id, quickCode);
        MessageBox.Show(Strings.Users_QuickCodeUpdated, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private User? GetSelectedUser()
    {
        return usersGrid.CurrentRow?.DataBoundItem as User;
    }

    private bool EnsureAdmin()
    {
        if (_mainViewModel.CurrentUser?.Role == UserRole.Admin)
        {
            return true;
        }

        MessageBox.Show(Strings.Users_AdminOnly, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        return false;
    }

    private async void OnRefreshAuditClick(object sender, EventArgs e)
    {
        await _auditLogViewModel.LoadAsync(auditFromPicker.Value, auditToPicker.Value, auditActionText.Text, auditTargetText.Text);
        _auditBinding = new BindingList<AuditEntry>(_auditLogViewModel.Entries);
        auditGrid.DataSource = _auditBinding;
    }

    private async void OnExportAuditClick(object sender, EventArgs e)
    {
        if (_auditBinding is null)
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx|JSON files (*.json)|*.json",
            FileName = "audit.xlsx"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var bytes = dialog.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? await _exportService.ExportAuditJsonAsync(_auditBinding)
                : await _exportService.ExportAuditAsync(_auditBinding);
            await File.WriteAllBytesAsync(dialog.FileName, bytes);
            MessageBox.Show(Strings.Export_AuditSaved, Strings.App_Title);
        }
    }

    private async void OnExportMembersClick(object? sender, EventArgs e)
    {
        if (_membersBinding is null)
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx|JSON files (*.json)|*.json",
            FileName = "members.xlsx"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var bytes = dialog.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? await _exportService.ExportMembersJsonAsync(_membersBinding)
                : await _exportService.ExportMembersAsync(_membersBinding);
            await File.WriteAllBytesAsync(dialog.FileName, bytes);
            UpdateStatus(Strings.Status_ExportComplete);
            MessageBox.Show(Strings.Export_MembersSaved, Strings.App_Title);
        }
    }

    private async void OnExportVisitsClick(object sender, EventArgs e)
    {
        if (_visitsBinding is null)
        {
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "Excel files (*.xlsx)|*.xlsx|JSON files (*.json)|*.json",
            FileName = "visits.xlsx"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var bytes = dialog.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                ? await _exportService.ExportVisitsJsonAsync(_visitsBinding)
                : await _exportService.ExportVisitsAsync(_visitsBinding);
            await File.WriteAllBytesAsync(dialog.FileName, bytes);
            UpdateStatus(Strings.Status_ExportComplete);
            MessageBox.Show(Strings.Export_VisitsSaved, Strings.App_Title);
        }
    }

    private async void OnBadgeClick(object? sender, EventArgs e)
    {
        var member = membersGrid.CurrentRow?.DataBoundItem as Member;
        if (member is null)
        {
            MessageBox.Show(Strings.Members_SelectFirst, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var safeName = string.Join("_", member.LastName.Split(Path.GetInvalidFileNameChars()));
        using var dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"{member.MemberNumber}_{safeName}.pdf"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var pdf = await _badgeRenderer.RenderBadgeAsync(member);
            await File.WriteAllBytesAsync(dialog.FileName, pdf);
            MessageBox.Show(Strings.Badge_PdfSaved, Strings.App_Title);
        }
    }

    private async void OnBatchBadgeClick(object? sender, EventArgs e)
    {
        // Get selected members from grid
        var selectedMembers = new List<Member>();
        foreach (DataGridViewRow row in membersGrid.SelectedRows)
        {
            if (row.DataBoundItem is Member member)
            {
                selectedMembers.Add(member);
            }
        }

        if (selectedMembers.Count == 0)
        {
            MessageBox.Show(Strings.Members_SelectFirst, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"badges_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                var pdf = await _badgeRenderer.RenderBatchBadgesAsync(selectedMembers);
                await File.WriteAllBytesAsync(dialog.FileName, pdf);
                MessageBox.Show(string.Format(Strings.Badge_BatchSaved, selectedMembers.Count), Strings.App_Title);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Strings.Badge_PdfFailed}: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private async void OnOpenCatalogsClick(object? sender, EventArgs e)
    {
        var form = _serviceProvider.GetRequiredService<CatalogsForm>();
        await form.ShowDialogAsync(this);
        await _catalogsViewModel.LoadAsync();
        await _membersViewModel.LoadAsync();
        _membersBinding = new BindingList<Member>(_membersViewModel.Members);
        membersGrid.DataSource = _membersBinding;
    }

    private async Task LoadUsersAsync()
    {
        if (_mainViewModel.CurrentUser?.Role != UserRole.Admin)
        {
            return;
        }

        await _usersViewModel.LoadAsync();
        _usersBinding = new BindingList<User>(_usersViewModel.Users);
        usersGrid.DataSource = _usersBinding;
    }

    /// <summary>
    /// Event handler to set localized column headers for Visits grid
    /// </summary>
    private void OnVisitsGridDataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        if (visitsGrid.Columns.Count == 0) return;

        // Map property names to localized headers
        var columnMappings = new Dictionary<string, string>
        {
            { nameof(Visit.VisitGuid), Strings.Grid_Id },
            { nameof(Visit.MemberId), Strings.Grid_MemberId },
            { nameof(Visit.MemberNumber), Strings.Grid_MemberNumber },
            { nameof(Visit.MemberFirstName), Strings.Grid_FirstName },
            { nameof(Visit.MemberLastName), Strings.Grid_LastName },
            { nameof(Visit.MemberBusinessRank), Strings.Grid_BusinessRank },
            { nameof(Visit.MemberOfficeName), Strings.Grid_Office },
            { nameof(Visit.MemberIsPermanentStaff), Strings.Grid_IsPermanent },
            { nameof(Visit.MemberTypeCode), Strings.Grid_MemberTypeId },
            { nameof(Visit.MemberTypeName), Strings.Grid_MemberType },
            { nameof(Visit.MemberPersonalIdNumber), Strings.Grid_PersonalIdNumber },
            { nameof(Visit.MemberBusinessIdNumber), Strings.Grid_BusinessIdNumber },
            { nameof(Visit.Direction), Strings.Grid_VisitType },
            { nameof(Visit.TimestampUtc), Strings.Grid_Timestamp },
            { nameof(Visit.TimestampLocal), Strings.Grid_Timestamp },
            { nameof(Visit.Method), Strings.Grid_Method },
            { nameof(Visit.SiteCode), Strings.Grid_SiteCode },
            { nameof(Visit.UserId), Strings.Grid_UserId },
            { nameof(Visit.Username), Strings.Grid_LoggedBy },
            { nameof(Visit.UserDisplayName), Strings.Grid_Username },
            { nameof(Visit.WorkstationId), Strings.Grid_Workstation },
            { nameof(Visit.Notes), Strings.Grid_Notes }
        };

        // Hide columns not needed for users (includes MemberTypeCode which is the badge type ID)
        var hiddenColumns = new[] { "Id", "VisitGuid", "MemberId", "UserId", "Username", "TimestampUtc", "MemberTypeCode" };

        foreach (DataGridViewColumn column in visitsGrid.Columns)
        {
            if (columnMappings.TryGetValue(column.DataPropertyName, out var localizedHeader))
            {
                column.HeaderText = localizedHeader;
            }

            if (hiddenColumns.Contains(column.DataPropertyName))
            {
                column.Visible = false;
            }

            // Enable sorting on all columns
            column.SortMode = DataGridViewColumnSortMode.Automatic;
        }
    }

    private void OnVisitsGridCellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        var columnName = visitsGrid.Columns[e.ColumnIndex].DataPropertyName;

        if (columnName == nameof(Visit.Direction) && e.Value is VisitDirection direction)
        {
            e.Value = direction == VisitDirection.Entry ? Strings.Gate_Entry : Strings.Gate_Exit;
            e.FormattingApplied = true;
        }
        else if (columnName == nameof(Visit.Method) && e.Value is VisitMethod method)
        {
            e.Value = method == VisitMethod.Scan ? Strings.Visit_MethodScan : Strings.Visit_MethodManual;
            e.FormattingApplied = true;
        }
    }

    /// <summary>
    /// Event handler to set localized column headers for Audit grid
    /// </summary>
    private void OnAuditGridDataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        if (auditGrid.Columns.Count == 0) return;

        // Map property names to localized headers
        var columnMappings = new Dictionary<string, string>
        {
            { nameof(AuditEntry.Id), Strings.Grid_Id },
            { nameof(AuditEntry.SiteCode), Strings.Grid_SiteCode },
            { nameof(AuditEntry.ActionType), Strings.Grid_Action },
            { nameof(AuditEntry.TargetType), Strings.Grid_EntityType },
            { nameof(AuditEntry.TargetId), Strings.Grid_EntityId },
            { nameof(AuditEntry.DetailsJson), Strings.Grid_Changes },
            { nameof(AuditEntry.TimestampUtc), Strings.Grid_Timestamp },
            { nameof(AuditEntry.Username), Strings.Grid_LoggedBy },
            { nameof(AuditEntry.UserId), Strings.Grid_UserId }
        };

        foreach (DataGridViewColumn column in auditGrid.Columns)
        {
            if (columnMappings.TryGetValue(column.DataPropertyName, out var localizedHeader))
            {
                column.HeaderText = localizedHeader;
            }

            // Enable sorting on all columns
            column.SortMode = DataGridViewColumnSortMode.Automatic;
        }
    }

    /// <summary>
    /// Refresh DateTimePicker controls to use current culture format
    /// </summary>
    private void RefreshDatePickerFormats()
    {
        // Force DateTimePickers to use selected UI culture's date format
        var culture = System.Threading.Thread.CurrentThread.CurrentUICulture;
        var format = culture.DateTimeFormat.ShortDatePattern;

        visitsFromPicker.Format = DateTimePickerFormat.Custom;
        visitsFromPicker.CustomFormat = format;
        visitsToPicker.Format = DateTimePickerFormat.Custom;
        visitsToPicker.CustomFormat = format;

        auditFromPicker.Format = DateTimePickerFormat.Custom;
        auditFromPicker.CustomFormat = format;
        auditToPicker.Format = DateTimePickerFormat.Custom;
        auditToPicker.CustomFormat = format;

        // Also refresh Members date pickers if they exist
        if (_memberBadgeIssuePicker != null)
        {
            _memberBadgeIssuePicker.CustomFormat = format;
            _memberBadgeExpiryPicker!.CustomFormat = format;
            _memberDateOfBirthPicker!.CustomFormat = format;
        }
    }

    #region Panel Navigation

    private void ShowPanel(Panel panelToShow)
    {
        gatePanel.Visible = panelToShow == gatePanel;
        membersPanel.Visible = panelToShow == membersPanel;
        usersPanel.Visible = panelToShow == usersPanel;
        visitsPanel.Visible = panelToShow == visitsPanel;
        settingsPanel.Visible = panelToShow == settingsPanel;
        auditPanel.Visible = panelToShow == auditPanel;

        // Lazy load data for the selected panel
        if (panelToShow == membersPanel && !_membersLoaded)
        {
            _ = LoadMembersPanelAsync();
        }
        else if (panelToShow == visitsPanel && !_visitsLoaded)
        {
            _ = LoadVisitsPanelAsync();
        }
        else if (panelToShow == auditPanel && !_auditLoaded)
        {
            _ = LoadAuditPanelAsync();
        }
        else if (panelToShow == usersPanel && !_usersLoaded)
        {
            _ = LoadUsersPanelAsync();
        }
        else if (panelToShow == settingsPanel && !_settingsLoaded)
        {
            _ = LoadSettingsPanelAsync();
        }
    }

    private async Task LoadMembersPanelAsync()
    {
        _membersLoaded = true;

        // Load members and catalogs in parallel
        var membersTask = _membersViewModel.LoadAsync();
        var catalogsTask = _catalogsViewModel.LoadAsync();
        await Task.WhenAll(membersTask, catalogsTask);

        _membersBinding = new BindingList<Member>(_membersViewModel.Members);
        membersGrid.DataSource = _membersBinding;

        // Populate combo boxes
        _memberTypeComboBox!.DataSource = _catalogsViewModel.MemberTypes.ToList();
        _memberTypeComboBox.DisplayMember = "DisplayName";
        _memberTypeComboBox.ValueMember = "Id";
        _memberTypeComboBox.SelectedIndex = -1;

        // Clear selection to start in "new member" mode
        membersGrid.ClearSelection();
        membersGrid.CurrentCell = null;
        ClearMemberForm();

        await InvokeAsync(() => _memberFirstNameTextBox?.Focus());
    }

    private async Task LoadVisitsPanelAsync()
    {
        _visitsLoaded = true;
        await _visitsViewModel.LoadAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        _visitsBinding = new BindingList<Visit>(_visitsViewModel.Visits);
        visitsGrid.DataSource = _visitsBinding;
    }

    private async Task LoadAuditPanelAsync()
    {
        _auditLoaded = true;
        await _auditLogViewModel.LoadAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, null, null);
        _auditBinding = new BindingList<AuditEntry>(_auditLogViewModel.Entries);
        auditGrid.DataSource = _auditBinding;
    }

    private async Task LoadUsersPanelAsync()
    {
        _usersLoaded = true;
        await LoadUsersAsync();
    }

    private async Task LoadSettingsPanelAsync()
    {
        _settingsLoaded = true;
        await _settingsViewModel.LoadAsync();
        await LoadSettingsToFormAsync();
    }

    private void UpdateWelcomeLabel(string? lastName)
    {
        var site = _mainViewModel.SiteDisplayName ?? _mainViewModel.SiteCode ?? "";
        if (string.IsNullOrEmpty(lastName))
        {
            welcomeLabel.Text = site;
        }
        else
        {
            welcomeLabel.Text = $"{lastName} | {site}";
        }
    }

    #endregion

    #region Menu Event Handlers

    private void OnMenuMainPageClick(object? sender, EventArgs e) => ShowPanel(gatePanel);

    private void OnMenuVisitsClick(object? sender, EventArgs e) => ShowPanel(visitsPanel);

    private void OnMenuLogoutClick(object? sender, EventArgs e)
    {
        // Clear current user
        _mainViewModel.CurrentUser = null;
        _currentUserService.CurrentUser = null;

        // Hide main form and show login as dialog
        Hide();

        using var loginForm = _serviceProvider.GetRequiredService<LoginForm>();
        loginForm.DialogMode = true; // Prevents creating new MainForm instance

        if (loginForm.ShowDialog() == DialogResult.OK && _currentUserService.CurrentUser != null)
        {
            // User logged in successfully - update and show main form
            SetCurrentUser(_currentUserService.CurrentUser);
            ResetFormState();
            Show();
        }
        else
        {
            // User cancelled login - close application
            Close();
        }
    }

    private void ResetFormState()
    {
        // Reset lazy loading flags so data reloads for new user
        _membersLoaded = false;
        _visitsLoaded = false;
        _auditLoaded = false;
        _usersLoaded = false;
        _settingsLoaded = false;

        // Return to gate panel
        ShowPanel(gatePanel);

        // Refresh recent logs for gate view
        _ = RefreshRecentLogsAsync();
    }

    private void OnMenuExitClick(object? sender, EventArgs e) => Close();

    private void OnMenuBadgesClick(object? sender, EventArgs e) => ShowPanel(membersPanel);

    private void OnMenuUsersClick(object? sender, EventArgs e) => ShowPanel(usersPanel);

    private void OnMenuSettingsClick(object? sender, EventArgs e) => ShowPanel(settingsPanel);

    private void OnMenuAuditClick(object? sender, EventArgs e) => ShowPanel(auditPanel);

    private void OnMenuUserGuideClick(object? sender, EventArgs e)
    {
        MessageBox.Show(Strings.Help_UserGuideNotAvailable, Strings.Menu_UserGuide, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnMenuAdminGuideClick(object? sender, EventArgs e)
    {
        MessageBox.Show(Strings.Help_AdminGuideNotAvailable, Strings.Menu_AdminGuide, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void OnMenuAboutClick(object? sender, EventArgs e)
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0";
        MessageBox.Show(
            $"{Strings.App_Title}\n{Strings.App_Subtitle}\n\n{Strings.About_Version}: {version}\n\n© 2024 Pylae",
            Strings.Menu_About,
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    #endregion

    #region Status Bar

    /// <summary>
    /// Updates the status bar with a message. Success messages auto-clear after a delay.
    /// </summary>
    private void UpdateStatus(string message, bool isSuccess = true)
    {
        statusLabel.Text = message;
        statusLabel.ForeColor = isSuccess ? Color.DarkGreen : Color.DarkRed;

        if (isSuccess)
        {
            // Auto-clear success messages after 5 seconds
            Task.Delay(5000).ContinueWith(_ =>
            {
                if (IsDisposed) return;
                BeginInvoke(() =>
                {
                    if (statusLabel.Text == message)
                    {
                        statusLabel.Text = Strings.Status_Ready;
                        statusLabel.ForeColor = SystemColors.ControlText;
                    }
                });
            });
        }
    }

    #endregion
}
