using System.Drawing;
using System.IO;
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
        fileLogoutMenuItem = new ToolStripMenuItem();
        fileSeparator = new ToolStripSeparator();
        fileExitMenuItem = new ToolStripMenuItem();
        adminMenu = new ToolStripMenuItem();
        adminBadgesMenuItem = new ToolStripMenuItem();
        adminUsersMenuItem = new ToolStripMenuItem();
        adminSettingsMenuItem = new ToolStripMenuItem();
        adminAuditMenuItem = new ToolStripMenuItem();
        adminRemoteSitesMenuItem = new ToolStripMenuItem();
        helpMenu = new ToolStripMenuItem();
        helpUserGuideMenuItem = new ToolStripMenuItem();
        helpAdminGuideMenuItem = new ToolStripMenuItem();
        helpSeparator = new ToolStripSeparator();
        helpAboutMenuItem = new ToolStripMenuItem();

        // Status bar
        statusStrip = new StatusStrip();
        statusLabel = new ToolStripStatusLabel();

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
        // Gate panel - member result display
        gateLayoutPanel = new TableLayoutPanel();
        gateLeftPanel = new Panel();
        gateRightPanel = new Panel();
        lastMemberPhotoBox = new PictureBox();
        lastMemberRankLabel = new Label();
        lastMemberNameLabel = new Label();
        lastMemberStatusLabel = new Label();
        lastMemberActiveLabel = new Label();
        lastMemberTypeLabel = new Label();
        lastMemberPersonalIdLabel = new Label();
        lastMemberOrgIdLabel = new Label();
        lastMemberIssueDateLabel = new Label();
        lastMemberExpiryDateLabel = new Label();
        lastLogTimeLabel = new Label();
        lastLogDirectionLabel = new Label();
        recentLogsGrid = new DataGridView();

        // Members panel - grid only (configured in MasterDetail)
        membersGrid = new DataGridView();

        // Users panel - grid only (configured in MasterDetail)
        usersGrid = new DataGridView();

        // Visits panel controls
        visitsGrid = new DataGridView();
        refreshVisitsButton = new Button();
        visitsFromPicker = new DateTimePicker();
        visitsToPicker = new DateTimePicker();
        visitsFilterButton = new Button();
        visitsEntryFilterButton = new Button();
        visitsExitFilterButton = new Button();
        visitsAllFilterButton = new Button();
        // remoteSitesButton removed - now in Administrator menu
        exportVisitsButton = new Button();
        visitsSearchTextBox = new TextBox();

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
        fileMenu.DropDownItems.AddRange(new ToolStripItem[] { fileMainMenuItem, fileVisitsMenuItem, fileSeparator, fileLogoutMenuItem, fileExitMenuItem });
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
        // fileLogoutMenuItem
        //
        fileLogoutMenuItem.Name = "fileLogoutMenuItem";
        fileLogoutMenuItem.Text = Strings.Menu_Logout;
        fileLogoutMenuItem.Click += OnMenuLogoutClick;

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
        adminMenu.DropDownItems.AddRange(new ToolStripItem[] { adminBadgesMenuItem, adminUsersMenuItem, adminSettingsMenuItem, adminAuditMenuItem, adminRemoteSitesMenuItem });
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
        // adminRemoteSitesMenuItem
        //
        adminRemoteSitesMenuItem.Name = "adminRemoteSitesMenuItem";
        adminRemoteSitesMenuItem.ShortcutKeys = Keys.F7;
        adminRemoteSitesMenuItem.Text = Strings.Menu_RemoteSites;
        adminRemoteSitesMenuItem.Click += OnMenuRemoteSitesClick;

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
        headerPanel.Padding = new Padding(12, 4, 16, 4);
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
        // welcomeLabel - positioned at right with padding
        //
        welcomeLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        welcomeLabel.AutoSize = true;
        welcomeLabel.Font = labelFont;
        welcomeLabel.Location = new Point(900, 4);
        welcomeLabel.Margin = new Padding(3, 0, 10, 0);
        welcomeLabel.Name = "welcomeLabel";
        welcomeLabel.Size = new Size(57, 20);
        welcomeLabel.TabIndex = 1;
        welcomeLabel.Text = "";
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
        // gatePanel - Main check-in/check-out view (two-column layout)
        //
        gatePanel.Controls.Add(gateLayoutPanel);
        gatePanel.Dock = DockStyle.Fill;
        gatePanel.Location = new Point(0, 78);
        gatePanel.Name = "gatePanel";
        gatePanel.Padding = new Padding(10);
        gatePanel.Size = new Size(1200, 722);
        gatePanel.TabIndex = 2;
        gatePanel.Visible = true;

        //
        // gateLayoutPanel - two columns: left (inputs), right (result)
        //
        gateLayoutPanel.ColumnCount = 2;
        gateLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        gateLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        gateLayoutPanel.RowCount = 1;
        gateLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        gateLayoutPanel.Dock = DockStyle.Fill;
        gateLayoutPanel.Name = "gateLayoutPanel";
        gateLayoutPanel.Controls.Add(gateLeftPanel, 0, 0);
        gateLayoutPanel.Controls.Add(gateRightPanel, 1, 0);

        //
        // gateLeftPanel - input controls
        //
        gateLeftPanel.Controls.Add(directionGroupBox);
        gateLeftPanel.Controls.Add(memberNumberLabel);
        gateLeftPanel.Controls.Add(memberNumberTextBox);
        gateLeftPanel.Controls.Add(notesLabel);
        gateLeftPanel.Controls.Add(notesTextBox);
        gateLeftPanel.Controls.Add(logVisitButton);
        gateLeftPanel.Dock = DockStyle.Fill;
        gateLeftPanel.Name = "gateLeftPanel";
        gateLeftPanel.Padding = new Padding(10);

        //
        // gateRightPanel - last result display with member details
        //
        gateRightPanel.Controls.Add(gateResultLabel);
        gateRightPanel.Controls.Add(lastResultValueLabel);
        gateRightPanel.Controls.Add(lastLogDirectionLabel);
        gateRightPanel.Controls.Add(lastLogTimeLabel);
        gateRightPanel.Controls.Add(lastMemberPhotoBox);
        gateRightPanel.Controls.Add(lastMemberRankLabel);
        gateRightPanel.Controls.Add(lastMemberNameLabel);
        gateRightPanel.Controls.Add(lastMemberStatusLabel);
        gateRightPanel.Controls.Add(lastMemberActiveLabel);
        gateRightPanel.Controls.Add(lastMemberTypeLabel);
        gateRightPanel.Controls.Add(lastMemberPersonalIdLabel);
        gateRightPanel.Controls.Add(lastMemberOrgIdLabel);
        gateRightPanel.Controls.Add(lastMemberIssueDateLabel);
        gateRightPanel.Controls.Add(lastMemberExpiryDateLabel);
        gateRightPanel.Controls.Add(badgeWarningLabel);
        gateRightPanel.Controls.Add(recentLogsGrid);
        gateRightPanel.Dock = DockStyle.Fill;
        gateRightPanel.Name = "gateRightPanel";
        gateRightPanel.Padding = new Padding(10);
        gateRightPanel.BorderStyle = BorderStyle.FixedSingle;

        //
        // directionGroupBox
        //
        directionGroupBox.Controls.Add(entryRadio);
        directionGroupBox.Controls.Add(exitRadio);
        directionGroupBox.Font = labelFont;
        directionGroupBox.Location = new Point(10, 10);
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
        memberNumberLabel.Location = new Point(10, 100);
        memberNumberLabel.Name = "memberNumberLabel";
        memberNumberLabel.Size = new Size(120, 20);
        memberNumberLabel.TabIndex = 1;
        memberNumberLabel.Text = Strings.Gate_MemberNumber;

        //
        // memberNumberTextBox
        //
        memberNumberTextBox.Font = inputFont;
        memberNumberTextBox.Location = new Point(10, 125);
        memberNumberTextBox.Name = "memberNumberTextBox";
        memberNumberTextBox.Size = new Size(300, 27);
        memberNumberTextBox.TabIndex = 2;
        memberNumberTextBox.KeyDown += OnMemberNumberKeyDown;

        //
        // notesLabel
        //
        notesLabel.AutoSize = true;
        notesLabel.Font = labelFont;
        notesLabel.Location = new Point(10, 170);
        notesLabel.Name = "notesLabel";
        notesLabel.Size = new Size(50, 20);
        notesLabel.TabIndex = 3;
        notesLabel.Text = Strings.Gate_Notes;

        //
        // notesTextBox
        //
        notesTextBox.Font = inputFont;
        notesTextBox.Location = new Point(10, 195);
        notesTextBox.Multiline = true;
        notesTextBox.Name = "notesTextBox";
        notesTextBox.Size = new Size(350, 80);
        notesTextBox.TabIndex = 4;

        //
        // logVisitButton
        //
        logVisitButton.AutoSize = true;
        logVisitButton.Font = buttonFont;
        logVisitButton.Location = new Point(10, 295);
        logVisitButton.MinimumSize = new Size(150, 40);
        logVisitButton.Name = "logVisitButton";
        logVisitButton.Padding = new Padding(10, 5, 10, 5);
        logVisitButton.Size = new Size(150, 40);
        logVisitButton.TabIndex = 5;
        logVisitButton.Text = Strings.Gate_LogVisit;
        logVisitButton.UseVisualStyleBackColor = true;
        logVisitButton.Click += OnLogVisitClick;

        //
        // gateResultLabel - "Last Result" heading
        //
        gateResultLabel.AutoSize = true;
        gateResultLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        gateResultLabel.Location = new Point(10, 10);
        gateResultLabel.Name = "gateResultLabel";
        gateResultLabel.Size = new Size(120, 25);
        gateResultLabel.TabIndex = 0;
        gateResultLabel.Text = Strings.Gate_LastResult;

        //
        // lastResultValueLabel - result text (entry/exit success)
        //
        lastResultValueLabel.AutoSize = true;
        lastResultValueLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        lastResultValueLabel.Location = new Point(10, 40);
        lastResultValueLabel.Name = "lastResultValueLabel";
        lastResultValueLabel.MaximumSize = new Size(600, 0);
        lastResultValueLabel.Size = new Size(0, 21);
        lastResultValueLabel.TabIndex = 1;

        //
        // lastLogDirectionLabel - ENTRY or EXIT
        //
        lastLogDirectionLabel.AutoSize = true;
        lastLogDirectionLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
        lastLogDirectionLabel.Location = new Point(10, 70);
        lastLogDirectionLabel.Name = "lastLogDirectionLabel";
        lastLogDirectionLabel.Size = new Size(0, 30);
        lastLogDirectionLabel.TabIndex = 2;
        lastLogDirectionLabel.ForeColor = Color.DarkBlue;

        //
        // lastLogTimeLabel - logged time
        //
        lastLogTimeLabel.AutoSize = true;
        lastLogTimeLabel.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
        lastLogTimeLabel.Location = new Point(150, 75);
        lastLogTimeLabel.Name = "lastLogTimeLabel";
        lastLogTimeLabel.Size = new Size(0, 20);
        lastLogTimeLabel.TabIndex = 3;

        //
        // lastMemberPhotoBox - member photo (larger size)
        //
        lastMemberPhotoBox.BorderStyle = BorderStyle.FixedSingle;
        lastMemberPhotoBox.Location = new Point(10, 110);
        lastMemberPhotoBox.Name = "lastMemberPhotoBox";
        lastMemberPhotoBox.Size = new Size(240, 300);
        lastMemberPhotoBox.SizeMode = PictureBoxSizeMode.Zoom;
        lastMemberPhotoBox.TabIndex = 4;
        lastMemberPhotoBox.TabStop = false;
        lastMemberPhotoBox.BackColor = SystemColors.Control;

        //
        // lastMemberRankLabel - Rank
        //
        lastMemberRankLabel.AutoSize = true;
        lastMemberRankLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberRankLabel.Location = new Point(265, 110);
        lastMemberRankLabel.Name = "lastMemberRankLabel";
        lastMemberRankLabel.MaximumSize = new Size(450, 0);
        lastMemberRankLabel.Size = new Size(0, 21);
        lastMemberRankLabel.TabIndex = 5;

        //
        // lastMemberNameLabel - First Last name
        //
        lastMemberNameLabel.AutoSize = true;
        lastMemberNameLabel.Font = new Font("Segoe UI", 16F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberNameLabel.Location = new Point(265, 140);
        lastMemberNameLabel.Name = "lastMemberNameLabel";
        lastMemberNameLabel.MaximumSize = new Size(450, 0);
        lastMemberNameLabel.Size = new Size(0, 30);
        lastMemberNameLabel.TabIndex = 6;

        //
        // lastMemberStatusLabel - Status: PERMANENT or TEMPORARY
        //
        lastMemberStatusLabel.AutoSize = true;
        lastMemberStatusLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberStatusLabel.Location = new Point(265, 180);
        lastMemberStatusLabel.Name = "lastMemberStatusLabel";
        lastMemberStatusLabel.MaximumSize = new Size(450, 0);
        lastMemberStatusLabel.Size = new Size(0, 20);
        lastMemberStatusLabel.TabIndex = 7;

        //
        // lastMemberActiveLabel - Badge Status: Active/Inactive
        //
        lastMemberActiveLabel.AutoSize = true;
        lastMemberActiveLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberActiveLabel.Location = new Point(265, 210);
        lastMemberActiveLabel.Name = "lastMemberActiveLabel";
        lastMemberActiveLabel.MaximumSize = new Size(450, 0);
        lastMemberActiveLabel.Size = new Size(0, 20);
        lastMemberActiveLabel.TabIndex = 8;

        //
        // lastMemberTypeLabel - Badge Type: ...
        //
        lastMemberTypeLabel.AutoSize = true;
        lastMemberTypeLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberTypeLabel.Location = new Point(265, 240);
        lastMemberTypeLabel.Name = "lastMemberTypeLabel";
        lastMemberTypeLabel.MaximumSize = new Size(450, 0);
        lastMemberTypeLabel.Size = new Size(0, 20);
        lastMemberTypeLabel.TabIndex = 9;

        //
        // lastMemberPersonalIdLabel - Identification Number
        //
        lastMemberPersonalIdLabel.AutoSize = true;
        lastMemberPersonalIdLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberPersonalIdLabel.Location = new Point(265, 270);
        lastMemberPersonalIdLabel.Name = "lastMemberPersonalIdLabel";
        lastMemberPersonalIdLabel.MaximumSize = new Size(450, 0);
        lastMemberPersonalIdLabel.Size = new Size(0, 20);
        lastMemberPersonalIdLabel.TabIndex = 10;

        //
        // lastMemberOrgIdLabel - Organization Identification
        //
        lastMemberOrgIdLabel.AutoSize = true;
        lastMemberOrgIdLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberOrgIdLabel.Location = new Point(265, 300);
        lastMemberOrgIdLabel.Name = "lastMemberOrgIdLabel";
        lastMemberOrgIdLabel.MaximumSize = new Size(450, 0);
        lastMemberOrgIdLabel.Size = new Size(0, 20);
        lastMemberOrgIdLabel.TabIndex = 11;

        //
        // lastMemberIssueDateLabel - Issue Date
        //
        lastMemberIssueDateLabel.AutoSize = true;
        lastMemberIssueDateLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberIssueDateLabel.Location = new Point(265, 330);
        lastMemberIssueDateLabel.Name = "lastMemberIssueDateLabel";
        lastMemberIssueDateLabel.MaximumSize = new Size(450, 0);
        lastMemberIssueDateLabel.Size = new Size(0, 20);
        lastMemberIssueDateLabel.TabIndex = 12;

        //
        // lastMemberExpiryDateLabel - Expiry Date
        //
        lastMemberExpiryDateLabel.AutoSize = true;
        lastMemberExpiryDateLabel.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point);
        lastMemberExpiryDateLabel.Location = new Point(265, 360);
        lastMemberExpiryDateLabel.Name = "lastMemberExpiryDateLabel";
        lastMemberExpiryDateLabel.MaximumSize = new Size(450, 0);
        lastMemberExpiryDateLabel.Size = new Size(0, 20);
        lastMemberExpiryDateLabel.TabIndex = 13;

        //
        // badgeWarningLabel
        //
        badgeWarningLabel.AutoSize = true;
        badgeWarningLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
        badgeWarningLabel.ForeColor = Color.DarkGoldenrod;
        badgeWarningLabel.Location = new Point(265, 390);
        badgeWarningLabel.Name = "badgeWarningLabel";
        badgeWarningLabel.MaximumSize = new Size(450, 0);
        badgeWarningLabel.Size = new Size(0, 21);
        badgeWarningLabel.TabIndex = 14;

        //
        // recentLogsGrid - last 6 logs
        //
        recentLogsGrid.AllowUserToAddRows = false;
        recentLogsGrid.AllowUserToDeleteRows = false;
        recentLogsGrid.AllowUserToResizeRows = false;
        recentLogsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        recentLogsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        recentLogsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        recentLogsGrid.Location = new Point(10, 420);
        recentLogsGrid.Name = "recentLogsGrid";
        recentLogsGrid.ReadOnly = true;
        recentLogsGrid.RowHeadersVisible = false;
        recentLogsGrid.RowTemplate.Height = 25;
        recentLogsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        recentLogsGrid.Size = new Size(700, 180);
        recentLogsGrid.TabIndex = 15;
        recentLogsGrid.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
        recentLogsGrid.ScrollBars = ScrollBars.Both;

        //
        // membersPanel - Badges/Members view
        //
        membersPanel.Dock = DockStyle.Fill;
        membersPanel.Location = new Point(0, 78);
        membersPanel.Name = "membersPanel";
        membersPanel.Padding = new Padding(10);
        membersPanel.Size = new Size(1200, 722);
        membersPanel.TabIndex = 3;
        membersPanel.Visible = false;

        //
        // usersPanel - Users management view
        //
        usersPanel.Dock = DockStyle.Fill;
        usersPanel.Location = new Point(0, 78);
        usersPanel.Name = "usersPanel";
        usersPanel.Padding = new Padding(10);
        usersPanel.Size = new Size(1200, 722);
        usersPanel.TabIndex = 4;
        usersPanel.Visible = false;

        //
        // visitsPanel - Visits view
        //
        visitsPanel.Controls.Add(visitsFromPicker);
        visitsPanel.Controls.Add(visitsToPicker);
        visitsPanel.Controls.Add(visitsFilterButton);
        visitsPanel.Controls.Add(visitsEntryFilterButton);
        visitsPanel.Controls.Add(visitsExitFilterButton);
        visitsPanel.Controls.Add(visitsAllFilterButton);
        visitsPanel.Controls.Add(visitsSearchTextBox);
        // remoteSitesButton removed - now in Administrator menu
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
        visitsFilterButton.MinimumSize = new Size(80, 32);
        visitsFilterButton.Name = "visitsFilterButton";
        visitsFilterButton.Padding = new Padding(6, 3, 6, 3);
        visitsFilterButton.Size = new Size(80, 32);
        visitsFilterButton.TabIndex = 2;
        visitsFilterButton.Text = Strings.Button_Filter;
        visitsFilterButton.UseVisualStyleBackColor = true;
        visitsFilterButton.Click += OnFilterVisitsClick;

        //
        // visitsEntryFilterButton
        //
        visitsEntryFilterButton.AutoSize = true;
        visitsEntryFilterButton.Font = buttonFont;
        visitsEntryFilterButton.Location = new Point(420, 8);
        visitsEntryFilterButton.MinimumSize = new Size(70, 32);
        visitsEntryFilterButton.Name = "visitsEntryFilterButton";
        visitsEntryFilterButton.Padding = new Padding(6, 3, 6, 3);
        visitsEntryFilterButton.Size = new Size(70, 32);
        visitsEntryFilterButton.TabIndex = 3;
        visitsEntryFilterButton.Text = Strings.Gate_Entry;
        visitsEntryFilterButton.UseVisualStyleBackColor = true;
        visitsEntryFilterButton.Click += OnFilterVisitsEntryClick;

        //
        // visitsExitFilterButton
        //
        visitsExitFilterButton.AutoSize = true;
        visitsExitFilterButton.Font = buttonFont;
        visitsExitFilterButton.Location = new Point(500, 8);
        visitsExitFilterButton.MinimumSize = new Size(70, 32);
        visitsExitFilterButton.Name = "visitsExitFilterButton";
        visitsExitFilterButton.Padding = new Padding(6, 3, 6, 3);
        visitsExitFilterButton.Size = new Size(70, 32);
        visitsExitFilterButton.TabIndex = 4;
        visitsExitFilterButton.Text = Strings.Gate_Exit;
        visitsExitFilterButton.UseVisualStyleBackColor = true;
        visitsExitFilterButton.Click += OnFilterVisitsExitClick;

        //
        // visitsAllFilterButton
        //
        visitsAllFilterButton.AutoSize = true;
        visitsAllFilterButton.Font = buttonFont;
        visitsAllFilterButton.Location = new Point(580, 8);
        visitsAllFilterButton.MinimumSize = new Size(60, 32);
        visitsAllFilterButton.Name = "visitsAllFilterButton";
        visitsAllFilterButton.Padding = new Padding(6, 3, 6, 3);
        visitsAllFilterButton.Size = new Size(60, 32);
        visitsAllFilterButton.TabIndex = 5;
        visitsAllFilterButton.Text = Strings.Button_All;
        visitsAllFilterButton.UseVisualStyleBackColor = true;
        visitsAllFilterButton.Click += OnFilterVisitsAllClick;

        //
        // remoteSitesButton removed - now accessible from Administrator menu

        //
        // visitsSearchTextBox
        //
        visitsSearchTextBox.Font = inputFont;
        visitsSearchTextBox.Location = new Point(650, 10);
        visitsSearchTextBox.Name = "visitsSearchTextBox";
        visitsSearchTextBox.PlaceholderText = Strings.Visits_SearchPlaceholder;
        visitsSearchTextBox.Size = new Size(250, 27);
        visitsSearchTextBox.TabIndex = 6;
        visitsSearchTextBox.TextChanged += OnVisitsSearchTextChanged;

        //
        // exportVisitsButton
        //
        exportVisitsButton.AutoSize = true;
        exportVisitsButton.Font = buttonFont;
        exportVisitsButton.Location = new Point(910, 8);
        exportVisitsButton.MinimumSize = new Size(100, 32);
        exportVisitsButton.Name = "exportVisitsButton";
        exportVisitsButton.Padding = new Padding(6, 3, 6, 3);
        exportVisitsButton.Size = new Size(100, 32);
        exportVisitsButton.TabIndex = 7;
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
        refreshVisitsButton.TabIndex = 8;
        refreshVisitsButton.Text = Strings.Button_Refresh;
        refreshVisitsButton.UseVisualStyleBackColor = true;
        refreshVisitsButton.Click += OnRefreshVisitsClick;

        //
        // visitsGrid
        //
        visitsGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        visitsGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        visitsGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        visitsGrid.Location = new Point(10, 50);
        visitsGrid.Name = "visitsGrid";
        visitsGrid.ReadOnly = true;
        visitsGrid.RowTemplate.Height = 25;
        visitsGrid.Size = new Size(1180, 660);
        visitsGrid.TabIndex = 9;
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
        settingsPanel.AutoScroll = true;

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
        // statusStrip
        //
        statusStrip.Items.Add(statusLabel);
        statusStrip.Dock = DockStyle.Bottom;
        statusStrip.Name = "statusStrip";
        statusStrip.Size = new Size(1200, 22);
        statusStrip.TabIndex = 100;
        //
        // statusLabel
        //
        statusLabel.Name = "statusLabel";
        statusLabel.Text = Strings.Status_Ready;
        statusLabel.Spring = true;
        statusLabel.TextAlign = ContentAlignment.MiddleLeft;
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
        Controls.Add(statusStrip);
        Controls.Add(mainMenuStrip);
        MainMenuStrip = mainMenuStrip;
        MinimumSize = new Size(900, 600);
        Name = "MainForm";
        Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "pylae_icon.ico"));
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

    // Status bar
    private StatusStrip statusStrip;
    private ToolStripStatusLabel statusLabel;

    // Menu
    private MenuStrip mainMenuStrip;
    private ToolStripMenuItem fileMenu;
    private ToolStripMenuItem fileMainMenuItem;
    private ToolStripMenuItem fileVisitsMenuItem;
    private ToolStripSeparator fileSeparator;
    private ToolStripMenuItem fileLogoutMenuItem;
    private ToolStripMenuItem fileExitMenuItem;
    private ToolStripMenuItem adminMenu;
    private ToolStripMenuItem adminBadgesMenuItem;
    private ToolStripMenuItem adminUsersMenuItem;
    private ToolStripMenuItem adminSettingsMenuItem;
    private ToolStripMenuItem adminAuditMenuItem;
    private ToolStripMenuItem adminRemoteSitesMenuItem;
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
    // Gate panel - member result display
    private TableLayoutPanel gateLayoutPanel;
    private Panel gateLeftPanel;
    private Panel gateRightPanel;
    private PictureBox lastMemberPhotoBox;
    private Label lastMemberRankLabel;
    private Label lastMemberNameLabel;
    private Label lastMemberStatusLabel;
    private Label lastMemberActiveLabel;
    private Label lastMemberTypeLabel;
    private Label lastMemberPersonalIdLabel;
    private Label lastMemberOrgIdLabel;
    private Label lastMemberIssueDateLabel;
    private Label lastMemberExpiryDateLabel;
    private Label lastLogTimeLabel;
    private Label lastLogDirectionLabel;
    private DataGridView recentLogsGrid;

    // Members panel - grid only (configured in MasterDetail)
    private DataGridView membersGrid;

    // Users panel - grid only (configured in MasterDetail)
    private DataGridView usersGrid;

    // Visits panel controls
    private DataGridView visitsGrid;
    private Button refreshVisitsButton;
    private DateTimePicker visitsFromPicker;
    private DateTimePicker visitsToPicker;
    private Button visitsFilterButton;
    private Button visitsEntryFilterButton;
    private Button visitsExitFilterButton;
    private Button visitsAllFilterButton;
    private TextBox visitsSearchTextBox;
    // remoteSitesButton removed - now in Administrator menu
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
