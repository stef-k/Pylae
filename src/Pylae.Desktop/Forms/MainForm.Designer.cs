using System.Drawing;
using System.Windows.Forms;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.Forms;

partial class MainForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        subtitleLabel = new Label();
        welcomeLabel = new Label();
        siteLabel = new Label();
        tabControl = new TabControl();
        gateTab = new TabPage();
        membersTab = new TabPage();
        visitsTab = new TabPage();
        usersTab = new TabPage();
        badgeWarningLabel = new Label();
        lastResultValueLabel = new Label();
        gateResultLabel = new Label();
        logVisitButton = new Button();
        notesTextBox = new TextBox();
        notesLabel = new Label();
        memberNumberTextBox = new TextBox();
        memberNumberLabel = new Label();
        exitRadio = new RadioButton();
        entryRadio = new RadioButton();
        settingsTab = new TabPage();
        settingsGrid = new DataGridView();
        saveSettingsButton = new Button();
        membersGrid = new DataGridView();
        visitsGrid = new DataGridView();
        usersGrid = new DataGridView();
        addMemberButton = new Button();
        refreshMembersButton = new Button();
        refreshVisitsButton = new Button();
        refreshUsersButton = new Button();
        visitsFromPicker = new DateTimePicker();
        visitsToPicker = new DateTimePicker();
        visitsFilterButton = new Button();
        remoteSitesButton = new Button();
        exportMembersButton = new Button();
        badgeButton = new Button();
        printBadgeButton = new Button();
        exportVisitsButton = new Button();
        addUserButton = new Button();
        editUserButton = new Button();
        deactivateUserButton = new Button();
        deleteUserButton = new Button();
        resetPasswordButton = new Button();
        resetQuickCodeButton = new Button();
        auditTab = new TabPage();
        auditGrid = new DataGridView();
        auditFromPicker = new DateTimePicker();
        auditToPicker = new DateTimePicker();
        auditActionText = new TextBox();
        auditTargetText = new TextBox();
        auditRefreshButton = new Button();
        auditExportButton = new Button();
        openCatalogsButton = new Button();
        backupButton = new Button();
        restoreButton = new Button();
        changePasswordButton = new Button();
        changeQuickCodeButton = new Button();
        tabControl.SuspendLayout();
        gateTab.SuspendLayout();
        membersTab.SuspendLayout();
        usersTab.SuspendLayout();
        visitsTab.SuspendLayout();
        settingsTab.SuspendLayout();
        auditTab.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)settingsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)membersGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)usersGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)visitsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)auditGrid).BeginInit();
        SuspendLayout();
        // 
        // subtitleLabel
        // 
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
        subtitleLabel.Location = new Point(12, 9);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Size = new Size(108, 19);
        subtitleLabel.TabIndex = 0;
        subtitleLabel.Text = "Gate & Control";
        // 
        // welcomeLabel
        // 
        welcomeLabel.AutoSize = true;
        welcomeLabel.Location = new Point(12, 32);
        welcomeLabel.Name = "welcomeLabel";
        welcomeLabel.Size = new Size(57, 15);
        welcomeLabel.TabIndex = 1;
        welcomeLabel.Text = "Welcome";
        // 
        // siteLabel
        // 
        siteLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        siteLabel.AutoSize = true;
        siteLabel.Location = new Point(540, 9);
        siteLabel.Name = "siteLabel";
        siteLabel.Size = new Size(49, 15);
        siteLabel.TabIndex = 2;
        siteLabel.Text = "Site info";
        // 
        // tabControl
        // 
        tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabControl.Controls.Add(gateTab);
        tabControl.Controls.Add(membersTab);
        tabControl.Controls.Add(usersTab);
        tabControl.Controls.Add(visitsTab);
        tabControl.Controls.Add(settingsTab);
        tabControl.Controls.Add(auditTab);
        tabControl.Location = new Point(12, 60);
        tabControl.Name = "tabControl";
        tabControl.SelectedIndex = 0;
        tabControl.Size = new Size(1176, 728);
        tabControl.TabIndex = 3;
        // 
        // gateTab
        // 
        gateTab.Controls.Add(badgeWarningLabel);
        gateTab.Controls.Add(lastResultValueLabel);
        gateTab.Controls.Add(gateResultLabel);
        gateTab.Controls.Add(logVisitButton);
        gateTab.Controls.Add(notesTextBox);
        gateTab.Controls.Add(notesLabel);
        gateTab.Controls.Add(memberNumberTextBox);
        gateTab.Controls.Add(memberNumberLabel);
        gateTab.Controls.Add(exitRadio);
        gateTab.Controls.Add(entryRadio);
        gateTab.Location = new Point(4, 24);
        gateTab.Name = "gateTab";
        gateTab.Padding = new Padding(3);
        gateTab.Size = new Size(632, 352);
        gateTab.TabIndex = 0;
        gateTab.Text = Strings.Tab_Gate;
        gateTab.UseVisualStyleBackColor = true;

        // 
        // membersTab
        // 
        membersTab.Controls.Add(exportMembersButton);
        membersTab.Controls.Add(badgeButton);
        membersTab.Controls.Add(printBadgeButton);
        membersTab.Controls.Add(addMemberButton);
        membersTab.Controls.Add(refreshMembersButton);
        membersTab.Controls.Add(membersGrid);
        membersTab.Location = new Point(4, 24);
        membersTab.Name = "membersTab";
        membersTab.Padding = new Padding(3);
        membersTab.Size = new Size(632, 352);
        membersTab.TabIndex = 1;
        membersTab.Text = Strings.Tab_Members;
        membersTab.UseVisualStyleBackColor = true;
        // 
        // usersTab
        // 
        usersTab.Controls.Add(resetQuickCodeButton);
        usersTab.Controls.Add(resetPasswordButton);
        usersTab.Controls.Add(deleteUserButton);
        usersTab.Controls.Add(deactivateUserButton);
        usersTab.Controls.Add(editUserButton);
        usersTab.Controls.Add(addUserButton);
        usersTab.Controls.Add(refreshUsersButton);
        usersTab.Controls.Add(usersGrid);
        usersTab.Location = new Point(4, 24);
        usersTab.Name = "usersTab";
        usersTab.Padding = new Padding(3);
        usersTab.Size = new Size(632, 352);
        usersTab.TabIndex = 2;
        usersTab.Text = Strings.Tab_Users;
        usersTab.UseVisualStyleBackColor = true;
        // 
        // visitsTab
        // 
        visitsTab.Controls.Add(exportVisitsButton);
        visitsTab.Controls.Add(visitsFilterButton);
        visitsTab.Controls.Add(remoteSitesButton);
        visitsTab.Controls.Add(visitsToPicker);
        visitsTab.Controls.Add(visitsFromPicker);
        visitsTab.Controls.Add(refreshVisitsButton);
        visitsTab.Controls.Add(visitsGrid);
        visitsTab.Location = new Point(4, 24);
        visitsTab.Name = "visitsTab";
        visitsTab.Padding = new Padding(3);
        visitsTab.Size = new Size(632, 352);
        visitsTab.TabIndex = 3;
        visitsTab.Text = Strings.Tab_Visits;
        visitsTab.UseVisualStyleBackColor = true;

        // 
        // auditTab
        // 
        auditTab.Controls.Add(auditExportButton);
        auditTab.Controls.Add(auditRefreshButton);
        auditTab.Controls.Add(auditTargetText);
        auditTab.Controls.Add(auditActionText);
        auditTab.Controls.Add(auditToPicker);
        auditTab.Controls.Add(auditFromPicker);
        auditTab.Controls.Add(auditGrid);
        auditTab.Location = new Point(4, 24);
        auditTab.Name = "auditTab";
        auditTab.Padding = new Padding(3);
        auditTab.Size = new Size(632, 352);
        auditTab.TabIndex = 5;
        auditTab.Text = Strings.Tab_Audit;
        auditTab.UseVisualStyleBackColor = true;
        // 
        // badgeWarningLabel
        // 
        badgeWarningLabel.AutoSize = true;
        badgeWarningLabel.ForeColor = Color.DarkGoldenrod;
        badgeWarningLabel.Location = new Point(20, 290);
        badgeWarningLabel.Name = "badgeWarningLabel";
        badgeWarningLabel.Size = new Size(0, 15);
        badgeWarningLabel.TabIndex = 9;
        // 
        // lastResultValueLabel
        // 
        lastResultValueLabel.AutoSize = true;
        lastResultValueLabel.Location = new Point(98, 260);
        lastResultValueLabel.Name = "lastResultValueLabel";
        lastResultValueLabel.Size = new Size(0, 15);
        lastResultValueLabel.TabIndex = 8;
        // 
        // gateResultLabel
        // 
        gateResultLabel.AutoSize = true;
        gateResultLabel.Location = new Point(20, 260);
        gateResultLabel.Name = "gateResultLabel";
        gateResultLabel.Size = new Size(65, 15);
        gateResultLabel.TabIndex = 7;
        gateResultLabel.Text = "Last result:";
        // 
        // logVisitButton
        // 
        logVisitButton.Location = new Point(20, 210);
        logVisitButton.Name = "logVisitButton";
        logVisitButton.Size = new Size(120, 30);
        logVisitButton.TabIndex = 6;
        logVisitButton.Text = "Log visit";
        logVisitButton.UseVisualStyleBackColor = true;
        logVisitButton.Click += OnLogVisitClick;
        // 
        // notesTextBox
        // 
        notesTextBox.Location = new Point(20, 150);
        notesTextBox.Multiline = true;
        notesTextBox.Name = "notesTextBox";
        notesTextBox.Size = new Size(320, 50);
        notesTextBox.TabIndex = 5;
        // 
        // notesLabel
        // 
        notesLabel.AutoSize = true;
        notesLabel.Location = new Point(20, 132);
        notesLabel.Name = "notesLabel";
        notesLabel.Size = new Size(38, 15);
        notesLabel.TabIndex = 4;
        notesLabel.Text = "Notes";
        // 
        // memberNumberTextBox
        // 
        memberNumberTextBox.Location = new Point(20, 90);
        memberNumberTextBox.Name = "memberNumberTextBox";
        memberNumberTextBox.Size = new Size(200, 23);
        memberNumberTextBox.TabIndex = 3;
        // 
        // memberNumberLabel
        // 
        memberNumberLabel.AutoSize = true;
        memberNumberLabel.Location = new Point(20, 72);
        memberNumberLabel.Name = "memberNumberLabel";
        memberNumberLabel.Size = new Size(97, 15);
        memberNumberLabel.TabIndex = 2;
        memberNumberLabel.Text = "Member number";
        // 
        // exitRadio
        // 
        exitRadio.AutoSize = true;
        exitRadio.Location = new Point(80, 30);
        exitRadio.Name = "exitRadio";
        exitRadio.Size = new Size(42, 19);
        exitRadio.TabIndex = 1;
        exitRadio.Text = "Exit";
        exitRadio.UseVisualStyleBackColor = true;
        // 
        // entryRadio
        // 
        entryRadio.AutoSize = true;
        entryRadio.Checked = true;
        entryRadio.Location = new Point(20, 30);
        entryRadio.Name = "entryRadio";
        entryRadio.Size = new Size(52, 19);
        entryRadio.TabIndex = 0;
        entryRadio.TabStop = true;
        entryRadio.Text = "Entry";
        entryRadio.UseVisualStyleBackColor = true;
        // 
        // settingsTab
        // 
        settingsTab.Controls.Add(openCatalogsButton);
        settingsTab.Controls.Add(changeQuickCodeButton);
        settingsTab.Controls.Add(changePasswordButton);
        settingsTab.Controls.Add(restoreButton);
        settingsTab.Controls.Add(backupButton);
        settingsTab.Controls.Add(saveSettingsButton);
        settingsTab.Controls.Add(settingsGrid);
        settingsTab.Location = new Point(4, 24);
        settingsTab.Name = "settingsTab";
        settingsTab.Padding = new Padding(3);
        settingsTab.Size = new Size(632, 352);
        settingsTab.TabIndex = 4;
        settingsTab.Text = Strings.Tab_Settings;
        settingsTab.UseVisualStyleBackColor = true;
        // 
        // settingsGrid
        // 
        settingsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        settingsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        settingsGrid.Location = new Point(6, 6);
        settingsGrid.Name = "settingsGrid";
        settingsGrid.ReadOnly = false;
        settingsGrid.RowTemplate.Height = 25;
        settingsGrid.Size = new Size(620, 300);
        settingsGrid.TabIndex = 0;

        // 
        // saveSettingsButton
        // 
        saveSettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        saveSettingsButton.Location = new Point(546, 312);
        saveSettingsButton.Name = "saveSettingsButton";
        saveSettingsButton.Size = new Size(80, 30);
        saveSettingsButton.TabIndex = 5;
        saveSettingsButton.Text = "Save";
        saveSettingsButton.UseVisualStyleBackColor = true;
        saveSettingsButton.Click += OnSaveSettingsClick;

        // 
        // openCatalogsButton
        // 
        openCatalogsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        openCatalogsButton.Location = new Point(6, 312);
        openCatalogsButton.Name = "openCatalogsButton";
        openCatalogsButton.Size = new Size(120, 30);
        openCatalogsButton.TabIndex = 2;
        openCatalogsButton.Text = "Catalogs";
        openCatalogsButton.UseVisualStyleBackColor = true;
        openCatalogsButton.Click += OnOpenCatalogsClick;

        // 
        // backupButton
        // 
        backupButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        backupButton.Location = new Point(132, 312);
        backupButton.Name = "backupButton";
        backupButton.Size = new Size(120, 30);
        backupButton.TabIndex = 3;
        backupButton.Text = "Backup";
        backupButton.UseVisualStyleBackColor = true;
        backupButton.Click += OnBackupClick;

        // 
        // restoreButton
        // 
        restoreButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        restoreButton.Location = new Point(258, 312);
        restoreButton.Name = "restoreButton";
        restoreButton.Size = new Size(120, 30);
        restoreButton.TabIndex = 4;
        restoreButton.Text = "Restore";
        restoreButton.UseVisualStyleBackColor = true;
        restoreButton.Click += OnRestoreClick;

        // 
        // changePasswordButton
        // 
        changePasswordButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        changePasswordButton.Location = new Point(6, 312);
        changePasswordButton.Name = "changePasswordButton";
        changePasswordButton.Size = new Size(120, 30);
        changePasswordButton.TabIndex = 1;
        changePasswordButton.Text = "Change password";
        changePasswordButton.UseVisualStyleBackColor = true;
        changePasswordButton.Click += OnChangePasswordClick;

        // 
        // changeQuickCodeButton
        // 
        changeQuickCodeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        changeQuickCodeButton.Location = new Point(132, 312);
        changeQuickCodeButton.Name = "changeQuickCodeButton";
        changeQuickCodeButton.Size = new Size(120, 30);
        changeQuickCodeButton.TabIndex = 2;
        changeQuickCodeButton.Text = "Change quick code";
        changeQuickCodeButton.UseVisualStyleBackColor = true;
        changeQuickCodeButton.Click += OnChangeQuickCodeClick;

        // 
        // membersGrid
        // 
        membersGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        membersGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        membersGrid.Location = new Point(6, 6);
        membersGrid.Name = "membersGrid";
        membersGrid.ReadOnly = true;
        membersGrid.RowTemplate.Height = 25;
        membersGrid.Size = new Size(620, 300);
        membersGrid.TabIndex = 0;

        // 
        // visitsGrid
        // 
        visitsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        visitsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        visitsGrid.Location = new Point(6, 35);
        visitsGrid.Name = "visitsGrid";
        visitsGrid.ReadOnly = true;
        visitsGrid.RowTemplate.Height = 25;
        visitsGrid.Size = new Size(620, 311);
        visitsGrid.TabIndex = 0;
        visitsGrid.CellDoubleClick += visitsGrid_CellDoubleClick;

        // 
        // auditGrid
        // 
        auditGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        auditGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        auditGrid.Location = new Point(6, 64);
        auditGrid.Name = "auditGrid";
        auditGrid.ReadOnly = true;
        auditGrid.RowTemplate.Height = 25;
        auditGrid.Size = new Size(620, 282);
        auditGrid.TabIndex = 0;

        // 
        // addMemberButton
        // 
        addMemberButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        addMemberButton.Location = new Point(6, 312);
        addMemberButton.Name = "addMemberButton";
        addMemberButton.Size = new Size(100, 30);
        addMemberButton.TabIndex = 1;
        addMemberButton.Text = "Add/Edit";
        addMemberButton.UseVisualStyleBackColor = true;
        addMemberButton.Click += OnAddMemberClick;

        // 
        // refreshMembersButton
        // 
        refreshMembersButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        refreshMembersButton.Location = new Point(526, 312);
        refreshMembersButton.Name = "refreshMembersButton";
        refreshMembersButton.Size = new Size(100, 30);
        refreshMembersButton.TabIndex = 2;
        refreshMembersButton.Text = "Refresh";
        refreshMembersButton.UseVisualStyleBackColor = true;
        refreshMembersButton.Click += OnRefreshMembersClick;

        // 
        // exportMembersButton
        // 
        exportMembersButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        exportMembersButton.Location = new Point(112, 312);
        exportMembersButton.Name = "exportMembersButton";
        exportMembersButton.Size = new Size(100, 30);
        exportMembersButton.TabIndex = 3;
        exportMembersButton.Text = "Export";
        exportMembersButton.UseVisualStyleBackColor = true;
        exportMembersButton.Click += OnExportMembersClick;

        // 
        // badgeButton
        // 
        badgeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        badgeButton.Location = new Point(218, 312);
        badgeButton.Name = "badgeButton";
        badgeButton.Size = new Size(100, 30);
        badgeButton.TabIndex = 4;
        badgeButton.Text = "Badge PDF";
        badgeButton.UseVisualStyleBackColor = true;
        badgeButton.Click += OnBadgeClick;

        // 
        // printBadgeButton
        // 
        printBadgeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        printBadgeButton.Location = new Point(324, 312);
        printBadgeButton.Name = "printBadgeButton";
        printBadgeButton.Size = new Size(100, 30);
        printBadgeButton.TabIndex = 5;
        printBadgeButton.Text = "Print badge";
        printBadgeButton.UseVisualStyleBackColor = true;
        printBadgeButton.Click += OnPrintBadgeClick;

        // 
        // usersGrid
        // 
        usersGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        usersGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        usersGrid.Location = new Point(6, 6);
        usersGrid.Name = "usersGrid";
        usersGrid.ReadOnly = true;
        usersGrid.RowTemplate.Height = 25;
        usersGrid.Size = new Size(500, 340);
        usersGrid.TabIndex = 0;

        // 
        // refreshUsersButton
        // 
        refreshUsersButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        refreshUsersButton.Location = new Point(512, 6);
        refreshUsersButton.Name = "refreshUsersButton";
        refreshUsersButton.Size = new Size(110, 27);
        refreshUsersButton.TabIndex = 1;
        refreshUsersButton.Text = "Refresh";
        refreshUsersButton.UseVisualStyleBackColor = true;
        refreshUsersButton.Click += OnRefreshUsersClick;

        // 
        // addUserButton
        // 
        addUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        addUserButton.Location = new Point(512, 39);
        addUserButton.Name = "addUserButton";
        addUserButton.Size = new Size(110, 27);
        addUserButton.TabIndex = 2;
        addUserButton.Text = "Add user";
        addUserButton.UseVisualStyleBackColor = true;
        addUserButton.Click += OnAddUserClick;

        // 
        // editUserButton
        // 
        editUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        editUserButton.Location = new Point(512, 72);
        editUserButton.Name = "editUserButton";
        editUserButton.Size = new Size(110, 27);
        editUserButton.TabIndex = 3;
        editUserButton.Text = "Edit";
        editUserButton.UseVisualStyleBackColor = true;
        editUserButton.Click += OnEditUserClick;

        // 
        // deactivateUserButton
        // 
        deactivateUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        deactivateUserButton.Location = new Point(512, 138);
        deactivateUserButton.Name = "deactivateUserButton";
        deactivateUserButton.Size = new Size(110, 27);
        deactivateUserButton.TabIndex = 5;
        deactivateUserButton.Text = "Deactivate";
        deactivateUserButton.UseVisualStyleBackColor = true;
        deactivateUserButton.Click += OnDeactivateUserClick;

        // 
        // deleteUserButton
        // 
        deleteUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        deleteUserButton.Location = new Point(512, 171);
        deleteUserButton.Name = "deleteUserButton";
        deleteUserButton.Size = new Size(110, 27);
        deleteUserButton.TabIndex = 6;
        deleteUserButton.Text = "Delete";
        deleteUserButton.UseVisualStyleBackColor = true;
        deleteUserButton.Click += OnDeleteUserClick;

        // 
        // resetPasswordButton
        // 
        resetPasswordButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        resetPasswordButton.Location = new Point(512, 105);
        resetPasswordButton.Name = "resetPasswordButton";
        resetPasswordButton.Size = new Size(110, 27);
        resetPasswordButton.TabIndex = 4;
        resetPasswordButton.Text = "Reset password";
        resetPasswordButton.UseVisualStyleBackColor = true;
        resetPasswordButton.Click += OnResetPasswordClick;

        // 
        // resetQuickCodeButton
        // 
        resetQuickCodeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        resetQuickCodeButton.Location = new Point(512, 204);
        resetQuickCodeButton.Name = "resetQuickCodeButton";
        resetQuickCodeButton.Size = new Size(110, 27);
        resetQuickCodeButton.TabIndex = 7;
        resetQuickCodeButton.Text = "Quick code";
        resetQuickCodeButton.UseVisualStyleBackColor = true;
        resetQuickCodeButton.Click += OnResetQuickCodeClick;

        // 
        // refreshVisitsButton
        // 
        refreshVisitsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        refreshVisitsButton.Location = new Point(546, 6);
        refreshVisitsButton.Name = "refreshVisitsButton";
        refreshVisitsButton.Size = new Size(80, 23);
        refreshVisitsButton.TabIndex = 4;
        refreshVisitsButton.Text = "Refresh";
        refreshVisitsButton.UseVisualStyleBackColor = true;
        refreshVisitsButton.Click += OnRefreshVisitsClick;

        // 
        // visitsFromPicker
        // 
        visitsFromPicker.Format = DateTimePickerFormat.Short;
        visitsFromPicker.Location = new Point(6, 6);
        visitsFromPicker.Name = "visitsFromPicker";
        visitsFromPicker.Size = new Size(120, 23);
        visitsFromPicker.TabIndex = 5;

        // 
        // visitsToPicker
        // 
        visitsToPicker.Format = DateTimePickerFormat.Short;
        visitsToPicker.Location = new Point(132, 6);
        visitsToPicker.Name = "visitsToPicker";
        visitsToPicker.Size = new Size(120, 23);
        visitsToPicker.TabIndex = 6;

        // 
        // visitsFilterButton
        // 
        visitsFilterButton.Location = new Point(258, 6);
        visitsFilterButton.Name = "visitsFilterButton";
        visitsFilterButton.Size = new Size(80, 23);
        visitsFilterButton.TabIndex = 7;
        visitsFilterButton.Text = "Filter";
        visitsFilterButton.UseVisualStyleBackColor = true;
        visitsFilterButton.Click += OnFilterVisitsClick;

        // 
        // exportVisitsButton
        // 
        exportVisitsButton.Location = new Point(450, 6);
        exportVisitsButton.Name = "exportVisitsButton";
        exportVisitsButton.Size = new Size(90, 23);
        exportVisitsButton.TabIndex = 9;
        exportVisitsButton.Text = "Export";
        exportVisitsButton.UseVisualStyleBackColor = true;
        exportVisitsButton.Click += OnExportVisitsClick;

        // 
        // auditFromPicker
        // 
        auditFromPicker.Format = DateTimePickerFormat.Short;
        auditFromPicker.Location = new Point(6, 6);
        auditFromPicker.Name = "auditFromPicker";
        auditFromPicker.Size = new Size(120, 23);
        auditFromPicker.TabIndex = 1;

        // 
        // auditToPicker
        // 
        auditToPicker.Format = DateTimePickerFormat.Short;
        auditToPicker.Location = new Point(132, 6);
        auditToPicker.Name = "auditToPicker";
        auditToPicker.Size = new Size(120, 23);
        auditToPicker.TabIndex = 2;

        // 
        // auditActionText
        // 
        auditActionText.Location = new Point(270, 6);
        auditActionText.Name = "auditActionText";
        auditActionText.PlaceholderText = "Action type";
        auditActionText.Size = new Size(120, 23);
        auditActionText.TabIndex = 3;

        // 
        // auditTargetText
        // 
        auditTargetText.Location = new Point(396, 6);
        auditTargetText.Name = "auditTargetText";
        auditTargetText.PlaceholderText = "Target type";
        auditTargetText.Size = new Size(120, 23);
        auditTargetText.TabIndex = 4;

        // 
        // auditRefreshButton
        // 
        auditRefreshButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        auditRefreshButton.Location = new Point(522, 6);
        auditRefreshButton.Name = "auditRefreshButton";
        auditRefreshButton.Size = new Size(104, 23);
        auditRefreshButton.TabIndex = 5;
        auditRefreshButton.Text = "Refresh";
        auditRefreshButton.UseVisualStyleBackColor = true;
        auditRefreshButton.Click += OnRefreshAuditClick;

        // 
        // auditExportButton
        // 
        auditExportButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        auditExportButton.Location = new Point(522, 35);
        auditExportButton.Name = "auditExportButton";
        auditExportButton.Size = new Size(104, 23);
        auditExportButton.TabIndex = 6;
        auditExportButton.Text = "Export";
        auditExportButton.UseVisualStyleBackColor = true;
        auditExportButton.Click += OnExportAuditClick;
        // 
        // remoteSitesButton
        // 
        remoteSitesButton.Location = new Point(344, 6);
        remoteSitesButton.Name = "remoteSitesButton";
        remoteSitesButton.Size = new Size(100, 23);
        remoteSitesButton.TabIndex = 8;
        remoteSitesButton.Text = "Remote sites";
        remoteSitesButton.UseVisualStyleBackColor = true;
        remoteSitesButton.Click += OnRemoteSitesClick;

        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 800);
        Controls.Add(tabControl);
        Controls.Add(siteLabel);
        Controls.Add(welcomeLabel);
        Controls.Add(subtitleLabel);
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Text = "Pylae";
        tabControl.ResumeLayout(false);
        gateTab.ResumeLayout(false);
        gateTab.PerformLayout();
        membersTab.ResumeLayout(false);
        usersTab.ResumeLayout(false);
        visitsTab.ResumeLayout(false);
        settingsTab.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)settingsGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)membersGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)usersGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)visitsGrid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label subtitleLabel;
    private Label welcomeLabel;
    private Label siteLabel;
    private TabControl tabControl;
    private TabPage gateTab;
    private TabPage settingsTab;
    private TabPage membersTab;
    private TabPage usersTab;
    private TabPage visitsTab;
    private RadioButton exitRadio;
    private RadioButton entryRadio;
    private Label memberNumberLabel;
    private TextBox memberNumberTextBox;
    private TextBox notesTextBox;
    private Label notesLabel;
    private Button logVisitButton;
    private Label gateResultLabel;
    private Label lastResultValueLabel;
    private Label badgeWarningLabel;
    private DataGridView settingsGrid;
    private DataGridView membersGrid;
    private DataGridView usersGrid;
    private DataGridView visitsGrid;
    private Button saveSettingsButton;
    private Button addMemberButton;
    private Button refreshMembersButton;
    private Button refreshVisitsButton;
    private Button refreshUsersButton;
    private DateTimePicker visitsFromPicker;
    private DateTimePicker visitsToPicker;
    private Button visitsFilterButton;
    private Button remoteSitesButton;
    private Button exportMembersButton;
    private Button badgeButton;
    private Button printBadgeButton;
    private Button exportVisitsButton;
    private Button addUserButton;
    private Button editUserButton;
    private Button deactivateUserButton;
    private Button deleteUserButton;
    private Button resetPasswordButton;
    private Button resetQuickCodeButton;
    private TabPage auditTab;
    private DataGridView auditGrid;
    private DateTimePicker auditFromPicker;
    private DateTimePicker auditToPicker;
    private TextBox auditActionText;
    private TextBox auditTargetText;
    private Button auditRefreshButton;
    private Button auditExportButton;
    private Button openCatalogsButton;
    private Button backupButton;
    private Button restoreButton;
    private Button changePasswordButton;
    private Button changeQuickCodeButton;
}
