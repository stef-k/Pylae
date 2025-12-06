using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Models;
using Pylae.Desktop.Helpers;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.Forms;

/// <summary>
/// Partial class containing master-detail view implementations for Members and Users tabs.
/// </summary>
public partial class MainForm
{
    private TextBox? _memberSearchBox;
    private Button? _selectAllMembersButton;
    private bool _allMembersSelected;
    private Label? _memberNumberValueLabel;
    private TextBox? _memberFirstNameTextBox;
    private TextBox? _memberLastNameTextBox;
    private TextBox? _memberBusinessRankTextBox;
    private TextBox? _memberPersonalIdTextBox;
    private TextBox? _memberBusinessIdTextBox;
    private TextBox? _memberOfficeTextBox;
    private ComboBox? _memberTypeComboBox;
    private CheckBox? _memberPermanentCheckBox;
    private CheckBox? _memberActiveCheckBox;
    private DateTimePicker? _memberBadgeIssuePicker;
    private DateTimePicker? _memberBadgeExpiryPicker;
    private DateTimePicker? _memberDateOfBirthPicker;
    private TextBox? _memberPhoneTextBox;
    private TextBox? _memberEmailTextBox;
    private TextBox? _memberNotesTextBox;
    private Button? _memberSaveButton;
    private Button? _memberNewButton;
    private Button? _memberDeleteButton;
    private PictureBox? _memberPhotoBox;
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
        membersPanel.Controls.Clear();

        // Create SplitContainer for adjustable form/grid ratio
        var splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterWidth = 6,
            BorderStyle = BorderStyle.None,
            BackColor = SystemColors.Control
        };
        // Defer min sizes and splitter distance until control is properly sized
        splitContainer.HandleCreated += (s, e) =>
        {
            splitContainer.Panel1MinSize = 220;
            splitContainer.Panel2MinSize = 200;
            splitContainer.FixedPanel = FixedPanel.Panel1;
            if (splitContainer.Height > 500)
                splitContainer.SplitterDistance = 300;
            else if (splitContainer.Height > 400)
                splitContainer.SplitterDistance = splitContainer.Height / 2;
        };

        // === TOP PANEL: Member Form ===
        var formPanel = CreateMemberFormPanel();
        splitContainer.Panel1.Controls.Add(formPanel);
        splitContainer.Panel1.Padding = new Padding(5, 5, 5, 0);

        // === BOTTOM PANEL: Grid with Search ===
        var gridPanel = CreateMemberGridPanel();
        splitContainer.Panel2.Controls.Add(gridPanel);
        splitContainer.Panel2.Padding = new Padding(5, 2, 5, 5);

        membersPanel.Controls.Add(splitContainer);

        // Wire up grid selection event
        membersGrid.SelectionChanged += OnMemberGridSelectionChanged;

        // Wire up data binding event to localize column headers
        membersGrid.DataBindingComplete += OnMembersGridDataBindingComplete;

        // Initialize form with defaults for new member
        ClearMemberForm();
    }

    private Panel CreateMemberFormPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = SystemColors.Control,
            Padding = new Padding(0)
        };

        // Create form layout - expanded to 7 rows for all fields
        var formLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 7,
            ColumnCount = 5,
            Padding = new Padding(5),
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };

        // Column styles: Label | Field | Label | Field | Photo/Buttons
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // Label 1
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));   // Field 1
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // Label 2
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));   // Field 2
        formLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));   // Photo/buttons column

        // Row styles: 7 rows
        for (int i = 0; i < 7; i++)
            formLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 7f));

        var labelFont = new Font("Segoe UI", 10F);
        var inputFont = new Font("Segoe UI", 10F);

        // Initialize controls
        _memberNumberValueLabel = new Label { Dock = DockStyle.Fill, Font = new Font(inputFont, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Text = Strings.MemberNumber_Auto };
        _memberFirstNameTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberLastNameTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberBusinessRankTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberPersonalIdTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberBusinessIdTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberOfficeTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberTypeComboBox = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Font = inputFont };
        _memberPhoneTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberEmailTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };
        _memberNotesTextBox = new TextBox { Dock = DockStyle.Fill, Font = inputFont };

        // Auto-uppercase all text fields on leave
        _memberFirstNameTextBox.Leave += OnTextBoxLeaveUppercase;
        _memberLastNameTextBox.Leave += OnTextBoxLeaveUppercase;
        _memberBusinessRankTextBox.Leave += OnTextBoxLeaveUppercase;
        _memberPersonalIdTextBox.Leave += OnTextBoxLeaveUppercase;
        _memberBusinessIdTextBox.Leave += OnTextBoxLeaveUppercase;
        _memberOfficeTextBox.Leave += OnTextBoxLeaveUppercase;
        _memberNotesTextBox.Leave += OnTextBoxLeaveUppercase;

        // Date pickers with checkbox-style enable (ShowCheckBox allows null dates)
        // Use CurrentUICulture for date format to match app language
        var dateFormat = System.Threading.Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortDatePattern;
        _memberBadgeIssuePicker = new DateTimePicker { Dock = DockStyle.Fill, Font = inputFont, Format = DateTimePickerFormat.Custom, CustomFormat = dateFormat, ShowCheckBox = true, Checked = false };
        _memberBadgeExpiryPicker = new DateTimePicker { Dock = DockStyle.Fill, Font = inputFont, Format = DateTimePickerFormat.Custom, CustomFormat = dateFormat, ShowCheckBox = true, Checked = false };
        _memberDateOfBirthPicker = new DateTimePicker { Dock = DockStyle.Fill, Font = inputFont, Format = DateTimePickerFormat.Custom, CustomFormat = dateFormat, ShowCheckBox = true, Checked = false };

        // Auto-calculate expiry date when issue date changes (if BadgeValidityMonths > 0)
        _memberBadgeIssuePicker.ValueChanged += OnMemberBadgeIssueDateChanged;

        // Checkboxes
        _memberPermanentCheckBox = new CheckBox { Text = Strings.Member_PermanentStaff, Dock = DockStyle.Fill, Font = inputFont };
        _memberActiveCheckBox = new CheckBox { Text = Strings.Member_Active, Dock = DockStyle.Fill, Font = inputFont, Checked = true };

        // Photo controls - PictureBox for portrait photos with click to enlarge
        _memberPhotoBox = new PictureBox
        {
            BorderStyle = BorderStyle.FixedSingle,
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Cursor = Cursors.Hand,
            BackColor = SystemColors.Control
        };
        _memberPhotoBox.Click += OnMemberPhotoClick;
        _memberChoosePhotoButton = new Button { Text = Strings.Photo_SelectTitle, Font = new Font("Segoe UI", 9F), Height = 28, Dock = DockStyle.Fill, AutoSize = true, UseVisualStyleBackColor = true };
        _memberClearPhotoButton = new Button { Text = Strings.Member_ClearPhoto, Font = new Font("Segoe UI", 9F), Height = 28, Dock = DockStyle.Fill, AutoSize = true, UseVisualStyleBackColor = true };

        _memberChoosePhotoButton.Click += OnMemberChoosePhoto;
        _memberClearPhotoButton.Click += OnMemberClearPhoto;

        // Action buttons - same height as photo buttons
        _memberNewButton = new Button { Text = Strings.Button_New, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Height = 28, Dock = DockStyle.Fill, UseVisualStyleBackColor = true };
        _memberSaveButton = new Button { Text = Strings.Button_Save, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Height = 28, Dock = DockStyle.Fill, MinimumSize = new Size(100, 28), UseVisualStyleBackColor = true, ForeColor = Color.Blue };
        _memberDeleteButton = new Button { Text = Strings.Button_Delete, Font = new Font("Segoe UI", 9F), Height = 28, Dock = DockStyle.Fill, UseVisualStyleBackColor = true, ForeColor = Color.Red };

        _memberNewButton.Click += OnMemberNew;
        _memberSaveButton.Click += OnMemberSave;
        _memberDeleteButton.Click += OnMemberDelete;

        // Row 0: Member Number (read-only, auto-assigned) | First Name
        formLayout.Controls.Add(new Label { Text = Strings.Member_Number + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 0);
        formLayout.Controls.Add(_memberNumberValueLabel, 1, 0);
        formLayout.Controls.Add(new Label { Text = Strings.Member_FirstName + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 2, 0);
        formLayout.Controls.Add(_memberFirstNameTextBox, 3, 0);

        // Row 1: Last Name | Business Rank
        formLayout.Controls.Add(new Label { Text = Strings.Member_LastName + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 1);
        formLayout.Controls.Add(_memberLastNameTextBox, 1, 1);
        formLayout.Controls.Add(new Label { Text = Strings.Member_BusinessRank + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 2, 1);
        formLayout.Controls.Add(_memberBusinessRankTextBox, 3, 1);

        // Row 2: Personal ID | Business ID
        formLayout.Controls.Add(new Label { Text = Strings.Member_PersonalId + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 2);
        formLayout.Controls.Add(_memberPersonalIdTextBox, 1, 2);
        formLayout.Controls.Add(new Label { Text = Strings.Member_BusinessId + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 2, 2);
        formLayout.Controls.Add(_memberBusinessIdTextBox, 3, 2);

        // Row 3: Office | Member Type
        formLayout.Controls.Add(new Label { Text = Strings.Member_Office + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 3);
        formLayout.Controls.Add(_memberOfficeTextBox, 1, 3);
        formLayout.Controls.Add(new Label { Text = Strings.Member_Type + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 2, 3);
        formLayout.Controls.Add(_memberTypeComboBox, 3, 3);

        // Row 4: Phone | Email
        formLayout.Controls.Add(new Label { Text = Strings.Member_Phone + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 4);
        formLayout.Controls.Add(_memberPhoneTextBox, 1, 4);
        formLayout.Controls.Add(new Label { Text = Strings.Member_Email + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 2, 4);
        formLayout.Controls.Add(_memberEmailTextBox, 3, 4);

        // Row 5: Badge Issue | Badge Expiry
        formLayout.Controls.Add(new Label { Text = Strings.Member_BadgeIssue + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 5);
        formLayout.Controls.Add(_memberBadgeIssuePicker, 1, 5);
        formLayout.Controls.Add(new Label { Text = Strings.Member_BadgeExpiry + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 2, 5);
        formLayout.Controls.Add(_memberBadgeExpiryPicker, 3, 5);

        // Row 6: Date of Birth | Notes | Checkboxes
        formLayout.Controls.Add(new Label { Text = Strings.Member_DateOfBirth + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = labelFont }, 0, 6);
        formLayout.Controls.Add(_memberDateOfBirthPicker, 1, 6);
        var checkboxPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight };
        checkboxPanel.Controls.Add(_memberPermanentCheckBox);
        checkboxPanel.Controls.Add(_memberActiveCheckBox);
        formLayout.Controls.Add(checkboxPanel, 2, 6);
        formLayout.SetColumnSpan(checkboxPanel, 2);

        // Photo column (spans rows 0-5) - photo preview takes most space
        var photoLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 2
        };
        photoLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Photo takes remaining
        photoLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32)); // Buttons row
        photoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        photoLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        photoLayout.Controls.Add(_memberPhotoBox, 0, 0);
        photoLayout.SetColumnSpan(_memberPhotoBox, 2);
        photoLayout.Controls.Add(_memberChoosePhotoButton, 0, 1);
        photoLayout.Controls.Add(_memberClearPhotoButton, 1, 1);

        formLayout.Controls.Add(photoLayout, 4, 0);
        formLayout.SetRowSpan(photoLayout, 6);

        // Button panel in last column, row 6
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

        formLayout.Controls.Add(buttonLayout, 4, 6);
        formLayout.SetRowSpan(buttonLayout, 1);

        panel.Controls.Add(formLayout);
        return panel;
    }

    private Panel CreateMemberGridPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0)
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

        // Search bar - narrower search, compact buttons
        var searchPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 6,
            Padding = new Padding(5, 0, 5, 0),
            BackColor = SystemColors.Control
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));  // Label
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180)); // Search box
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));   // Export
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));   // Badge PDF
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));   // Select All
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 17));   // Batch Badges

        var searchLabel = new Label { Text = Strings.Search + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberSearchBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _memberSearchBox.TextChanged += OnMemberSearch;

        var exportButton = new Button { Text = Strings.Button_Export, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), UseVisualStyleBackColor = true };
        var badgeButton = new Button { Text = Strings.Button_BadgePdf, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), UseVisualStyleBackColor = true };
        _selectAllMembersButton = new Button { Text = Strings.Button_SelectAll, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), UseVisualStyleBackColor = true };
        var batchBadgeButton = new Button { Text = Strings.Button_BatchBadges, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F), UseVisualStyleBackColor = true };

        exportButton.Click += OnExportMembersClick;
        badgeButton.Click += OnBadgeClick;
        _selectAllMembersButton.Click += OnSelectAllMembersClick;
        batchBadgeButton.Click += OnBatchBadgeClick;

        searchPanel.Controls.Add(searchLabel, 0, 0);
        searchPanel.Controls.Add(_memberSearchBox, 1, 0);
        searchPanel.Controls.Add(exportButton, 2, 0);
        searchPanel.Controls.Add(badgeButton, 3, 0);
        searchPanel.Controls.Add(_selectAllMembersButton, 4, 0);
        searchPanel.Controls.Add(batchBadgeButton, 5, 0);

        // Grid - enable multi-select for batch badge export
        membersGrid.Dock = DockStyle.Fill;
        membersGrid.Font = new Font("Segoe UI", 10F);
        membersGrid.RowTemplate.Height = 30;
        membersGrid.AllowUserToAddRows = false;
        membersGrid.AllowUserToDeleteRows = false;
        membersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        membersGrid.MultiSelect = true;
        membersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        membersGrid.EnableDoubleBuffering();

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
        else
        {
            // No member selected - show defaults for new member
            ClearMemberForm();
        }
    }

    private void OnSelectAllMembersClick(object? sender, EventArgs e)
    {
        _allMembersSelected = !_allMembersSelected;

        if (_allMembersSelected)
        {
            membersGrid.SelectAll();
            _selectAllMembersButton!.Text = Strings.Button_DeselectAll;
        }
        else
        {
            membersGrid.ClearSelection();
            _selectAllMembersButton!.Text = Strings.Button_SelectAll;
        }
    }

    private void PopulateMemberForm(Member member)
    {
        if (_memberNumberValueLabel == null) return;

        _memberNumberValueLabel.Text = member.MemberNumber > 0 ? member.MemberNumber.ToString() : Strings.MemberNumber_Auto;
        _memberFirstNameTextBox!.Text = member.FirstName;
        _memberLastNameTextBox!.Text = member.LastName;
        _memberBusinessRankTextBox!.Text = member.BusinessRank ?? string.Empty;
        _memberPersonalIdTextBox!.Text = member.PersonalIdNumber ?? string.Empty;
        _memberBusinessIdTextBox!.Text = member.BusinessIdNumber ?? string.Empty;
        _memberPermanentCheckBox!.Checked = member.IsPermanentStaff;
        _memberActiveCheckBox!.Checked = member.IsActive;

        // New text fields
        _memberPhoneTextBox!.Text = member.Phone ?? string.Empty;
        _memberEmailTextBox!.Text = member.Email ?? string.Empty;
        _memberNotesTextBox!.Text = member.Notes ?? string.Empty;

        // Date pickers
        if (member.BadgeIssueDate.HasValue)
        {
            _memberBadgeIssuePicker!.Checked = true;
            _memberBadgeIssuePicker.Value = member.BadgeIssueDate.Value;
        }
        else
        {
            _memberBadgeIssuePicker!.Checked = false;
        }

        if (member.BadgeExpiryDate.HasValue)
        {
            _memberBadgeExpiryPicker!.Checked = true;
            _memberBadgeExpiryPicker.Value = member.BadgeExpiryDate.Value;
        }
        else
        {
            _memberBadgeExpiryPicker!.Checked = false;
        }

        if (member.DateOfBirth.HasValue)
        {
            _memberDateOfBirthPicker!.Checked = true;
            _memberDateOfBirthPicker.Value = member.DateOfBirth.Value;
        }
        else
        {
            _memberDateOfBirthPicker!.Checked = false;
        }

        // Set office text field
        _memberOfficeTextBox!.Text = member.Office ?? string.Empty;

        // Set combo box selections (will be populated in OnLoad)
        if (member.MemberTypeId.HasValue)
            _memberTypeComboBox!.SelectedValue = member.MemberTypeId.Value;
        else
            _memberTypeComboBox!.SelectedIndex = -1;

        // Photo - show thumbnail if available
        _memberPhotoPath = member.PhotoFileName;
        if (!string.IsNullOrEmpty(member.PhotoFileName))
        {
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
                    _memberPhotoBox!.Image = Image.FromFile(photoPath);
                }
                catch
                {
                    _memberPhotoBox!.Image = null;
                }
            }
            else
            {
                _memberPhotoBox!.Image = null;
            }
        }
        else
        {
            _memberPhotoBox!.Image = null;
        }
    }

    private void ClearMemberForm()
    {
        if (_memberNumberValueLabel == null) return;

        // Clear grid selection so Save creates new member instead of updating
        membersGrid.ClearSelection();
        membersGrid.CurrentCell = null;

        _memberNumberValueLabel.Text = Strings.MemberNumber_Auto;
        _memberFirstNameTextBox!.Clear();
        _memberLastNameTextBox!.Clear();
        _memberBusinessRankTextBox!.Clear();
        _memberPersonalIdTextBox!.Clear();
        _memberBusinessIdTextBox!.Clear();
        _memberPhoneTextBox!.Clear();
        _memberEmailTextBox!.Clear();
        _memberNotesTextBox!.Clear();
        _memberPermanentCheckBox!.Checked = false;
        _memberActiveCheckBox!.Checked = true;
        _memberDateOfBirthPicker!.Checked = false;
        _memberOfficeTextBox!.Text = string.Empty;
        _memberTypeComboBox!.SelectedIndex = -1;
        _memberPhotoPath = null;
        _memberPhotoBox!.Image = null;

        // Set default badge dates for new member (today + validity months)
        // Must set Checked first, then Value - ValueChanged event checks Checked flag
        _memberBadgeIssuePicker!.Checked = true;
        _memberBadgeIssuePicker.Value = DateTime.Today;
        // OnMemberBadgeIssueDateChanged will auto-calculate expiry
    }

    private void OnMemberBadgeIssueDateChanged(object? sender, EventArgs e)
    {
        if (_memberBadgeIssuePicker == null || _memberBadgeExpiryPicker == null) return;

        // Only auto-calculate if issue date is checked (has value)
        if (!_memberBadgeIssuePicker.Checked) return;

        // Get BadgeValidityMonths from settings
        var validityMonthsSetting = _settingsViewModel.Settings
            .FirstOrDefault(s => s.Key == SettingKeys.BadgeValidityMonths)?.Value;

        if (!int.TryParse(validityMonthsSetting, out var validityMonths) || validityMonths <= 0)
            return;

        // Auto-calculate expiry date
        var expiryDate = _memberBadgeIssuePicker.Value.Date.AddMonths(validityMonths);
        _memberBadgeExpiryPicker.Value = expiryDate;
        _memberBadgeExpiryPicker.Checked = true;
    }

    private void OnMemberNew(object? sender, EventArgs e)
    {
        membersGrid.ClearSelection();
        membersGrid.CurrentCell = null; // Also clear CurrentRow so Save creates new member
        ClearMemberForm();
        _memberFirstNameTextBox?.Focus();
    }

    private async void OnMemberSave(object? sender, EventArgs e)
    {
        if (_memberFirstNameTextBox == null) return;

        // Check if we have a selected member or creating new
        Member member;
        if (membersGrid.CurrentRow?.DataBoundItem is Member existingMember)
        {
            // Editing existing member
            member = existingMember;
        }
        else
        {
            // Creating new member - clear default Id so CreateAsync is called
            // (Member class auto-generates Id in constructor, but we need empty for create)
            member = new Member { Id = string.Empty };
        }
        member.FirstName = _memberFirstNameTextBox!.Text;
        member.LastName = _memberLastNameTextBox!.Text;
        member.BusinessRank = _memberBusinessRankTextBox!.Text;
        member.PersonalIdNumber = _memberPersonalIdTextBox!.Text;
        member.BusinessIdNumber = _memberBusinessIdTextBox!.Text;
        member.Phone = _memberPhoneTextBox!.Text;
        member.Email = _memberEmailTextBox!.Text;
        member.Notes = _memberNotesTextBox!.Text;
        member.IsPermanentStaff = _memberPermanentCheckBox!.Checked;
        member.IsActive = _memberActiveCheckBox!.Checked;
        member.BadgeIssueDate = _memberBadgeIssuePicker!.Checked ? _memberBadgeIssuePicker.Value : null;
        member.BadgeExpiryDate = _memberBadgeExpiryPicker!.Checked ? _memberBadgeExpiryPicker.Value : null;
        member.DateOfBirth = _memberDateOfBirthPicker!.Checked ? _memberDateOfBirthPicker.Value : null;
        member.Office = string.IsNullOrWhiteSpace(_memberOfficeTextBox!.Text) ? null : _memberOfficeTextBox.Text.Trim();
        member.MemberTypeId = _memberTypeComboBox!.SelectedValue as int?;

        // Handle photo: copy to Photos directory if it's an external file
        if (!string.IsNullOrEmpty(_memberPhotoPath))
        {
            if (Path.IsPathRooted(_memberPhotoPath) && File.Exists(_memberPhotoPath))
            {
                // Photo is an external file - copy to Photos directory
                var photosPath = _serviceProvider.GetRequiredService<Data.Context.DatabaseOptions>().GetPhotosPath();
                Directory.CreateDirectory(photosPath);

                // Generate unique filename using GUID to avoid conflicts
                var extension = Path.GetExtension(_memberPhotoPath);
                var newFileName = $"{Guid.NewGuid():N}{extension}";
                var destinationPath = Path.Combine(photosPath, newFileName);

                try
                {
                    File.Copy(_memberPhotoPath, destinationPath, overwrite: true);
                    member.PhotoFileName = newFileName; // Store just the filename
                    _memberPhotoPath = newFileName; // Update local reference to be relative
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Strings.Photo_CopyFailed}: {ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Continue saving without the photo
                    member.PhotoFileName = null;
                }
            }
            else
            {
                // Already a relative filename (existing photo)
                member.PhotoFileName = _memberPhotoPath;
            }
        }
        else
        {
            member.PhotoFileName = null;
        }

        try
        {
            await _membersViewModel.SaveAsync(member);
            await _membersViewModel.LoadAsync();
            _membersBinding = new BindingList<Member>(_membersViewModel.Members);
            membersGrid.DataSource = _membersBinding;

            UpdateStatus(Strings.Status_MemberSaved);
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

            UpdateStatus(Strings.Status_MemberDeleted);
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
            // Search across all visible/useful columns
            var filtered = _membersViewModel.Members.Where(m =>
                m.MemberNumber.ToString().Contains(searchText) ||
                m.FirstName.ToLowerInvariant().Contains(searchText) ||
                m.LastName.ToLowerInvariant().Contains(searchText) ||
                (m.PersonalIdNumber?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.BusinessIdNumber?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.BusinessRank?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.Phone?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.Email?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.Notes?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.Office?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.MemberType?.DisplayName?.ToLowerInvariant().Contains(searchText) ?? false) ||
                (m.MemberType?.Code?.ToLowerInvariant().Contains(searchText) ?? false)
            ).ToList();
            _membersBinding = new BindingList<Member>(filtered);
        }

        membersGrid.DataSource = _membersBinding;
    }

    private void OnMemberChoosePhoto(object? sender, EventArgs e)
    {
        // Try user's desktop first, fall back to MyPictures, then current directory
        var initialDir = GetSafeInitialDirectory();

        using var dialog = new OpenFileDialog
        {
            Filter = Strings.Photo_Filter,
            Title = Strings.Photo_SelectTitle,
            InitialDirectory = initialDir
        };

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            _memberPhotoPath = dialog.FileName;

            // Load and display the image
            try
            {
                _memberPhotoBox!.Image = Image.FromFile(dialog.FileName);
            }
            catch
            {
                // If image can't be loaded, clear the box
                _memberPhotoBox!.Image = null;
            }
        }
    }

    private static string GetSafeInitialDirectory()
    {
        // Try paths in order of preference, with defensive error handling
        string[] pathsToTry =
        [
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            AppContext.BaseDirectory
        ];

        foreach (var path in pathsToTry)
        {
            try
            {
                if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
                    return path;
            }
            catch
            {
                // Access denied or other error, try next path
            }
        }

        return string.Empty; // Let the dialog use its default
    }

    private void OnMemberClearPhoto(object? sender, EventArgs e)
    {
        _memberPhotoPath = null;
        _memberPhotoBox!.Image = null;
    }

    private async void OnMemberPhotoClick(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_memberPhotoPath)) return;

        var photoPath = _memberPhotoPath;
        if (!Path.IsPathRooted(photoPath))
        {
            // Resolve relative path from photos directory
            var photosPath = _serviceProvider.GetRequiredService<Data.Context.DatabaseOptions>().GetPhotosPath();
            photoPath = Path.Combine(photosPath, photoPath);
        }

        if (!File.Exists(photoPath)) return;

        // Open photo in a dialog for better viewing
        using var dialog = new Form
        {
            Text = Strings.Photo_Preview,
            StartPosition = FormStartPosition.CenterParent,
            Size = new Size(500, 650),
            MinimumSize = new Size(300, 400),
            MaximizeBox = true,
            MinimizeBox = false,
            ShowInTaskbar = false
        };

        var pictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            Image = Image.FromFile(photoPath)
        };

        dialog.Controls.Add(pictureBox);
        await dialog.ShowDialogAsync(this);
    }

    // ==================== USERS MASTER-DETAIL VIEW ====================

    /// <summary>
    /// Sets up the Users tab with a fixed form at top, grid at bottom.
    /// Top panel: User form for CRUD operations (fixed height)
    /// Bottom panel: DataGridView with search/filter (takes remaining space)
    /// </summary>
    private void SetupUsersMasterDetailView()
    {
        // Clear existing controls
        usersPanel.Controls.Clear();

        // Create main layout - fixed form at top, grid takes rest
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Padding = new Padding(10)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 220)); // Fixed form height - increased for buttons
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // Grid takes rest

        // === TOP: User Form (fixed height) ===
        var formPanel = CreateUserFormPanel();
        mainLayout.Controls.Add(formPanel, 0, 0);

        // === BOTTOM: Grid with Search (fills remaining space) ===
        var gridPanel = CreateUserGridPanel();
        mainLayout.Controls.Add(gridPanel, 0, 1);

        usersPanel.Controls.Add(mainLayout);

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

        // Auto-uppercase all text fields on leave
        _userUsernameTextBox.Leave += OnTextBoxLeaveUppercase;
        _userFirstNameTextBox.Leave += OnTextBoxLeaveUppercase;
        _userLastNameTextBox.Leave += OnTextBoxLeaveUppercase;

        // Populate role combo box
        _userRoleComboBox.DataSource = Enum.GetValues(typeof(UserRole));

        // Action buttons - fixed height
        _userNewButton = new Button { Text = Strings.Button_New, Height = 33, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold), UseVisualStyleBackColor = true };
        _userSaveButton = new Button { Text = Strings.Button_Save, Height = 33, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F, FontStyle.Bold), UseVisualStyleBackColor = true, ForeColor = Color.Blue };
        _userDeleteButton = new Button { Text = Strings.Button_Delete, Height = 33, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F), UseVisualStyleBackColor = true, ForeColor = Color.Red };

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
            ColumnCount = 5,
            Padding = new Padding(5, 0, 5, 0),
            BackColor = SystemColors.Control
        };
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));  // Label - increased
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 280)); // Search box
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // Spacer
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170)); // Reset Password - increased
        searchPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180)); // Reset Quick Code - increased

        var searchLabel = new Label { Text = Strings.Search + ":", TextAlign = ContentAlignment.MiddleRight, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userSearchBox = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10F) };
        _userSearchBox.TextChanged += OnUserSearch;

        var resetPasswordButton = new Button { Text = Strings.Button_ResetPassword, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
        var resetQuickCodeButton = new Button { Text = Strings.Button_ResetQuickCode, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };

        resetPasswordButton.Click += OnResetPasswordClick;
        resetQuickCodeButton.Click += OnResetQuickCodeClick;

        searchPanel.Controls.Add(searchLabel, 0, 0);
        searchPanel.Controls.Add(_userSearchBox, 1, 0);
        searchPanel.Controls.Add(resetPasswordButton, 3, 0);
        searchPanel.Controls.Add(resetQuickCodeButton, 4, 0);

        // Grid - with AutoSizeColumnsMode.Fill to expand to panel width
        usersGrid.Dock = DockStyle.Fill;
        usersGrid.Font = new Font("Segoe UI", 10F);
        usersGrid.RowTemplate.Height = 30;
        usersGrid.AllowUserToAddRows = false;
        usersGrid.AllowUserToDeleteRows = false;
        usersGrid.ReadOnly = true;
        usersGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        usersGrid.MultiSelect = false;
        usersGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        usersGrid.EnableDoubleBuffering();

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
                if (await passwordDialog.ShowDialogAsync() != DialogResult.OK)
                    return;
                // User will be created via the dialog
            }
            else
            {
                // Update existing user (no password change)
                await _usersViewModel.SaveAsync(user, null, null);
            }

            await LoadUsersAsync();
            UpdateStatus(Strings.Status_UserSaved);
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

            UpdateStatus(Strings.Status_UserDeleted);
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
            { nameof(Member.Office), Strings.Grid_Office },
            { nameof(Member.MemberTypeId), Strings.Grid_MemberTypeId },
            { nameof(Member.MemberType), Strings.Grid_MemberType },
            { nameof(Member.MemberTypeName), Strings.Grid_MemberType },
            { nameof(Member.IsPermanentStaff), Strings.Grid_IsPermanent },
            { nameof(Member.PhotoFileName), Strings.Grid_PhotoPath },
            { nameof(Member.Phone), Strings.Grid_Phone },
            { nameof(Member.Email), Strings.Grid_Email },
            { nameof(Member.DateOfBirth), Strings.Grid_DateOfBirth },
            { nameof(Member.BadgeIssueDate), Strings.Grid_BadgeIssueDate },
            { nameof(Member.BadgeExpiryDate), Strings.Grid_BadgeExpiryDate },
            { nameof(Member.Notes), Strings.Grid_Notes },
            { nameof(Member.IsActive), Strings.Grid_IsActive },
            { nameof(Member.CreatedAtUtc), Strings.Grid_CreatedAt },
            { nameof(Member.UpdatedAtUtc), Strings.Grid_UpdatedAt }
        };

        // Hide internal columns to save horizontal space
        var hiddenColumns = new[] { nameof(Member.Id), nameof(Member.MemberTypeId), nameof(Member.MemberType), nameof(Member.PhotoFileName) };

        foreach (DataGridViewColumn column in membersGrid.Columns)
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
            { nameof(User.IsSystem), Strings.Grid_IsSystem },
            { nameof(User.LastLoginAtUtc), Strings.Grid_LastLogin },
            { nameof(User.CreatedAtUtc), Strings.Grid_CreatedAt }
        };

        // Hide sensitive columns
        var hiddenColumns = new[] { "PasswordHash", "PasswordSalt", "PasswordIterations",
            "QuickCodeHash", "QuickCodeSalt", "QuickCodeIterations" };

        foreach (DataGridViewColumn column in usersGrid.Columns)
        {
            if (columnMappings.TryGetValue(column.DataPropertyName, out var localizedHeader))
            {
                column.HeaderText = localizedHeader;
            }

            // Hide sensitive columns
            if (hiddenColumns.Contains(column.DataPropertyName))
            {
                column.Visible = false;
            }
        }
    }

    /// <summary>
    /// Auto-uppercase text fields when focus leaves.
    /// </summary>
    private void OnTextBoxLeaveUppercase(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox || string.IsNullOrWhiteSpace(textBox.Text))
            return;

        textBox.Text = textBox.Text.ToUpperInvariant();
    }
}
