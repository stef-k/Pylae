using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
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
    private readonly ISettingsService _settingsService;
    private readonly IExportService _exportService;
    private readonly IBadgeRenderer _badgeRenderer;
    private readonly IServiceProvider _serviceProvider;
    private readonly CatalogsViewModel _catalogsViewModel;
    private readonly UsersViewModel _usersViewModel;
    private readonly CurrentUserService _currentUserService;
    private readonly IBackupService _backupService;

    private BindingList<Setting> _settingsBinding = new();
    private BindingList<Member>? _membersBinding;
    private BindingList<Visit>? _visitsBinding;
    private BindingList<AuditEntry>? _auditBinding;
    private BindingList<User>? _usersBinding;

    public MainForm(
        MainViewModel mainViewModel,
        GateViewModel gateViewModel,
        SettingsViewModel settingsViewModel,
        MembersViewModel membersViewModel,
        VisitsViewModel visitsViewModel,
        AuditLogViewModel auditLogViewModel,
        SyncServer syncServer,
        SyncServerOptions syncOptions,
        ISettingsService settingsService,
        IExportService exportService,
        IBadgeRenderer badgeRenderer,
        IServiceProvider serviceProvider,
        CatalogsViewModel catalogsViewModel,
        UsersViewModel usersViewModel,
        CurrentUserService currentUserService,
        IBackupService backupService)
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
        _settingsService = settingsService;
        _exportService = exportService;
        _badgeRenderer = badgeRenderer;
        _serviceProvider = serviceProvider;
        _catalogsViewModel = catalogsViewModel;
        _usersViewModel = usersViewModel;
        _currentUserService = currentUserService;
        _backupService = backupService;

        Text = $"{Strings.App_Title} - {Strings.App_Subtitle}";
        welcomeLabel.Text = Strings.Main_Welcome;

        // Wire up data binding events to localize grid column headers
        visitsGrid.DataBindingComplete += OnVisitsGridDataBindingComplete;
        auditGrid.DataBindingComplete += OnAuditGridDataBindingComplete;

        // Show gate panel by default
        ShowPanel(gatePanel);
    }

    public void SetCurrentUser(User user)
    {
        _mainViewModel.CurrentUser = user;
        _currentUserService.CurrentUser = user;
        UpdateWelcomeLabel(user.FirstName);

        // Enable/disable admin menu based on role
        var isAdmin = user.Role == UserRole.Admin;
        adminMenu.Enabled = isAdmin;
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        await _mainViewModel.InitializeAsync();
        siteLabel.Text = _mainViewModel.SiteDisplayName ?? _mainViewModel.SiteCode ?? "";
        UpdateWelcomeLabel(_mainViewModel.CurrentUser?.FirstName);

        // Set up settings form view (replaces grid)
        SetupSettingsFormView();
        await _settingsViewModel.LoadAsync();
        await LoadSettingsToFormAsync();

        // Set up master-detail view for Members
        SetupMembersMasterDetailView();

        await _membersViewModel.LoadAsync();
        _membersBinding = new BindingList<Member>(_membersViewModel.Members);
        membersGrid.DataSource = _membersBinding;

        // Populate combo boxes for Members form
        await PopulateMemberComboBoxes();

        await _visitsViewModel.LoadAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);
        _visitsBinding = new BindingList<Visit>(_visitsViewModel.Visits);
        visitsGrid.DataSource = _visitsBinding;
        visitsFromPicker.Value = DateTime.Today.AddDays(-7);
        visitsToPicker.Value = DateTime.Today;

        await _auditLogViewModel.LoadAsync(DateTime.UtcNow.AddDays(-7), DateTime.UtcNow, null, null);
        _auditBinding = new BindingList<AuditEntry>(_auditLogViewModel.Entries);
        auditGrid.DataSource = _auditBinding;
        auditFromPicker.Value = DateTime.Today.AddDays(-7);
        auditToPicker.Value = DateTime.Today;

        // Set up master-detail view for Users
        SetupUsersMasterDetailView();

        await LoadUsersAsync();

        // Refresh date pickers to use app locale after all views are set up
        RefreshDatePickerFormats();

        await TryStartSyncAsync();
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
        var settings = await _settingsService.GetAllAsync();
        var networkEnabled = settings.TryGetValue(Core.Constants.SettingKeys.NetworkEnabled, out var enabledValue) && enabledValue == "1";
        var apiKey = settings.TryGetValue(Core.Constants.SettingKeys.NetworkApiKey, out var keyValue) ? keyValue : string.Empty;
        var port = settings.TryGetValue(Core.Constants.SettingKeys.NetworkPort, out var portValue) && int.TryParse(portValue, out var parsedPort)
            ? parsedPort
            : _syncOptions.Port;

        _syncOptions.NetworkEnabled = networkEnabled;
        _syncOptions.ApiKey = apiKey;
        _syncOptions.Port = port;
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

        lastResultValueLabel.Text = _gateViewModel.LastResult ?? string.Empty;
        var badgeStatus = _gateViewModel.LastBadgeStatus;
        var warningText = _gateViewModel.BadgeWarning ?? string.Empty;
        badgeWarningLabel.Text = warningText;
        badgeWarningLabel.ForeColor = badgeStatus?.IsExpired == true
            ? Color.DarkRed
            : badgeStatus?.IsWarning == true
                ? Color.DarkOrange
                : SystemColors.ControlText;
        badgeWarningLabel.Visible = !string.IsNullOrWhiteSpace(warningText);
        memberNumberTextBox.Text = string.Empty;
        notesTextBox.Text = string.Empty;
        memberNumberTextBox.Focus();
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
            var previousCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;
            var bytes = await _backupService.CreateBackupAsync(includePhotos: true);
            await File.WriteAllBytesAsync(dialog.FileName, bytes);
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

    private async void OnAddMemberClick(object sender, EventArgs e)
    {
        var selected = membersGrid.CurrentRow?.DataBoundItem as Member;
        var selectedMember = selected is null ? new Member { Id = string.Empty } : CloneMember(selected);

        var editor = _serviceProvider.GetRequiredService<MemberEditorForm>();
        editor.LoadMember(selectedMember);
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await _membersViewModel.LoadAsync();
            _membersBinding = new BindingList<Member>(_membersViewModel.Members);
            membersGrid.DataSource = _membersBinding;
        }
    }

    private async void OnRefreshMembersClick(object sender, EventArgs e)
    {
        await _membersViewModel.LoadAsync();
        _membersBinding = new BindingList<Member>(_membersViewModel.Members);
        membersGrid.DataSource = _membersBinding;
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
        _visitsBinding = new BindingList<Visit>(_visitsViewModel.Visits);
        visitsGrid.DataSource = _visitsBinding;
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

    private void OnRemoteSitesClick(object sender, EventArgs e)
    {
        var form = _serviceProvider.GetRequiredService<RemoteSitesForm>();
        form.ShowDialog(this);
    }

    private async void OnRefreshUsersClick(object sender, EventArgs e)
    {
        if (!EnsureAdmin())
        {
            return;
        }

        await LoadUsersAsync();
    }

    private async void OnAddUserClick(object sender, EventArgs e)
    {
        if (!EnsureAdmin())
        {
            return;
        }

        var editor = _serviceProvider.GetRequiredService<UserEditorForm>();
        editor.LoadUser(new User { IsActive = true, Role = UserRole.User });
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadUsersAsync();
        }
    }

    private async void OnEditUserClick(object sender, EventArgs e)
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

        var editor = _serviceProvider.GetRequiredService<UserEditorForm>();
        editor.LoadUser(CloneUser(user));
        if (editor.ShowDialog(this) == DialogResult.OK)
        {
            await LoadUsersAsync();
        }
    }

    private async void OnDeactivateUserClick(object sender, EventArgs e)
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

        if (MessageBox.Show(Strings.Users_DeactivateConfirm, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        await _usersViewModel.DeactivateAsync(user.Id);
        await LoadUsersAsync();
    }

    private async void OnDeleteUserClick(object sender, EventArgs e)
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

        if (MessageBox.Show(Strings.Users_DeleteConfirm, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        await _usersViewModel.DeleteAsync(user.Id);
        await LoadUsersAsync();
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

        var dialog = new SaveFileDialog
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

        var dialog = new SaveFileDialog
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
            MessageBox.Show(Strings.Export_MembersSaved, Strings.App_Title);
        }
    }

    private async void OnExportVisitsClick(object sender, EventArgs e)
    {
        if (_visitsBinding is null)
        {
            return;
        }

        var dialog = new SaveFileDialog
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

        var dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"member_{member.MemberNumber}.pdf"
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var pdf = await _badgeRenderer.RenderBadgeAsync(member);
            await File.WriteAllBytesAsync(dialog.FileName, pdf);
            MessageBox.Show(Strings.Badge_PdfSaved, Strings.App_Title);
        }
    }

    private async void OnPrintBadgeClick(object sender, EventArgs e)
    {
        var member = membersGrid.CurrentRow?.DataBoundItem as Member;
        if (member is null)
        {
            MessageBox.Show(Strings.Members_SelectFirst, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var tempPath = Path.Combine(Path.GetTempPath(), $"badge_{member.MemberNumber}_{Guid.NewGuid():N}.pdf");
        var pdf = await _badgeRenderer.RenderBadgeAsync(member);
        await File.WriteAllBytesAsync(tempPath, pdf);
        using var printDialog = new PrintDialog();
        printDialog.UseEXDialog = true;
        MessageBox.Show(Strings.Badge_PrintReady, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);

        if (printDialog.ShowDialog(this) == DialogResult.OK)
        {
            try
            {
                var printer = printDialog.PrinterSettings.PrinterName;
                var startInfo = new System.Diagnostics.ProcessStartInfo(tempPath)
                {
                    UseShellExecute = true,
                    Verb = "printto",
                    Arguments = $"\"{printer}\""
                };
                System.Diagnostics.Process.Start(startInfo);
            }
            catch
            {
                MessageBox.Show(Strings.Badge_PrintFailed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        else
        {
            // if user cancels, offer to open for manual preview
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(tempPath) { UseShellExecute = true });
            }
            catch
            {
                // ignore failure to open
            }
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

        var dialog = new SaveFileDialog
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
        form.ShowDialog(this);
        await _catalogsViewModel.LoadAsync();
        await _membersViewModel.LoadAsync();
        _membersBinding = new BindingList<Member>(_membersViewModel.Members);
        membersGrid.DataSource = _membersBinding;
    }

    private static Member CloneMember(Member source)
    {
        return new Member
        {
            Id = source.Id,
            MemberNumber = source.MemberNumber,
            FirstName = source.FirstName,
            LastName = source.LastName,
            BusinessRank = source.BusinessRank,
            Office = source.Office,
            MemberTypeId = source.MemberTypeId,
            PersonalIdNumber = source.PersonalIdNumber,
            BusinessIdNumber = source.BusinessIdNumber,
            IsPermanentStaff = source.IsPermanentStaff,
            PhotoFileName = source.PhotoFileName,
            BadgeIssueDate = source.BadgeIssueDate,
            BadgeExpiryDate = source.BadgeExpiryDate,
            DateOfBirth = source.DateOfBirth,
            Phone = source.Phone,
            Email = source.Email,
            Notes = source.Notes,
            IsActive = source.IsActive,
            CreatedAtUtc = source.CreatedAtUtc,
            UpdatedAtUtc = source.UpdatedAtUtc
        };
    }

    private static User CloneUser(User source)
    {
        return new User
        {
            Id = source.Id,
            Username = source.Username,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Role = source.Role,
            IsShared = source.IsShared,
            IsSystem = source.IsSystem,
            IsActive = source.IsActive,
            CreatedAtUtc = source.CreatedAtUtc,
            LastLoginAtUtc = source.LastLoginAtUtc
        };
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

        // Hide ID columns - not needed for users
        var hiddenColumns = new[] { "Id", "VisitGuid" };

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
    }

    private void UpdateWelcomeLabel(string? firstName)
    {
        var site = _mainViewModel.SiteDisplayName ?? _mainViewModel.SiteCode ?? "";
        if (string.IsNullOrEmpty(firstName))
        {
            welcomeLabel.Text = $"{Strings.Main_Welcome} | {site}";
        }
        else
        {
            welcomeLabel.Text = $"{Strings.Main_Welcome}, {firstName} | {site}";
        }
    }

    #endregion

    #region Menu Event Handlers

    private void OnMenuMainPageClick(object? sender, EventArgs e) => ShowPanel(gatePanel);

    private void OnMenuVisitsClick(object? sender, EventArgs e) => ShowPanel(visitsPanel);

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
}
