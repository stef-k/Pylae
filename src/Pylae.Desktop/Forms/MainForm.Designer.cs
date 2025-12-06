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

        // Define fonts
        var labelFont = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        var inputFont = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        var buttonFont = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        var headerFont = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);

        // Menu strip
        mainMenuStrip = new MenuStrip();
        fileMenu = new ToolStripMenuItem();
        fileMainMenuItem = new ToolStripMenuItem();
        fileVisitsMenuItem = new ToolStripMenuItem();
        fileSeparator = new ToolStripSeparator();
        fileExitMenuItem = new ToolStripMenuItem();
        adminMenu = new ToolStripMenuItem();
        adminBadgesMenuItem = new ToolStripMenuItem();
        adminUsersMenuItem = new ToolStripMenuItem();
        adminSettingsMenuItem = new ToolStripMenuItem();
        adminAuditMenuItem = new ToolStripMenuItem();
        helpMenu = new ToolStripMenuItem();
        helpUserGuideMenuItem = new ToolStripMenuItem();
        helpAdminGuideMenuItem = new ToolStripMenuItem();
        helpSeparator = new ToolStripSeparator();
        helpAboutMenuItem = new ToolStripMenuItem();

        // Header panel
        headerPanel = new Panel();
        subtitleLabel = new Label();
        welcomeLabel = new Label();
        siteLabel = new Label();

        // Content panels (one for each view)
        gatePanel = new Panel();
        membersPanel = new Panel();
        usersPanel = new Panel();
        visitsPanel = new Panel();
        settingsPanel = new Panel();
        auditPanel = new Panel();

        // Gate panel controls
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
        directionGroupBox = new GroupBox();

        // Members panel controls
        membersGrid = new DataGridView();
        addMemberButton = new Button();
        refreshMembersButton = new Button();
        exportMembersButton = new Button();
        badgeButton = new Button();
        printBadgeButton = new Button();

        // Users panel controls
        usersGrid = new DataGridView();
        refreshUsersButton = new Button();
        addUserButton = new Button();
        editUserButton = new Button();
        deactivateUserButton = new Button();
        deleteUserButton = new Button();
        resetPasswordButton = new Button();
        resetQuickCodeButton = new Button();

        // Visits panel controls
        visitsGrid = new DataGridView();
        refreshVisitsButton = new Button();
        visitsFromPicker = new DateTimePicker();
        visitsToPicker = new DateTimePicker();
        visitsFilterButton = new Button();
        remoteSitesButton = new Button();
        exportVisitsButton = new Button();

        // Settings panel controls
        settingsGrid = new DataGridView();
        saveSettingsButton = new Button();
        openCatalogsButton = new Button();
        backupButton = new Button();
        restoreButton = new Button();

        // Audit panel controls
        auditGrid = new DataGridView();
        auditFromPicker = new DateTimePicker();
        auditToPicker = new DateTimePicker();
        auditActionText = new TextBox();
        auditTargetText = new TextBox();
        auditRefreshButton = new Button();
        auditExportButton = new Button();

        mainMenuStrip.SuspendLayout();
        headerPanel.SuspendLayout();
        gatePanel.SuspendLayout();
        directionGroupBox.SuspendLayout();
        membersPanel.SuspendLayout();
        usersPanel.SuspendLayout();
        visitsPanel.SuspendLayout();
        settingsPanel.SuspendLayout();
        auditPanel.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)settingsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)membersGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)usersGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)visitsGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)auditGrid).BeginInit();
        SuspendLayout();

        //
        // mainMenuStrip
        //
        mainMenuStrip.Font = buttonFont;
        mainMenuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, adminMenu, helpMenu });
        mainMenuStrip.Location = new Point(0, 0);
        mainMenuStrip.Name = "mainMenuStrip";
        mainMenuStrip.Size = new Size(1200, 28);
        mainMenuStrip.TabIndex = 0;

        //
        // fileMenu
        //
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { fileMainMenuItem, fileVisitsMenuItem, fileSeparator, fileExitMenuItem });
        fileMenu.Name = "fileMenu";
        fileMenu.Text = Strings.Menu_File;

        //
        // fileMainMenuItem
        //
        fileMainMenuItem.Name = "fileMainMenuItem";
        fileMainMenuItem.ShortcutKeys = Keys.F1;
        fileMainMenuItem.Text = Strings.Menu_MainPage;
        fileMainMenuItem.Click += OnMenuMainPageClick;

        //
        // fileVisitsMenuItem
        //
        fileVisitsMenuItem.Name = "fileVisitsMenuItem";
        fileVisitsMenuItem.ShortcutKeys = Keys.F2;
        fileVisitsMenuItem.Text = Strings.Menu_Visits;
        fileVisitsMenuItem.Click += OnMenuVisitsClick;

        //
        // fileSeparator
        //
        fileSeparator.Name = "fileSeparator";

        //
        // fileExitMenuItem
        //
        fileExitMenuItem.Name = "fileExitMenuItem";
        fileExitMenuItem.ShortcutKeys = Keys.Alt | Keys.F4;
        fileExitMenuItem.Text = Strings.Menu_Exit;
        fileExitMenuItem.Click += OnMenuExitClick;

        //
        // adminMenu
        //
        adminMenu.DropDownItems.AddRange(new ToolStripItem[] { adminBadgesMenuItem, adminUsersMenuItem, adminSettingsMenuItem, adminAuditMenuItem });
        adminMenu.Name = "adminMenu";
        adminMenu.Text = Strings.Menu_Admin;

        //
        // adminBadgesMenuItem
        //
        adminBadgesMenuItem.Name = "adminBadgesMenuItem";
        adminBadgesMenuItem.ShortcutKeys = Keys.F3;
        adminBadgesMenuItem.Text = Strings.Menu_Badges;
        adminBadgesMenuItem.Click += OnMenuBadgesClick;

        //
        // adminUsersMenuItem
        //
        adminUsersMenuItem.Name = "adminUsersMenuItem";
        adminUsersMenuItem.ShortcutKeys = Keys.F4;
        adminUsersMenuItem.Text = Strings.Menu_Users;
        adminUsersMenuItem.Click += OnMenuUsersClick;

        //
        // adminSettingsMenuItem
        //
        adminSettingsMenuItem.Name = "adminSettingsMenuItem";
        adminSettingsMenuItem.ShortcutKeys = Keys.F5;
        adminSettingsMenuItem.Text = Strings.Menu_Settings;
        adminSettingsMenuItem.Click += OnMenuSettingsClick;

        //
        // adminAuditMenuItem
        //
        adminAuditMenuItem.Name = "adminAuditMenuItem";
        adminAuditMenuItem.ShortcutKeys = Keys.F6;
        adminAuditMenuItem.Text = Strings.Menu_Audit;
        adminAuditMenuItem.Click += OnMenuAuditClick;

        //
        // helpMenu
        //
        helpMenu.DropDownItems.AddRange(new ToolStripItem[] { helpUserGuideMenuItem, helpAdminGuideMenuItem, helpSeparator, helpAboutMenuItem });
        helpMenu.Name = "helpMenu";
        helpMenu.Text = Strings.Menu_Help;

        //
        // helpUserGuideMenuItem
        //
        helpUserGuideMenuItem.Name = "helpUserGuideMenuItem";
        helpUserGuideMenuItem.Text = Strings.Menu_UserGuide;
        helpUserGuideMenuItem.Click += OnMenuUserGuideClick;

        //
        // helpAdminGuideMenuItem
        //
        helpAdminGuideMenuItem.Name = "helpAdminGuideMenuItem";
        helpAdminGuideMenuItem.Text = Strings.Menu_AdminGuide;
        helpAdminGuideMenuItem.Click += OnMenuAdminGuideClick;

        //
        // helpSeparator
        //
        helpSeparator.Name = "helpSeparator";

        //
        // helpAboutMenuItem
        //
        helpAboutMenuItem.Name = "helpAboutMenuItem";
        helpAboutMenuItem.Text = Strings.Menu_About;
        helpAboutMenuItem.Click += OnMenuAboutClick;

        //
        // headerPanel - single row with site info on right
        //
        headerPanel.Controls.Add(welcomeLabel);
        headerPanel.Controls.Add(siteLabel);
        headerPanel.Dock = DockStyle.Top;
        headerPanel.Location = new Point(0, 28);
        headerPanel.Name = "headerPanel";
        headerPanel.Padding = new Padding(12, 4, 12, 4);
        headerPanel.Size = new Size(1200, 28);
        headerPanel.TabIndex = 1;

        //
        // subtitleLabel - hidden (kept for compatibility)
        //
        subtitleLabel.AutoSize = true;
        subtitleLabel.Font = headerFont;
        subtitleLabel.Location = new Point(12, 4);
        subtitleLabel.Name = "subtitleLabel";
        subtitleLabel.Size = new Size(0, 0);
        subtitleLabel.TabIndex = 0;
        subtitleLabel.Visible = false;

        //
        // welcomeLabel - positioned at right, before siteLabel
        //
        welcomeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        welcomeLabel.AutoSize = true;
        welcomeLabel.Font = labelFont;
        welcomeLabel.Location = new Point(900, 4);
        welcomeLabel.Name = "welcomeLabel";
        welcomeLabel.Size = new Size(57, 20);
        welcomeLabel.TabIndex = 1;
        welcomeLabel.Text = "Welcome";
        welcomeLabel.TextAlign = ContentAlignment.MiddleRight;

        //
        // siteLabel - hidden (info now in welcomeLabel)
        //
        siteLabel.AutoSize = true;
        siteLabel.Font = labelFont;
        siteLabel.Location = new Point(0, 0);
        siteLabel.Name = "siteLabel";
        siteLabel.Size = new Size(0, 0);
        siteLabel.TabIndex = 2;
        siteLabel.Visible = false;

        //
        // gatePanel - Main check-in/check-out view
        //
        gatePanel.Controls.Add(directionGroupBox);
        gatePanel.Controls.Add(memberNumberLabel);
        gatePanel.Controls.Add(memberNumberTextBox);
        gatePanel.Controls.Add(notesLabel);
        gatePanel.Controls.Add(notesTextBox);
        gatePanel.Controls.Add(logVisitButton);
        gatePanel.Controls.Add(gateResultLabel);
        gatePanel.Controls.Add(lastResultValueLabel);
        gatePanel.Controls.Add(badgeWarningLabel);
        gatePanel.Dock = DockStyle.Fill;
        gatePanel.Location = new Point(0, 78);
        gatePanel.Name = "gatePanel";
        gatePanel.Padding = new Padding(20);
        gatePanel.Size = new Size(1200, 722);
        gatePanel.TabIndex = 2;
        gatePanel.Visible = true;

        //
        // directionGroupBox
        //
        directionGroupBox.Controls.Add(entryRadio);
        directionGroupBox.Controls.Add(exitRadio);
        directionGroupBox.Font = labelFont;
        directionGroupBox.Location = new Point(20, 20);
        directionGroupBox.Name = "directionGroupBox";
        directionGroupBox.Padding = new Padding(10);
        directionGroupBox.Size = new Size(300, 70);
        directionGroupBox.TabIndex = 0;
        directionGroupBox.TabStop = false;
        directionGroupBox.Text = Strings.Gate_Direction;

        //
        // entryRadio
        //
        entryRadio.AutoSize = true;
        entryRadio.Checked = true;
        entryRadio.Font = labelFont;
        entryRadio.Location = new Point(20, 30);
        entryRadio.Name = "entryRadio";
        entryRadio.Size = new Size(100, 24);
        entryRadio.TabIndex = 0;
        entryRadio.TabStop = true;
        entryRadio.Text = Strings.Gate_Entry;
        entryRadio.UseVisualStyleBackColor = true;

        //
        // exitRadio
        //
        exitRadio.AutoSize = true;
        exitRadio.Font = labelFont;
        exitRadio.Location = new Point(150, 30);
        exitRadio.Name = "exitRadio";
        exitRadio.Size = new Size(100, 24);
        exitRadio.TabIndex = 1;
        exitRadio.Text = Strings.Gate_Exit;
        exitRadio.UseVisualStyleBackColor = true;

        //
        // memberNumberLabel
        //
        memberNumberLabel.AutoSize = true;
        memberNumberLabel.Font = labelFont;
        memberNumberLabel.Location = new Point(20, 110);
        memberNumberLabel.Name = "memberNumberLabel";
        memberNumberLabel.Size = new Size(120, 20);
        memberNumberLabel.TabIndex = 1;
        memberNumberLabel.Text = Strings.Gate_MemberNumber;

        //
        // memberNumberTextBox
        //
        memberNumberTextBox.Font = inputFont;
        memberNumberTextBox.Location = new Point(20, 135);
        memberNumberTextBox.Name = "memberNumberTextBox";
        memberNumberTextBox.Size = new Size(300, 27);
        memberNumberTextBox.TabIndex = 2;
        memberNumberTextBox.KeyDown += OnMemberNumberKeyDown;

        //
        // notesLabel
        //
        notesLabel.AutoSize = true;
        notesLabel.Font = labelFont;
        notesLabel.Location = new Point(20, 180);
        notesLabel.Name = "notesLabel";
        notesLabel.Size = new Size(50, 20);
        notesLabel.TabIndex = 3;
        notesLabel.Text = Strings.Gate_Notes;

        //
        // notesTextBox
        //
        notesTextBox.Font = inputFont;
        notesTextBox.Location = new Point(20, 205);
        notesTextBox.Multiline = true;
        notesTextBox.Name = "notesTextBox";
        notesTextBox.Size = new Size(400, 80);
        notesTextBox.TabIndex = 4;

        //
        // logVisitButton
        //
        logVisitButton.AutoSize = true;
        logVisitButton.Font = buttonFont;
        logVisitButton.Location = new Point(20, 305);
        logVisitButton.MinimumSize = new Size(150, 40);
        logVisitButton.Name = "logVisitButton";
        logVisitButton.Padding = new Padding(10, 5, 10, 5);
        logVisitButton.Size = new Size(150, 40);
        logVisitButton.TabIndex = 5;
        logVisitButton.Text = Strings.Gate_LogVisit;
        logVisitButton.UseVisualStyleBackColor = true;
        logVisitButton.Click += OnLogVisitClick;

        //
        // gateResultLabel
        //
        gateResultLabel.AutoSize = true;
        gateResultLabel.Font = labelFont;
        gateResultLabel.Location = new Point(20, 365);
        gateResultLabel.Name = "gateResultLabel";
        gateResultLabel.Size = new Size(80, 20);
        gateResultLabel.TabIndex = 6;
        gateResultLabel.Text = Strings.Gate_LastResult;

        //
        // lastResultValueLabel
        //
        lastResultValueLabel.AutoSize = true;
        lastResultValueLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastResultValueLabel.Location = new Point(120, 365);
        lastResultValueLabel.Name = "lastResultValueLabel";
        lastResultValueLabel.Size = new Size(0, 20);
        lastResultValueLabel.TabIndex = 7;

        //
        // badgeWarningLabel
        //
        badgeWarningLabel.AutoSize = true;
        badgeWarningLabel.Font = labelFont;
        badgeWarningLabel.ForeColor = Color.DarkGoldenrod;
        badgeWarningLabel.Location = new Point(20, 400);
        badgeWarningLabel.Name = "badgeWarningLabel";
        badgeWarningLabel.Size = new Size(0, 20);
        badgeWarningLabel.TabIndex = 8;

        //
        // membersPanel - Badges/Members view
        //
        membersPanel.Controls.Add(membersGrid);
        membersPanel.Controls.Add(addMemberButton);
        membersPanel.Controls.Add(exportMembersButton);
        membersPanel.Controls.Add(badgeButton);
        membersPanel.Controls.Add(printBadgeButton);
        membersPanel.Controls.Add(refreshMembersButton);
        membersPanel.Dock = DockStyle.Fill;
        membersPanel.Location = new Point(0, 78);
        membersPanel.Name = "membersPanel";
        membersPanel.Padding = new Padding(10);
        membersPanel.Size = new Size(1200, 722);
        membersPanel.TabIndex = 3;
        membersPanel.Visible = false;

        //
        // membersGrid
        //
        membersGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        membersGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        membersGrid.Location = new Point(10, 10);
        membersGrid.Name = "membersGrid";
        membersGrid.ReadOnly = true;
        membersGrid.RowTemplate.Height = 25;
        membersGrid.Size = new Size(1180, 660);
        membersGrid.TabIndex = 0;

        //
        // addMemberButton
        //
        addMemberButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        addMemberButton.AutoSize = true;
        addMemberButton.Font = buttonFont;
        addMemberButton.Location = new Point(10, 680);
        addMemberButton.MinimumSize = new Size(120, 35);
        addMemberButton.Name = "addMemberButton";
        addMemberButton.Padding = new Padding(8, 3, 8, 3);
        addMemberButton.Size = new Size(120, 35);
        addMemberButton.TabIndex = 1;
        addMemberButton.Text = Strings.Button_AddEdit;
        addMemberButton.UseVisualStyleBackColor = true;
        addMemberButton.Click += OnAddMemberClick;

        //
        // exportMembersButton
        //
        exportMembersButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        exportMembersButton.AutoSize = true;
        exportMembersButton.Font = buttonFont;
        exportMembersButton.Location = new Point(140, 680);
        exportMembersButton.MinimumSize = new Size(120, 35);
        exportMembersButton.Name = "exportMembersButton";
        exportMembersButton.Padding = new Padding(8, 3, 8, 3);
        exportMembersButton.Size = new Size(120, 35);
        exportMembersButton.TabIndex = 2;
        exportMembersButton.Text = Strings.Button_Export;
        exportMembersButton.UseVisualStyleBackColor = true;
        exportMembersButton.Click += OnExportMembersClick;

        //
        // badgeButton
        //
        badgeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        badgeButton.AutoSize = true;
        badgeButton.Font = buttonFont;
        badgeButton.Location = new Point(270, 680);
        badgeButton.MinimumSize = new Size(120, 35);
        badgeButton.Name = "badgeButton";
        badgeButton.Padding = new Padding(8, 3, 8, 3);
        badgeButton.Size = new Size(120, 35);
        badgeButton.TabIndex = 3;
        badgeButton.Text = Strings.Button_BadgePdf;
        badgeButton.UseVisualStyleBackColor = true;
        badgeButton.Click += OnBadgeClick;

        //
        // printBadgeButton
        //
        printBadgeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        printBadgeButton.AutoSize = true;
        printBadgeButton.Font = buttonFont;
        printBadgeButton.Location = new Point(400, 680);
        printBadgeButton.MinimumSize = new Size(120, 35);
        printBadgeButton.Name = "printBadgeButton";
        printBadgeButton.Padding = new Padding(8, 3, 8, 3);
        printBadgeButton.Size = new Size(120, 35);
        printBadgeButton.TabIndex = 4;
        printBadgeButton.Text = Strings.Button_PrintBadge;
        printBadgeButton.UseVisualStyleBackColor = true;
        printBadgeButton.Click += OnPrintBadgeClick;

        //
        // refreshMembersButton
        //
        refreshMembersButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        refreshMembersButton.AutoSize = true;
        refreshMembersButton.Font = buttonFont;
        refreshMembersButton.Location = new Point(1070, 680);
        refreshMembersButton.MinimumSize = new Size(120, 35);
        refreshMembersButton.Name = "refreshMembersButton";
        refreshMembersButton.Padding = new Padding(8, 3, 8, 3);
        refreshMembersButton.Size = new Size(120, 35);
        refreshMembersButton.TabIndex = 5;
        refreshMembersButton.Text = Strings.Button_Refresh;
        refreshMembersButton.UseVisualStyleBackColor = true;
        refreshMembersButton.Click += OnRefreshMembersClick;

        //
        // usersPanel - Users management view
        //
        usersPanel.Controls.Add(usersGrid);
        usersPanel.Controls.Add(refreshUsersButton);
        usersPanel.Controls.Add(addUserButton);
        usersPanel.Controls.Add(editUserButton);
        usersPanel.Controls.Add(resetPasswordButton);
        usersPanel.Controls.Add(deactivateUserButton);
        usersPanel.Controls.Add(deleteUserButton);
        usersPanel.Controls.Add(resetQuickCodeButton);
        usersPanel.Dock = DockStyle.Fill;
        usersPanel.Location = new Point(0, 78);
        usersPanel.Name = "usersPanel";
        usersPanel.Padding = new Padding(10);
        usersPanel.Size = new Size(1200, 722);
        usersPanel.TabIndex = 4;
        usersPanel.Visible = false;

        //
        // usersGrid
        //
        usersGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        usersGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        usersGrid.Location = new Point(10, 10);
        usersGrid.Name = "usersGrid";
        usersGrid.ReadOnly = true;
        usersGrid.RowTemplate.Height = 25;
        usersGrid.Size = new Size(1040, 700);
        usersGrid.TabIndex = 0;

        //
        // refreshUsersButton
        //
        refreshUsersButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        refreshUsersButton.AutoSize = true;
        refreshUsersButton.Font = buttonFont;
        refreshUsersButton.Location = new Point(1060, 10);
        refreshUsersButton.MinimumSize = new Size(130, 35);
        refreshUsersButton.Name = "refreshUsersButton";
        refreshUsersButton.Padding = new Padding(8, 3, 8, 3);
        refreshUsersButton.Size = new Size(130, 35);
        refreshUsersButton.TabIndex = 1;
        refreshUsersButton.Text = Strings.Button_Refresh;
        refreshUsersButton.UseVisualStyleBackColor = true;
        refreshUsersButton.Click += OnRefreshUsersClick;

        //
        // addUserButton
        //
        addUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        addUserButton.AutoSize = true;
        addUserButton.Font = buttonFont;
        addUserButton.Location = new Point(1060, 55);
        addUserButton.MinimumSize = new Size(130, 35);
        addUserButton.Name = "addUserButton";
        addUserButton.Padding = new Padding(8, 3, 8, 3);
        addUserButton.Size = new Size(130, 35);
        addUserButton.TabIndex = 2;
        addUserButton.Text = Strings.Button_AddUser;
        addUserButton.UseVisualStyleBackColor = true;
        addUserButton.Click += OnAddUserClick;

        //
        // editUserButton
        //
        editUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        editUserButton.AutoSize = true;
        editUserButton.Font = buttonFont;
        editUserButton.Location = new Point(1060, 100);
        editUserButton.MinimumSize = new Size(130, 35);
        editUserButton.Name = "editUserButton";
        editUserButton.Padding = new Padding(8, 3, 8, 3);
        editUserButton.Size = new Size(130, 35);
        editUserButton.TabIndex = 3;
        editUserButton.Text = Strings.Button_Edit;
        editUserButton.UseVisualStyleBackColor = true;
        editUserButton.Click += OnEditUserClick;

        //
        // resetPasswordButton
        //
        resetPasswordButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        resetPasswordButton.AutoSize = true;
        resetPasswordButton.Font = buttonFont;
        resetPasswordButton.Location = new Point(1060, 145);
        resetPasswordButton.MinimumSize = new Size(130, 35);
        resetPasswordButton.Name = "resetPasswordButton";
        resetPasswordButton.Padding = new Padding(8, 3, 8, 3);
        resetPasswordButton.Size = new Size(130, 35);
        resetPasswordButton.TabIndex = 4;
        resetPasswordButton.Text = Strings.Button_ResetPassword;
        resetPasswordButton.UseVisualStyleBackColor = true;
        resetPasswordButton.Click += OnResetPasswordClick;

        //
        // deactivateUserButton
        //
        deactivateUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        deactivateUserButton.AutoSize = true;
        deactivateUserButton.Font = buttonFont;
        deactivateUserButton.Location = new Point(1060, 190);
        deactivateUserButton.MinimumSize = new Size(130, 35);
        deactivateUserButton.Name = "deactivateUserButton";
        deactivateUserButton.Padding = new Padding(8, 3, 8, 3);
        deactivateUserButton.Size = new Size(130, 35);
        deactivateUserButton.TabIndex = 5;
        deactivateUserButton.Text = Strings.Button_Deactivate;
        deactivateUserButton.UseVisualStyleBackColor = true;
        deactivateUserButton.Click += OnDeactivateUserClick;

        //
        // deleteUserButton
        //
        deleteUserButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        deleteUserButton.AutoSize = true;
        deleteUserButton.Font = buttonFont;
        deleteUserButton.ForeColor = Color.Red;
        deleteUserButton.Location = new Point(1060, 235);
        deleteUserButton.MinimumSize = new Size(130, 35);
        deleteUserButton.Name = "deleteUserButton";
        deleteUserButton.Padding = new Padding(8, 3, 8, 3);
        deleteUserButton.Size = new Size(130, 35);
        deleteUserButton.TabIndex = 6;
        deleteUserButton.Text = Strings.Button_Delete;
        deleteUserButton.UseVisualStyleBackColor = true;
        deleteUserButton.Click += OnDeleteUserClick;

        //
        // resetQuickCodeButton
        //
        resetQuickCodeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        resetQuickCodeButton.AutoSize = true;
        resetQuickCodeButton.Font = buttonFont;
        resetQuickCodeButton.Location = new Point(1060, 280);
        resetQuickCodeButton.MinimumSize = new Size(130, 35);
        resetQuickCodeButton.Name = "resetQuickCodeButton";
        resetQuickCodeButton.Padding = new Padding(8, 3, 8, 3);
        resetQuickCodeButton.Size = new Size(130, 35);
        resetQuickCodeButton.TabIndex = 7;
        resetQuickCodeButton.Text = Strings.Button_QuickCode;
        resetQuickCodeButton.UseVisualStyleBackColor = true;
        resetQuickCodeButton.Click += OnResetQuickCodeClick;

        //
        // visitsPanel - Visits view
        //
        visitsPanel.Controls.Add(visitsFromPicker);
        visitsPanel.Controls.Add(visitsToPicker);
        visitsPanel.Controls.Add(visitsFilterButton);
        visitsPanel.Controls.Add(remoteSitesButton);
        visitsPanel.Controls.Add(exportVisitsButton);
        visitsPanel.Controls.Add(refreshVisitsButton);
        visitsPanel.Controls.Add(visitsGrid);
        visitsPanel.Dock = DockStyle.Fill;
        visitsPanel.Location = new Point(0, 78);
        visitsPanel.Name = "visitsPanel";
        visitsPanel.Padding = new Padding(10);
        visitsPanel.Size = new Size(1200, 722);
        visitsPanel.TabIndex = 5;
        visitsPanel.Visible = false;

        //
        // visitsFromPicker
        //
        visitsFromPicker.Font = inputFont;
        visitsFromPicker.Format = DateTimePickerFormat.Short;
        visitsFromPicker.Location = new Point(10, 10);
        visitsFromPicker.Name = "visitsFromPicker";
        visitsFromPicker.Size = new Size(150, 27);
        visitsFromPicker.TabIndex = 0;

        //
        // visitsToPicker
        //
        visitsToPicker.Font = inputFont;
        visitsToPicker.Format = DateTimePickerFormat.Short;
        visitsToPicker.Location = new Point(170, 10);
        visitsToPicker.Name = "visitsToPicker";
        visitsToPicker.Size = new Size(150, 27);
        visitsToPicker.TabIndex = 1;

        //
        // visitsFilterButton
        //
        visitsFilterButton.AutoSize = true;
        visitsFilterButton.Font = buttonFont;
        visitsFilterButton.Location = new Point(330, 8);
        visitsFilterButton.MinimumSize = new Size(100, 32);
        visitsFilterButton.Name = "visitsFilterButton";
        visitsFilterButton.Padding = new Padding(8, 3, 8, 3);
        visitsFilterButton.Size = new Size(100, 32);
        visitsFilterButton.TabIndex = 2;
        visitsFilterButton.Text = Strings.Button_Filter;
        visitsFilterButton.UseVisualStyleBackColor = true;
        visitsFilterButton.Click += OnFilterVisitsClick;

        //
        // remoteSitesButton
        //
        remoteSitesButton.AutoSize = true;
        remoteSitesButton.Font = buttonFont;
        remoteSitesButton.Location = new Point(440, 8);
        remoteSitesButton.MinimumSize = new Size(120, 32);
        remoteSitesButton.Name = "remoteSitesButton";
        remoteSitesButton.Padding = new Padding(8, 3, 8, 3);
        remoteSitesButton.Size = new Size(120, 32);
        remoteSitesButton.TabIndex = 3;
        remoteSitesButton.Text = Strings.Button_RemoteSites;
        remoteSitesButton.UseVisualStyleBackColor = true;
        remoteSitesButton.Click += OnRemoteSitesClick;

        //
        // exportVisitsButton
        //
        exportVisitsButton.AutoSize = true;
        exportVisitsButton.Font = buttonFont;
        exportVisitsButton.Location = new Point(570, 8);
        exportVisitsButton.MinimumSize = new Size(100, 32);
        exportVisitsButton.Name = "exportVisitsButton";
        exportVisitsButton.Padding = new Padding(8, 3, 8, 3);
        exportVisitsButton.Size = new Size(100, 32);
        exportVisitsButton.TabIndex = 4;
        exportVisitsButton.Text = Strings.Button_Export;
        exportVisitsButton.UseVisualStyleBackColor = true;
        exportVisitsButton.Click += OnExportVisitsClick;

        //
        // refreshVisitsButton
        //
        refreshVisitsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        refreshVisitsButton.AutoSize = true;
        refreshVisitsButton.Font = buttonFont;
        refreshVisitsButton.Location = new Point(1080, 8);
        refreshVisitsButton.MinimumSize = new Size(100, 32);
        refreshVisitsButton.Name = "refreshVisitsButton";
        refreshVisitsButton.Padding = new Padding(8, 3, 8, 3);
        refreshVisitsButton.Size = new Size(100, 32);
        refreshVisitsButton.TabIndex = 5;
        refreshVisitsButton.Text = Strings.Button_Refresh;
        refreshVisitsButton.UseVisualStyleBackColor = true;
        refreshVisitsButton.Click += OnRefreshVisitsClick;

        //
        // visitsGrid
        //
        visitsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        visitsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        visitsGrid.Location = new Point(10, 50);
        visitsGrid.Name = "visitsGrid";
        visitsGrid.ReadOnly = true;
        visitsGrid.RowTemplate.Height = 25;
        visitsGrid.Size = new Size(1180, 660);
        visitsGrid.TabIndex = 6;
        visitsGrid.CellDoubleClick += visitsGrid_CellDoubleClick;

        //
        // settingsPanel - Settings view
        //
        settingsPanel.Controls.Add(settingsGrid);
        settingsPanel.Controls.Add(openCatalogsButton);
        settingsPanel.Controls.Add(backupButton);
        settingsPanel.Controls.Add(restoreButton);
        settingsPanel.Controls.Add(saveSettingsButton);
        settingsPanel.Dock = DockStyle.Fill;
        settingsPanel.Location = new Point(0, 78);
        settingsPanel.Name = "settingsPanel";
        settingsPanel.Padding = new Padding(10);
        settingsPanel.Size = new Size(1200, 722);
        settingsPanel.TabIndex = 6;
        settingsPanel.Visible = false;

        //
        // settingsGrid
        //
        settingsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        settingsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        settingsGrid.Location = new Point(10, 10);
        settingsGrid.Name = "settingsGrid";
        settingsGrid.ReadOnly = false;
        settingsGrid.RowTemplate.Height = 25;
        settingsGrid.Size = new Size(1180, 660);
        settingsGrid.TabIndex = 0;

        //
        // openCatalogsButton
        //
        openCatalogsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        openCatalogsButton.AutoSize = true;
        openCatalogsButton.Font = buttonFont;
        openCatalogsButton.Location = new Point(10, 680);
        openCatalogsButton.MinimumSize = new Size(120, 35);
        openCatalogsButton.Name = "openCatalogsButton";
        openCatalogsButton.Padding = new Padding(8, 3, 8, 3);
        openCatalogsButton.Size = new Size(120, 35);
        openCatalogsButton.TabIndex = 1;
        openCatalogsButton.Text = Strings.Button_Catalogs;
        openCatalogsButton.UseVisualStyleBackColor = true;
        openCatalogsButton.Click += OnOpenCatalogsClick;

        //
        // backupButton
        //
        backupButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        backupButton.AutoSize = true;
        backupButton.Font = buttonFont;
        backupButton.Location = new Point(140, 680);
        backupButton.MinimumSize = new Size(120, 35);
        backupButton.Name = "backupButton";
        backupButton.Padding = new Padding(8, 3, 8, 3);
        backupButton.Size = new Size(120, 35);
        backupButton.TabIndex = 2;
        backupButton.Text = Strings.Button_Backup;
        backupButton.UseVisualStyleBackColor = true;
        backupButton.Click += OnBackupClick;

        //
        // restoreButton
        //
        restoreButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        restoreButton.AutoSize = true;
        restoreButton.Font = buttonFont;
        restoreButton.Location = new Point(270, 680);
        restoreButton.MinimumSize = new Size(120, 35);
        restoreButton.Name = "restoreButton";
        restoreButton.Padding = new Padding(8, 3, 8, 3);
        restoreButton.Size = new Size(120, 35);
        restoreButton.TabIndex = 3;
        restoreButton.Text = Strings.Button_Restore;
        restoreButton.UseVisualStyleBackColor = true;
        restoreButton.Click += OnRestoreClick;

        //
        // saveSettingsButton
        //
        saveSettingsButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        saveSettingsButton.AutoSize = true;
        saveSettingsButton.Font = buttonFont;
        saveSettingsButton.ForeColor = Color.Blue;
        saveSettingsButton.Location = new Point(1070, 680);
        saveSettingsButton.MinimumSize = new Size(120, 35);
        saveSettingsButton.Name = "saveSettingsButton";
        saveSettingsButton.Padding = new Padding(8, 3, 8, 3);
        saveSettingsButton.Size = new Size(120, 35);
        saveSettingsButton.TabIndex = 6;
        saveSettingsButton.Text = Strings.Button_Save;
        saveSettingsButton.UseVisualStyleBackColor = true;
        saveSettingsButton.Click += OnSaveSettingsClick;

        //
        // auditPanel - Audit view
        //
        auditPanel.Controls.Add(auditFromPicker);
        auditPanel.Controls.Add(auditToPicker);
        auditPanel.Controls.Add(auditActionText);
        auditPanel.Controls.Add(auditTargetText);
        auditPanel.Controls.Add(auditRefreshButton);
        auditPanel.Controls.Add(auditExportButton);
        auditPanel.Controls.Add(auditGrid);
        auditPanel.Dock = DockStyle.Fill;
        auditPanel.Location = new Point(0, 78);
        auditPanel.Name = "auditPanel";
        auditPanel.Padding = new Padding(10);
        auditPanel.Size = new Size(1200, 722);
        auditPanel.TabIndex = 7;
        auditPanel.Visible = false;

        //
        // auditFromPicker
        //
        auditFromPicker.Font = inputFont;
        auditFromPicker.Format = DateTimePickerFormat.Short;
        auditFromPicker.Location = new Point(10, 10);
        auditFromPicker.Name = "auditFromPicker";
        auditFromPicker.Size = new Size(150, 27);
        auditFromPicker.TabIndex = 0;

        //
        // auditToPicker
        //
        auditToPicker.Font = inputFont;
        auditToPicker.Format = DateTimePickerFormat.Short;
        auditToPicker.Location = new Point(170, 10);
        auditToPicker.Name = "auditToPicker";
        auditToPicker.Size = new Size(150, 27);
        auditToPicker.TabIndex = 1;

        //
        // auditActionText
        //
        auditActionText.Font = inputFont;
        auditActionText.Location = new Point(340, 10);
        auditActionText.Name = "auditActionText";
        auditActionText.PlaceholderText = Strings.Audit_ActionType;
        auditActionText.Size = new Size(150, 27);
        auditActionText.TabIndex = 2;

        //
        // auditTargetText
        //
        auditTargetText.Font = inputFont;
        auditTargetText.Location = new Point(500, 10);
        auditTargetText.Name = "auditTargetText";
        auditTargetText.PlaceholderText = Strings.Audit_TargetType;
        auditTargetText.Size = new Size(150, 27);
        auditTargetText.TabIndex = 3;

        //
        // auditRefreshButton
        //
        auditRefreshButton.AutoSize = true;
        auditRefreshButton.Font = buttonFont;
        auditRefreshButton.Location = new Point(670, 8);
        auditRefreshButton.MinimumSize = new Size(100, 32);
        auditRefreshButton.Name = "auditRefreshButton";
        auditRefreshButton.Padding = new Padding(8, 3, 8, 3);
        auditRefreshButton.Size = new Size(100, 32);
        auditRefreshButton.TabIndex = 4;
        auditRefreshButton.Text = Strings.Button_Refresh;
        auditRefreshButton.UseVisualStyleBackColor = true;
        auditRefreshButton.Click += OnRefreshAuditClick;

        //
        // auditExportButton
        //
        auditExportButton.AutoSize = true;
        auditExportButton.Font = buttonFont;
        auditExportButton.Location = new Point(780, 8);
        auditExportButton.MinimumSize = new Size(100, 32);
        auditExportButton.Name = "auditExportButton";
        auditExportButton.Padding = new Padding(8, 3, 8, 3);
        auditExportButton.Size = new Size(100, 32);
        auditExportButton.TabIndex = 5;
        auditExportButton.Text = Strings.Button_Export;
        auditExportButton.UseVisualStyleBackColor = true;
        auditExportButton.Click += OnExportAuditClick;

        //
        // auditGrid
        //
        auditGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        auditGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        auditGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        auditGrid.Location = new Point(10, 50);
        auditGrid.Name = "auditGrid";
        auditGrid.ReadOnly = true;
        auditGrid.RowTemplate.Height = 25;
        auditGrid.Size = new Size(1180, 660);
        auditGrid.TabIndex = 6;

        //
        // MainForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1200, 800);
        Controls.Add(gatePanel);
        Controls.Add(membersPanel);
        Controls.Add(usersPanel);
        Controls.Add(visitsPanel);
        Controls.Add(settingsPanel);
        Controls.Add(auditPanel);
        Controls.Add(headerPanel);
        Controls.Add(mainMenuStrip);
        MainMenuStrip = mainMenuStrip;
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        KeyPreview = true;
        KeyDown += OnMainFormKeyDown;
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Text = "Pylae";
        mainMenuStrip.ResumeLayout(false);
        mainMenuStrip.PerformLayout();
        headerPanel.ResumeLayout(false);
        headerPanel.PerformLayout();
        gatePanel.ResumeLayout(false);
        gatePanel.PerformLayout();
        directionGroupBox.ResumeLayout(false);
        directionGroupBox.PerformLayout();
        membersPanel.ResumeLayout(false);
        membersPanel.PerformLayout();
        usersPanel.ResumeLayout(false);
        usersPanel.PerformLayout();
        visitsPanel.ResumeLayout(false);
        visitsPanel.PerformLayout();
        settingsPanel.ResumeLayout(false);
        settingsPanel.PerformLayout();
        auditPanel.ResumeLayout(false);
        auditPanel.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)settingsGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)membersGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)usersGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)visitsGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)auditGrid).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    // Menu
    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem fileMainMenuItem;
    private ToolStripMenuItem fileVisitsMenuItem;
    private ToolStripSeparator fileSeparator;
    private ToolStripMenuItem fileExitMenuItem;
    private ToolStripMenuItem adminMenu;
    private ToolStripMenuItem adminBadgesMenuItem;
    private ToolStripMenuItem adminUsersMenuItem;
    private ToolStripMenuItem adminSettingsMenuItem;
    private ToolStripMenuItem adminAuditMenuItem;
    private ToolStripMenuItem helpMenu;
    private ToolStripMenuItem helpUserGuideMenuItem;
    private ToolStripMenuItem helpAdminGuideMenuItem;
    private ToolStripSeparator helpSeparator;
    private ToolStripMenuItem helpAboutMenuItem;

    // Header
    private Panel headerPanel;
    private Label subtitleLabel;
    private Label welcomeLabel;
    private Label siteLabel;

    // Content panels
    private Panel gatePanel;
    private Panel membersPanel;
    private Panel usersPanel;
    private Panel visitsPanel;
    private Panel settingsPanel;
    private Panel auditPanel;

    // Gate panel controls
    private GroupBox directionGroupBox;
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

    // Members panel controls
    private DataGridView membersGrid;
    private Button addMemberButton;
    private Button refreshMembersButton;
    private Button exportMembersButton;
    private Button badgeButton;
    private Button printBadgeButton;

    // Users panel controls
    private DataGridView usersGrid;
    private Button refreshUsersButton;
    private Button addUserButton;
    private Button editUserButton;
    private Button deactivateUserButton;
    private Button deleteUserButton;
    private Button resetPasswordButton;
    private Button resetQuickCodeButton;

    // Visits panel controls
    private DataGridView visitsGrid;
    private Button refreshVisitsButton;
    private DateTimePicker visitsFromPicker;
    private DateTimePicker visitsToPicker;
    private Button visitsFilterButton;
    private Button remoteSitesButton;
    private Button exportVisitsButton;

    // Settings panel controls
    private DataGridView settingsGrid;
    private Button saveSettingsButton;
    private Button openCatalogsButton;
    private Button backupButton;
    private Button restoreButton;

    // Audit panel controls
    private DataGridView auditGrid;
    private DateTimePicker auditFromPicker;
    private DateTimePicker auditToPicker;
    private TextBox auditActionText;
    private TextBox auditTargetText;
    private Button auditRefreshButton;
    private Button auditExportButton;
}
