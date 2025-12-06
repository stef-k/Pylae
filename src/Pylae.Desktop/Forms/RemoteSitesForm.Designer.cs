using System.Drawing;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class RemoteSitesForm
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
        hostLabel = new Label();
        hostTextBox = new TextBox();
        portLabel = new Label();
        portNumeric = new NumericUpDown();
        apiKeyLabel = new Label();
        apiKeyTextBox = new TextBox();
        infoButton = new Button();
        infoTextBox = new TextBox();
        fetchVisitsButton = new Button();
        fromPicker = new DateTimePicker();
        toPicker = new DateTimePicker();
        fromLabel = new Label();
        toLabel = new Label();
        visitsCountLabel = new Label();
        pushMasterButton = new Button();
        downloadVisitsButton = new Button();
        includePhotosCheckBox = new CheckBox();
        pullMasterButton = new Button();
        statusLabel = new Label();
        statusTextBox = new TextBox();
        recentEventsLabel = new Label();
        recentEventsList = new ListBox();
        ((System.ComponentModel.ISupportInitialize)portNumeric).BeginInit();
        SuspendLayout();
        // 
        // hostLabel
        // 
        hostLabel.AutoSize = true;
        hostLabel.Location = new Point(12, 15);
        hostLabel.Name = "hostLabel";
        hostLabel.Size = new Size(32, 15);
        hostLabel.TabIndex = 0;
        hostLabel.Text = "Host";
        //
        // hostTextBox
        //
        hostTextBox.Location = new Point(12, 33);
        hostTextBox.Name = "hostTextBox";
        hostTextBox.Size = new Size(250, 23);
        hostTextBox.TabIndex = 1;
        hostTextBox.Text = "localhost";
        //
        // portLabel
        //
        portLabel.AutoSize = true;
        portLabel.Location = new Point(280, 15);
        portLabel.Name = "portLabel";
        portLabel.Size = new Size(29, 15);
        portLabel.TabIndex = 2;
        portLabel.Text = "Port";
        //
        // portNumeric
        //
        portNumeric.Location = new Point(280, 33);
        portNumeric.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        portNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        portNumeric.Name = "portNumeric";
        portNumeric.Size = new Size(80, 23);
        portNumeric.TabIndex = 3;
        portNumeric.Value = new decimal(new int[] { 8080, 0, 0, 0 });
        //
        // apiKeyLabel
        //
        apiKeyLabel.AutoSize = true;
        apiKeyLabel.Location = new Point(380, 15);
        apiKeyLabel.Name = "apiKeyLabel";
        apiKeyLabel.Size = new Size(47, 15);
        apiKeyLabel.TabIndex = 4;
        apiKeyLabel.Text = "API Key";
        //
        // apiKeyTextBox
        //
        apiKeyTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        apiKeyTextBox.Location = new Point(380, 33);
        apiKeyTextBox.Name = "apiKeyTextBox";
        apiKeyTextBox.Size = new Size(700, 23);
        apiKeyTextBox.TabIndex = 5;
        //
        // infoButton
        //
        infoButton.Location = new Point(12, 70);
        infoButton.Name = "infoButton";
        infoButton.Size = new Size(130, 25);
        infoButton.TabIndex = 6;
        infoButton.Text = "Fetch Info";
        infoButton.UseVisualStyleBackColor = true;
        infoButton.Click += OnFetchInfoClick;
        //
        // infoTextBox
        //
        infoTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        infoTextBox.Location = new Point(12, 99);
        infoTextBox.Multiline = true;
        infoTextBox.Name = "infoTextBox";
        infoTextBox.ReadOnly = true;
        infoTextBox.Size = new Size(1068, 80);
        infoTextBox.TabIndex = 7;
        //
        // fetchVisitsButton
        //
        fetchVisitsButton.Location = new Point(300, 197);
        fetchVisitsButton.Name = "fetchVisitsButton";
        fetchVisitsButton.Size = new Size(140, 25);
        fetchVisitsButton.TabIndex = 8;
        fetchVisitsButton.Text = "Fetch Visits";
        fetchVisitsButton.UseVisualStyleBackColor = true;
        fetchVisitsButton.Click += OnFetchVisitsClick;
        //
        // fromPicker
        //
        fromPicker.Format = DateTimePickerFormat.Short;
        fromPicker.Location = new Point(12, 197);
        fromPicker.Name = "fromPicker";
        fromPicker.Size = new Size(130, 23);
        fromPicker.TabIndex = 9;
        //
        // toPicker
        //
        toPicker.Format = DateTimePickerFormat.Short;
        toPicker.Location = new Point(160, 197);
        toPicker.Name = "toPicker";
        toPicker.Size = new Size(130, 23);
        toPicker.TabIndex = 10;
        //
        // fromLabel
        //
        fromLabel.AutoSize = true;
        fromLabel.Location = new Point(12, 179);
        fromLabel.Name = "fromLabel";
        fromLabel.Size = new Size(33, 15);
        fromLabel.TabIndex = 11;
        fromLabel.Text = "From";
        //
        // toLabel
        //
        toLabel.AutoSize = true;
        toLabel.Location = new Point(160, 179);
        toLabel.Name = "toLabel";
        toLabel.Size = new Size(20, 15);
        toLabel.TabIndex = 12;
        toLabel.Text = "To";
        //
        // visitsCountLabel
        //
        visitsCountLabel.AutoSize = true;
        visitsCountLabel.Location = new Point(450, 201);
        visitsCountLabel.Name = "visitsCountLabel";
        visitsCountLabel.Size = new Size(0, 15);
        visitsCountLabel.TabIndex = 13;
        //
        // pushMasterButton
        //
        pushMasterButton.AutoSize = true;
        pushMasterButton.Location = new Point(12, 240);
        pushMasterButton.MinimumSize = new Size(220, 28);
        pushMasterButton.Name = "pushMasterButton";
        pushMasterButton.Padding = new Padding(10, 3, 10, 3);
        pushMasterButton.TabIndex = 14;
        pushMasterButton.Text = "Push master";
        pushMasterButton.UseVisualStyleBackColor = true;
        pushMasterButton.Click += OnPushMasterClick;
        //
        // includePhotosCheckBox
        //
        includePhotosCheckBox.AutoSize = true;
        includePhotosCheckBox.Location = new Point(245, 244);
        includePhotosCheckBox.Name = "includePhotosCheckBox";
        includePhotosCheckBox.TabIndex = 16;
        includePhotosCheckBox.Text = "Include photos";
        includePhotosCheckBox.Checked = true;
        includePhotosCheckBox.CheckState = CheckState.Checked;
        includePhotosCheckBox.UseVisualStyleBackColor = true;
        //
        // pullMasterButton
        //
        pullMasterButton.AutoSize = true;
        pullMasterButton.Location = new Point(480, 240);
        pullMasterButton.MinimumSize = new Size(260, 28);
        pullMasterButton.Name = "pullMasterButton";
        pullMasterButton.Padding = new Padding(10, 3, 10, 3);
        pullMasterButton.TabIndex = 17;
        pullMasterButton.Text = "Pull master";
        pullMasterButton.UseVisualStyleBackColor = true;
        pullMasterButton.Click += OnPullMasterClick;
        //
        // downloadVisitsButton
        //
        downloadVisitsButton.AutoSize = true;
        downloadVisitsButton.Location = new Point(760, 240);
        downloadVisitsButton.MinimumSize = new Size(310, 28);
        downloadVisitsButton.Name = "downloadVisitsButton";
        downloadVisitsButton.Padding = new Padding(10, 3, 10, 3);
        downloadVisitsButton.TabIndex = 15;
        downloadVisitsButton.Text = "Pull visits.db";
        downloadVisitsButton.UseVisualStyleBackColor = true;
        downloadVisitsButton.Click += OnDownloadVisitsClick;
        //
        // statusLabel
        //
        statusLabel.AutoSize = true;
        statusLabel.Location = new Point(12, 280);
        statusLabel.Name = "statusLabel";
        statusLabel.Size = new Size(39, 15);
        statusLabel.TabIndex = 18;
        statusLabel.Text = "Status";
        //
        // statusTextBox
        //
        statusTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        statusTextBox.Location = new Point(12, 298);
        statusTextBox.Multiline = true;
        statusTextBox.Name = "statusTextBox";
        statusTextBox.ReadOnly = true;
        statusTextBox.ScrollBars = ScrollBars.Vertical;
        statusTextBox.Size = new Size(1068, 80);
        statusTextBox.TabIndex = 19;
        //
        // recentEventsLabel
        //
        recentEventsLabel.AutoSize = true;
        recentEventsLabel.Location = new Point(12, 385);
        recentEventsLabel.Name = "recentEventsLabel";
        recentEventsLabel.Size = new Size(81, 15);
        recentEventsLabel.TabIndex = 20;
        recentEventsLabel.Text = "Recent events";
        //
        // recentEventsList
        //
        recentEventsList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        recentEventsList.FormattingEnabled = true;
        recentEventsList.ItemHeight = 15;
        recentEventsList.Location = new Point(12, 403);
        recentEventsList.Name = "recentEventsList";
        recentEventsList.Size = new Size(1068, 94);
        recentEventsList.TabIndex = 21;
        //
        // RemoteSitesForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1100, 510);
        Controls.Add(recentEventsList);
        Controls.Add(recentEventsLabel);
        Controls.Add(statusTextBox);
        Controls.Add(statusLabel);
        Controls.Add(downloadVisitsButton);
        Controls.Add(pullMasterButton);
        Controls.Add(includePhotosCheckBox);
        Controls.Add(pushMasterButton);
        Controls.Add(visitsCountLabel);
        Controls.Add(toLabel);
        Controls.Add(fromLabel);
        Controls.Add(toPicker);
        Controls.Add(fromPicker);
        Controls.Add(fetchVisitsButton);
        Controls.Add(infoTextBox);
        Controls.Add(infoButton);
        Controls.Add(apiKeyTextBox);
        Controls.Add(apiKeyLabel);
        Controls.Add(portNumeric);
        Controls.Add(portLabel);
        Controls.Add(hostTextBox);
        Controls.Add(hostLabel);
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(900, 510);
        MaximizeBox = true;
        MinimizeBox = false;
        Name = "RemoteSitesForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Remote Sites";
        ((System.ComponentModel.ISupportInitialize)portNumeric).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label hostLabel;
    private TextBox hostTextBox;
    private Label portLabel;
    private NumericUpDown portNumeric;
    private Label apiKeyLabel;
    private TextBox apiKeyTextBox;
    private Button infoButton;
    private TextBox infoTextBox;
    private Button fetchVisitsButton;
    private DateTimePicker fromPicker;
    private DateTimePicker toPicker;
    private Label fromLabel;
    private Label toLabel;
    private Label visitsCountLabel;
    private Button pushMasterButton;
    private Button downloadVisitsButton;
    private CheckBox includePhotosCheckBox;
    private Button pullMasterButton;
    private Label statusLabel;
    private TextBox statusTextBox;
    private Label recentEventsLabel;
    private ListBox recentEventsList;
}
