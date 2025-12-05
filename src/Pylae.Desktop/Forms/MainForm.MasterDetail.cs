using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Enums;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.Forms;

/// <summary>
/// Partial class containing master-detail view implementations for Members and Users tabs.
/// </summary>
public partial class MainForm
{
    private TextBox? _memberSearchBox;
    private TextBox? _memberNumberTextBox;
    private TextBox? _memberFirstNameTextBox;
    private TextBox? _memberLastNameTextBox;
    private TextBox? _memberBusinessRankTextBox;
    private TextBox? _memberPersonalIdTextBox;
    private TextBox? _memberBusinessIdTextBox;
    private ComboBox? _memberOfficeComboBox;
    private ComboBox? _memberTypeComboBox;
    private CheckBox? _memberPermanentCheckBox;
    private Button? _memberSaveButton;
    private Button? _memberNewButton;
    private Button? _memberDeleteButton;
    private Label? _memberPhotoLabel;
    private Button? _memberChoosePhotoButton;
    private Button? _memberClearPhotoButton;
    private string? _memberPhotoPath;

    private TextBox? _userSearchBox;
    private TextBox? _userUsernameTextBox;
    private TextBox? _userFirstNameTextBox;
    private TextBox? _userLastNameTextBox;
    private ComboBox? _userRoleComboBox;
    private CheckBox? _userActiveCheckBox;
    private CheckBox? _userSharedLoginCheckBox;
    private Button? _userSaveButton;
    private Button? _userNewButton;
    private Button? _userDeleteButton;

    /// <summary>
    /// Sets up the Members tab with a master-detail layout.
    /// Top panel: Member form for CRUD operations
    /// Bottom panel: DataGridView with search/filter
    /// </summary>
    private void SetupMembersMasterDetailView()
    {
        // Clear existing controls
        membersTab.Controls.Clear();

        // Create main layout (vertical split: form top, grid bottom)
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(10)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220)); // Form height
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid fills remaining

        // === TOP PANEL: Member Form ===
        var formPanel = CreateMemberFormPanel();
        mainLayout.Controls.Add(formPanel, 0, 0);

        // === BOTTOM PANEL: Grid with Search ===
        var gridPanel = CreateMemberGridPanel();
        mainLayout.Controls.Add(gridPanel, 0, 1);

        membersTab.Controls.Add(mainLayout);

        // Wire up grid selection event
        membersGrid.SelectionChanged += OnMemberGridSelectionChanged;

        // Wire up data binding event to localize column headers
        membersGrid.DataBindingComplete += OnMembersGridDataBindingComplete;
    }

    private Panel CreateMemberFormPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = SystemColors.Control,
            Padding = new Padding(5)
        };

        // Create form layout (3 columns: labels, fields, photo)
        var formLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 5,
            Padding = new Padding(5),
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };

        // Column styles: Label | Field | Label | Field | Photo
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // Label 1
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));   // Field 1
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // Label 2
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));   // Field 2
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));   // Photo column

        // Row styles: even height for 4 rows
        for (int i = 0; i < 4; i++)
            formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        // Initialize controls
        _memberNumberTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberFirstNameTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberLastNameTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberBusinessRankTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberPersonalIdTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberBusinessIdTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberOfficeComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
        _memberTypeComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
        _memberPermanentCheckBox = new CheckBox { Text = Strings.Member_PermanentStaff, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

        // Photo controls
        _memberPhotoLabel = new Label
        {
            Text = Strings.Photo_None,
            BorderStyle = BorderStyle.FixedSingle,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F)
        };
        _memberChoosePhotoButton = new Button { Text = Strings.Photo_SelectTitle, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
        _memberClearPhotoButton = new Button { Text = Strings.Member_ClearPhoto, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };

        _memberChoosePhotoButton.Click += OnMemberChoosePhoto;
        _memberClearPhotoButton.Click += OnMemberClearPhoto;

        // Action buttons
        _memberNewButton = new Button { Text = Strings.Button_New, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        _memberSaveButton = new Button { Text = Strings.Button_Save, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        _memberDeleteButton = new Button { Text = Strings.Button_Delete, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

        _memberNewButton.Click += OnMemberNew;
        _memberSaveButton.Click += OnMemberSave;
        _memberDeleteButton.Click += OnMemberDelete;

        // Row 0: Member Number | First Name
        formLayout.Controls.Add(new Label { Text = Strings.Member_Number + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 0);
        formLayout.Controls.Add(_memberNumberTextBox, 1, 0);
        formLayout.Controls.Add(new Label { Text = Strings.Member_FirstName + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, 0);
        formLayout.Controls.Add(_memberFirstNameTextBox, 3, 0);

        // Row 1: Last Name | Business Rank
        formLayout.Controls.Add(new Label { Text = Strings.Member_LastName + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 1);
        formLayout.Controls.Add(_memberLastNameTextBox, 1, 1);
        formLayout.Controls.Add(new Label { Text = Strings.Member_BusinessRank + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, 1);
        formLayout.Controls.Add(_memberBusinessRankTextBox, 3, 1);

        // Row 2: Personal ID | Business ID
        formLayout.Controls.Add(new Label { Text = Strings.Member_PersonalId + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 2);
        formLayout.Controls.Add(_memberPersonalIdTextBox, 1, 2);
        formLayout.Controls.Add(new Label { Text = Strings.Member_BusinessId + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, 2);
        formLayout.Controls.Add(_memberBusinessIdTextBox, 3, 2);

        // Row 3: Office | Member Type | Permanent
        formLayout.Controls.Add(new Label { Text = Strings.Member_Office + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 0, 3);
        formLayout.Controls.Add(_memberOfficeComboBox, 1, 3);
        formLayout.Controls.Add(new Label { Text = Strings.Member_Type + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill }, 2, 3);
        formLayout.Controls.Add(_memberTypeComboBox, 3, 3);

        // Photo column (spans all rows)
        var photoLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 1
        };
        photoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 60)); // Photo
        photoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20)); // Choose button
        photoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 20)); // Clear button
        photoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 0));  // Spacer

        photoLayout.Controls.Add(_memberPhotoLabel, 0, 0);
        photoLayout.Controls.Add(_memberChoosePhotoButton, 0, 1);
        photoLayout.Controls.Add(_memberClearPhotoButton, 0, 2);

        formLayout.Controls.Add(photoLayout, 4, 0);
        formLayout.SetRowSpan(photoLayout, 3);

        // Button panel in last column, bottom row
        var buttonLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 3,
            Padding = new Padding(0)
        };
        buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

        buttonLayout.Controls.Add(_memberNewButton, 0, 0);
        buttonLayout.Controls.Add(_memberSaveButton, 1, 0);
        buttonLayout.Controls.Add(_memberDeleteButton, 2, 0);

        formLayout.Controls.Add(buttonLayout, 4, 3);

        // Permanent checkbox in remaining space
        var permanentPanel = new Panel { Dock = DockStyle.Fill };
        permanentPanel.Controls.Add(_memberPermanentCheckBox);
        formLayout.Controls.Add(permanentPanel, 0, 3);
        formLayout.SetColumnSpan(permanentPanel, 2);

        panel.Controls.Add(formLayout);
        return panel;
    }

    private Panel CreateMemberGridPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(5)
        };

        // Layout: search bar on top, grid below
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Search bar
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid

        // Search bar
        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 4,
            Padding = new Padding(5, 0, 5, 0)
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));

        var searchLabel = new Label { Text = Strings.Search + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberSearchBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberSearchBox.TextChanged += OnMemberSearch;

        var exportButton = new Button { Text = Strings.Button_Export, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        var badgeButton = new Button { Text = Strings.Button_BadgePdf, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

        exportButton.Click += OnExportMembersClick;
        badgeButton.Click += OnBadgeClick;

        searchPanel.Controls.Add(searchLabel, 0, 0);
        searchPanel.Controls.Add(_memberSearchBox, 1, 0);
        searchPanel.Controls.Add(exportButton, 2, 0);
        searchPanel.Controls.Add(badgeButton, 3, 0);

        // Grid
        membersGrid.Dock = DockStyle.Fill;
        membersGrid.Font = new Font("Segoe UI", 10F);
        membersGrid.RowTemplate.Height = 30;
        membersGrid.AllowUserToAddRows = false;
        membersGrid.AllowUserToDeleteRows = false;
        membersGrid.ReadOnly = true;
        membersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        membersGrid.MultiSelect = false;

        layout.Controls.Add(searchPanel, 0, 0);
        layout.Controls.Add(membersGrid, 0, 1);

        panel.Controls.Add(layout);
        return panel;
    }

    // Event handlers for Members form
    private void OnMemberGridSelectionChanged(object? sender, EventArgs e)
    {
        if (membersGrid.CurrentRow?.DataBoundItem is Member member)
        {
            PopulateMemberForm(member);
        }
    }

    private void PopulateMemberForm(Member member)
    {
        if (_memberNumberTextBox == null) return;

        _memberNumberTextBox.Text = member.MemberNumber.ToString();
        _memberFirstNameTextBox!.Text = member.FirstName;
        _memberLastNameTextBox!.Text = member.LastName;
        _memberBusinessRankTextBox!.Text = member.BusinessRank ?? string.Empty;
        _memberPersonalIdTextBox!.Text = member.PersonalIdNumber ?? string.Empty;
        _memberBusinessIdTextBox!.Text = member.BusinessIdNumber ?? string.Empty;
        _memberPermanentCheckBox!.Checked = member.IsPermanentStaff;

        // Set combo box selections (will be populated in OnLoad)
        if (member.OfficeId.HasValue)
            _memberOfficeComboBox!.SelectedValue = member.OfficeId.Value;
        else
            _memberOfficeComboBox!.SelectedIndex = -1;

        if (member.MemberTypeId.HasValue)
            _memberTypeComboBox!.SelectedValue = member.MemberTypeId.Value;
        else
            _memberTypeComboBox!.SelectedIndex = -1;

        // Photo
        _memberPhotoPath = member.PhotoFileName;
        if (!string.IsNullOrEmpty(member.PhotoFileName))
        {
            _memberPhotoLabel!.Text = Path.GetFileName(member.PhotoFileName);
        }
        else
        {
            _memberPhotoLabel!.Text = Strings.Photo_None;
        }
    }

    private void ClearMemberForm()
    {
        if (_memberNumberTextBox == null) return;

        _memberNumberTextBox.Clear();
        _memberFirstNameTextBox!.Clear();
        _memberLastNameTextBox!.Clear();
        _memberBusinessRankTextBox!.Clear();
        _memberPersonalIdTextBox!.Clear();
        _memberBusinessIdTextBox!.Clear();
        _memberPermanentCheckBox!.Checked = false;
        _memberOfficeComboBox!.SelectedIndex = -1;
        _memberTypeComboBox!.SelectedIndex = -1;
        _memberPhotoPath = null;
        _memberPhotoLabel!.Text = Strings.Photo_None;
    }

    private void OnMemberNew(object? sender, EventArgs e)
    {
        ClearMemberForm();
        _memberNumberTextBox?.Focus();
    }

    private async void OnMemberSave(object? sender, EventArgs e)
    {
        if (_memberNumberTextBox == null || string.IsNullOrWhiteSpace(_memberNumberTextBox.Text))
        {
            MessageBox.Show(Strings.Member_NumberRequired, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!int.TryParse(_memberNumberTextBox.Text, out var memberNumber))
        {
            MessageBox.Show(Strings.Members_NumberInvalid, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var member = membersGrid.CurrentRow?.DataBoundItem as Member ?? new Member();
        member.MemberNumber = memberNumber;
        member.FirstName = _memberFirstNameTextBox!.Text;
        member.LastName = _memberLastNameTextBox!.Text;
        member.BusinessRank = _memberBusinessRankTextBox!.Text;
        member.PersonalIdNumber = _memberPersonalIdTextBox!.Text;
        member.BusinessIdNumber = _memberBusinessIdTextBox!.Text;
        member.IsPermanentStaff = _memberPermanentCheckBox!.Checked;
        member.OfficeId = _memberOfficeComboBox!.SelectedValue as int?;
        member.MemberTypeId = _memberTypeComboBox!.SelectedValue as int?;
        member.PhotoFileName = _memberPhotoPath;

        try
        {
            await _membersViewModel.SaveAsync(member);
            await _membersViewModel.LoadAsync();
            _membersBinding = new BindingList<Member>(_membersViewModel.Members);
            membersGrid.DataSource = _membersBinding;

            MessageBox.Show(Strings.Member_SaveSuccess, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Strings.Member_SaveFailed}: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void OnMemberDelete(object? sender, EventArgs e)
    {
        var member = membersGrid.CurrentRow?.DataBoundItem as Member;
        if (member == null || string.IsNullOrEmpty(member.Id))
        {
            MessageBox.Show(Strings.Members_SelectFirst, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show(Strings.Member_DeleteConfirm, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            var memberService = _serviceProvider.GetRequiredService<Core.Interfaces.IMemberService>();
            await memberService.DeleteAsync(member.Id);
            await _membersViewModel.LoadAsync();
            _membersBinding = new BindingList<Member>(_membersViewModel.Members);
            membersGrid.DataSource = _membersBinding;
            ClearMemberForm();

            MessageBox.Show(Strings.Member_DeleteSuccess, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Strings.Member_DeleteFailed}: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnMemberSearch(object? sender, EventArgs e)
    {
        if (_memberSearchBox == null || _membersViewModel.Members == null) return;

        var searchText = _memberSearchBox.Text.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            _membersBinding = new BindingList<Member>(_membersViewModel.Members);
        }
        else
        {
            var filtered = _membersViewModel.Members.Where(m =>
                m.MemberNumber.ToString().Contains(searchText) ||
                m.FirstName.ToLowerInvariant().Contains(searchText) ||
                m.LastName.ToLowerInvariant().Contains(searchText) ||
                (m.PersonalIdNumber?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.BusinessIdNumber?.ToLowerInvariant().Contains(searchText) ?? false)
            ).ToList();
            _membersBinding = new BindingList<Member>(filtered);
        }

        membersGrid.DataSource = _membersBinding;
    }

    private void OnMemberChoosePhoto(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = Strings.Photo_Filter,
            Title = Strings.Photo_SelectTitle
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _memberPhotoPath = dialog.FileName;
            _memberPhotoLabel!.Text = Path.GetFileName(dialog.FileName);
        }
    }

    private void OnMemberClearPhoto(object? sender, EventArgs e)
    {
        _memberPhotoPath = null;
        _memberPhotoLabel!.Text = Strings.Photo_None;
    }

    // ==================== USERS MASTER-DETAIL VIEW ====================

    /// <summary>
    /// Sets up the Users tab with a master-detail layout.
    /// Top panel: User form for CRUD operations
    /// Bottom panel: DataGridView with search/filter
    /// </summary>
    private void SetupUsersMasterDetailView()
    {
        // Clear existing controls
        usersTab.Controls.Clear();

        // Create main layout (vertical split: form top, grid bottom)
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(10)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 200)); // Form height
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid fills remaining

        // === TOP PANEL: User Form ===
        var formPanel = CreateUserFormPanel();
        mainLayout.Controls.Add(formPanel, 0, 0);

        // === BOTTOM PANEL: Grid with Search ===
        var gridPanel = CreateUserGridPanel();
        mainLayout.Controls.Add(gridPanel, 0, 1);

        usersTab.Controls.Add(mainLayout);

        // Wire up grid selection event
        usersGrid.SelectionChanged += OnUserGridSelectionChanged;

        // Wire up data binding event to localize column headers
        usersGrid.DataBindingComplete += OnUsersGridDataBindingComplete;
    }

    private Panel CreateUserFormPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = SystemColors.Control,
            Padding = new Padding(5)
        };

        // Create form layout (2 columns of fields)
        var formLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 4,
            ColumnCount = 4,
            Padding = new Padding(5),
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };

        // Column styles: Label | Field | Label | Field
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // Label 1
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // Field 1
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120)); // Label 2
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));   // Field 2

        // Row styles
        for (int i = 0; i < 4; i++)
            formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

        // Initialize controls
        _userUsernameTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userFirstNameTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userLastNameTextBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userRoleComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10F) };
        _userActiveCheckBox = new CheckBox { Text = Strings.Users_IsActive, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userSharedLoginCheckBox = new CheckBox { Text = Strings.Users_IsShared, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

        // Populate role combo box
        _userRoleComboBox.DataSource = Enum.GetValues(typeof(UserRole));

        // Action buttons
        _userNewButton = new Button { Text = Strings.Button_New, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        _userSaveButton = new Button { Text = Strings.Button_Save, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold) };
        _userDeleteButton = new Button { Text = Strings.Button_Delete, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };

        _userNewButton.Click += OnUserNew;
        _userSaveButton.Click += OnUserSave;
        _userDeleteButton.Click += OnUserDelete;

        // Row 0: Username | First Name
        formLayout.Controls.Add(new Label { Text = Strings.Users_Username + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) }, 0, 0);
        formLayout.Controls.Add(_userUsernameTextBox, 1, 0);
        formLayout.Controls.Add(new Label { Text = Strings.Users_FirstName + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) }, 2, 0);
        formLayout.Controls.Add(_userFirstNameTextBox, 3, 0);

        // Row 1: Last Name | Role
        formLayout.Controls.Add(new Label { Text = Strings.Users_LastName + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) }, 0, 1);
        formLayout.Controls.Add(_userLastNameTextBox, 1, 1);
        formLayout.Controls.Add(new Label { Text = Strings.Users_Role + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) }, 2, 1);
        formLayout.Controls.Add(_userRoleComboBox, 3, 1);

        // Row 2: Active checkbox | Shared Login checkbox
        formLayout.Controls.Add(_userActiveCheckBox, 0, 2);
        formLayout.SetColumnSpan(_userActiveCheckBox, 2);
        formLayout.Controls.Add(_userSharedLoginCheckBox, 2, 2);
        formLayout.SetColumnSpan(_userSharedLoginCheckBox, 2);

        // Row 3: Action buttons
        var buttonLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 3,
            Padding = new Padding(0, 10, 0, 0)
        };
        buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));

        buttonLayout.Controls.Add(_userNewButton, 0, 0);
        buttonLayout.Controls.Add(_userSaveButton, 1, 0);
        buttonLayout.Controls.Add(_userDeleteButton, 2, 0);

        formLayout.Controls.Add(buttonLayout, 0, 3);
        formLayout.SetColumnSpan(buttonLayout, 4);

        panel.Controls.Add(formLayout);
        return panel;
    }

    private Panel CreateUserGridPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(5)
        };

        // Layout: search bar on top, grid below
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40)); // Search bar
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Grid

        // Search bar
        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 4,
            Padding = new Padding(5, 0, 5, 0)
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));

        var searchLabel = new Label { Text = Strings.Search + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userSearchBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userSearchBox.TextChanged += OnUserSearch;

        var resetPasswordButton = new Button { Text = Strings.Button_ResetPassword, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
        var resetQuickCodeButton = new Button { Text = Strings.Button_ResetQuickCode, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };

        resetPasswordButton.Click += OnResetPasswordClick;
        resetQuickCodeButton.Click += OnResetQuickCodeClick;

        searchPanel.Controls.Add(searchLabel, 0, 0);
        searchPanel.Controls.Add(_userSearchBox, 1, 0);
        searchPanel.Controls.Add(resetPasswordButton, 2, 0);
        searchPanel.Controls.Add(resetQuickCodeButton, 3, 0);

        // Grid
        usersGrid.Dock = DockStyle.Fill;
        usersGrid.Font = new Font("Segoe UI", 10F);
        usersGrid.RowTemplate.Height = 30;
        usersGrid.AllowUserToAddRows = false;
        usersGrid.AllowUserToDeleteRows = false;
        usersGrid.ReadOnly = true;
        usersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        usersGrid.MultiSelect = false;

        layout.Controls.Add(searchPanel, 0, 0);
        layout.Controls.Add(usersGrid, 0, 1);

        panel.Controls.Add(layout);
        return panel;
    }

    // Event handlers for Users form
    private void OnUserGridSelectionChanged(object? sender, EventArgs e)
    {
        if (usersGrid.CurrentRow?.DataBoundItem is User user)
        {
            PopulateUserForm(user);
        }
    }

    private void PopulateUserForm(User user)
    {
        if (_userUsernameTextBox == null) return;

        _userUsernameTextBox.Text = user.Username;
        _userFirstNameTextBox!.Text = user.FirstName;
        _userLastNameTextBox!.Text = user.LastName;
        _userRoleComboBox!.SelectedItem = user.Role;
        _userActiveCheckBox!.Checked = user.IsActive;
        _userSharedLoginCheckBox!.Checked = user.IsShared;
    }

    private void ClearUserForm()
    {
        if (_userUsernameTextBox == null) return;

        _userUsernameTextBox.Clear();
        _userFirstNameTextBox!.Clear();
        _userLastNameTextBox!.Clear();
        _userRoleComboBox!.SelectedIndex = 0;
        _userActiveCheckBox!.Checked = true;
        _userSharedLoginCheckBox!.Checked = false;
    }

    private void OnUserNew(object? sender, EventArgs e)
    {
        ClearUserForm();
        _userUsernameTextBox?.Focus();
    }

    private async void OnUserSave(object? sender, EventArgs e)
    {
        if (_userUsernameTextBox == null || string.IsNullOrWhiteSpace(_userUsernameTextBox.Text))
        {
            MessageBox.Show(Strings.Users_UsernameRequired, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var user = usersGrid.CurrentRow?.DataBoundItem as User ?? new User();
        var isNewUser = user.Id == 0;

        user.Username = _userUsernameTextBox.Text;
        user.FirstName = _userFirstNameTextBox!.Text;
        user.LastName = _userLastNameTextBox!.Text;
        user.Role = (UserRole)_userRoleComboBox!.SelectedItem!;
        user.IsActive = _userActiveCheckBox!.Checked;
        user.IsShared = _userSharedLoginCheckBox!.Checked;

        try
        {
            if (isNewUser)
            {
                // New user - need password - open editor dialog
                using var passwordDialog = new UserEditorForm(_usersViewModel);
                if (passwordDialog.ShowDialog() != DialogResult.OK)
                    return;
                // User will be created via the dialog
            }
            else
            {
                // Update existing user (no password change)
                await _usersViewModel.SaveAsync(user, null, null);
            }

            await LoadUsersAsync();
            MessageBox.Show(Strings.User_SaveSuccess, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Strings.User_SaveFailed}: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void OnUserDelete(object? sender, EventArgs e)
    {
        var user = usersGrid.CurrentRow?.DataBoundItem as User;
        if (user == null || user.Id == 0)
        {
            MessageBox.Show(Strings.Users_SelectUser, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (user.IsSystem)
        {
            MessageBox.Show(Strings.User_CannotDeleteSystem, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (MessageBox.Show(Strings.Users_DeleteConfirm, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            await _usersViewModel.DeleteAsync(user.Id);
            await LoadUsersAsync();
            ClearUserForm();

            MessageBox.Show(Strings.User_DeleteSuccess, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Strings.User_DeleteFailed}: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OnUserSearch(object? sender, EventArgs e)
    {
        if (_userSearchBox == null || _usersViewModel.Users == null) return;

        var searchText = _userSearchBox.Text.ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(searchText))
        {
            _usersBinding = new BindingList<User>(_usersViewModel.Users);
        }
        else
        {
            var filtered = _usersViewModel.Users.Where(u =>
                u.Username.ToLowerInvariant().Contains(searchText) ||
                u.FirstName.ToLowerInvariant().Contains(searchText) ||
                u.LastName.ToLowerInvariant().Contains(searchText)
            ).ToList();
            _usersBinding = new BindingList<User>(filtered);
        }

        usersGrid.DataSource = _usersBinding;
    }

    /// <summary>
    /// Event handler to set localized column headers for Members grid
    /// </summary>
    private void OnMembersGridDataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        if (membersGrid.Columns.Count == 0) return;

        // Map property names to localized headers
        var columnMappings = new Dictionary<string, string>
        {
            { nameof(Member.Id), Strings.Grid_Id },
            { nameof(Member.MemberNumber), Strings.Grid_MemberNumber },
            { nameof(Member.FirstName), Strings.Grid_FirstName },
            { nameof(Member.LastName), Strings.Grid_LastName },
            { nameof(Member.PersonalIdNumber), Strings.Grid_PersonalIdNumber },
            { nameof(Member.BusinessIdNumber), Strings.Grid_BusinessIdNumber },
            { nameof(Member.BusinessRank), Strings.Grid_BusinessRank },
            { nameof(Member.OfficeId), Strings.Grid_OfficeId },
            { nameof(Member.MemberTypeId), Strings.Grid_MemberTypeId },
            { nameof(Member.IsPermanentStaff), Strings.Grid_IsPermanent },
            { nameof(Member.PhotoFileName), Strings.Grid_PhotoPath }
        };

        foreach (DataGridViewColumn column in membersGrid.Columns)
        {
            if (columnMappings.TryGetValue(column.DataPropertyName, out var localizedHeader))
            {
                column.HeaderText = localizedHeader;
            }
        }
    }

    /// <summary>
    /// Event handler to set localized column headers for Users grid
    /// </summary>
    private void OnUsersGridDataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
    {
        if (usersGrid.Columns.Count == 0) return;

        // Map property names to localized headers
        var columnMappings = new Dictionary<string, string>
        {
            { nameof(User.Id), Strings.Grid_Id },
            { nameof(User.Username), Strings.Grid_Username },
            { nameof(User.FirstName), Strings.Grid_FirstName },
            { nameof(User.LastName), Strings.Grid_LastName },
            { nameof(User.Role), Strings.Grid_Role },
            { nameof(User.IsActive), Strings.Grid_IsActive },
            { nameof(User.IsShared), Strings.Grid_IsSharedLogin },
            { nameof(User.LastLoginAtUtc), Strings.Grid_LastLogin }
        };

        foreach (DataGridViewColumn column in usersGrid.Columns)
        {
            if (columnMappings.TryGetValue(column.DataPropertyName, out var localizedHeader))
            {
                column.HeaderText = localizedHeader;
            }
        }
    }
}
