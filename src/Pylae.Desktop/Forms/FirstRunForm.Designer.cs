using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class FirstRunForm
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
        siteCodeLabel = new Label();
        siteCodeHint = new Label();
        siteCodeText = new TextBox();
        siteNameLabel = new Label();
        siteNameText = new TextBox();
        siteNameHint = new Label();
        adminPasswordLabel = new Label();
        adminPasswordText = new TextBox();
        adminHint = new Label();
        encryptionPasswordLabel = new Label();
        encryptionPasswordText = new TextBox();
        encryptionHint = new Label();
        languageLabel = new Label();
        languageCombo = new ComboBox();
        saveButton = new Button();
        cancelButton = new Button();
        SuspendLayout();
        // 
        // siteCodeLabel
        // 
        siteCodeLabel.AutoSize = true;
        siteCodeLabel.Location = new Point(12, 15);
        siteCodeLabel.Name = "siteCodeLabel";
        siteCodeLabel.Size = new Size(56, 15);
        siteCodeLabel.TabIndex = 0;
        siteCodeLabel.Text = "Site code";
        // 
        // siteCodeHint
        // 
        siteCodeHint.AutoSize = true;
        siteCodeHint.ForeColor = SystemColors.GrayText;
        siteCodeHint.Location = new Point(12, 59);
        siteCodeHint.Name = "siteCodeHint";
        siteCodeHint.Size = new Size(225, 15);
        siteCodeHint.TabIndex = 2;
        siteCodeHint.Text = "Lowercase, digits, dash/underscore allowed.";
        // 
        // siteCodeText
        // 
        siteCodeText.Location = new Point(12, 33);
        siteCodeText.Name = "siteCodeText";
        siteCodeText.Size = new Size(180, 23);
        siteCodeText.TabIndex = 1;
        // 
        // siteNameLabel
        // 
        siteNameLabel.AutoSize = true;
        siteNameLabel.Location = new Point(12, 82);
        siteNameLabel.Name = "siteNameLabel";
        siteNameLabel.Size = new Size(95, 15);
        siteNameLabel.TabIndex = 3;
        siteNameLabel.Text = "Site display name";
        // 
        // siteNameText
        // 
        siteNameText.Location = new Point(12, 100);
        siteNameText.Name = "siteNameText";
        siteNameText.Size = new Size(323, 23);
        siteNameText.TabIndex = 4;
        // 
        // siteNameHint
        // 
        siteNameHint.AutoSize = true;
        siteNameHint.ForeColor = SystemColors.GrayText;
        siteNameHint.Location = new Point(12, 126);
        siteNameHint.Name = "siteNameHint";
        siteNameHint.Size = new Size(250, 15);
        siteNameHint.TabIndex = 5;
        siteNameHint.Text = "Shown on title bar and badge; can include Greek.";
        // 
        // adminPasswordLabel
        // 
        adminPasswordLabel.AutoSize = true;
        adminPasswordLabel.Location = new Point(12, 150);
        adminPasswordLabel.Name = "adminPasswordLabel";
        adminPasswordLabel.Size = new Size(95, 15);
        adminPasswordLabel.TabIndex = 6;
        adminPasswordLabel.Text = "Admin password";
        // 
        // adminPasswordText
        // 
        adminPasswordText.Location = new Point(12, 168);
        adminPasswordText.Name = "adminPasswordText";
        adminPasswordText.PasswordChar = '*';
        adminPasswordText.Size = new Size(180, 23);
        adminPasswordText.TabIndex = 7;
        // 
        // adminHint
        // 
        adminHint.AutoSize = true;
        adminHint.ForeColor = SystemColors.GrayText;
        adminHint.Location = new Point(12, 194);
        adminHint.Name = "adminHint";
        adminHint.Size = new Size(166, 15);
        adminHint.TabIndex = 8;
        adminHint.Text = "Default admin username: admin";
        // 
        // encryptionPasswordLabel
        // 
        encryptionPasswordLabel.AutoSize = true;
        encryptionPasswordLabel.Location = new Point(12, 219);
        encryptionPasswordLabel.Name = "encryptionPasswordLabel";
        encryptionPasswordLabel.Size = new Size(120, 15);
        encryptionPasswordLabel.TabIndex = 9;
        encryptionPasswordLabel.Text = "Encryption password";
        // 
        // encryptionPasswordText
        // 
        encryptionPasswordText.Location = new Point(12, 237);
        encryptionPasswordText.Name = "encryptionPasswordText";
        encryptionPasswordText.PasswordChar = '*';
        encryptionPasswordText.Size = new Size(180, 23);
        encryptionPasswordText.TabIndex = 10;
        // 
        // encryptionHint
        // 
        encryptionHint.AutoSize = true;
        encryptionHint.ForeColor = SystemColors.GrayText;
        encryptionHint.Location = new Point(12, 263);
        encryptionHint.Name = "encryptionHint";
        encryptionHint.Size = new Size(306, 15);
        encryptionHint.TabIndex = 11;
        encryptionHint.Text = "Required to open the databases. Keep it safe; cannot be reset.";
        // 
        // languageLabel
        // 
        languageLabel.AutoSize = true;
        languageLabel.Location = new Point(12, 292);
        languageLabel.Name = "languageLabel";
        languageLabel.Size = new Size(60, 15);
        languageLabel.TabIndex = 12;
        languageLabel.Text = "Language";
        // 
        // languageCombo
        // 
        languageCombo.Location = new Point(12, 310);
        languageCombo.Name = "languageCombo";
        languageCombo.Size = new Size(180, 23);
        languageCombo.TabIndex = 13;
        // 
        // saveButton
        // 
        saveButton.Location = new Point(179, 349);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(75, 23);
        saveButton.TabIndex = 14;
        saveButton.Text = "Save";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += OnSaveClick;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(260, 349);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 23);
        cancelButton.TabIndex = 15;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += OnCancelClick;
        // 
        // FirstRunForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(347, 384);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(languageCombo);
        Controls.Add(languageLabel);
        Controls.Add(encryptionHint);
        Controls.Add(encryptionPasswordText);
        Controls.Add(encryptionPasswordLabel);
        Controls.Add(adminHint);
        Controls.Add(adminPasswordText);
        Controls.Add(adminPasswordLabel);
        Controls.Add(siteNameHint);
        Controls.Add(siteNameText);
        Controls.Add(siteNameLabel);
        Controls.Add(siteCodeHint);
        Controls.Add(siteCodeText);
        Controls.Add(siteCodeLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FirstRunForm";
        StartPosition = FormStartPosition.CenterScreen;
        Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "pylae_icon.ico"));
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label siteCodeLabel;
    private Label siteCodeHint;
    private TextBox siteCodeText;
    private Label siteNameLabel;
    private TextBox siteNameText;
    private Label siteNameHint;
    private Label adminPasswordLabel;
    private TextBox adminPasswordText;
    private Label adminHint;
    private Label encryptionPasswordLabel;
    private TextBox encryptionPasswordText;
    private Label encryptionHint;
    private Label languageLabel;
    private ComboBox languageCombo;
    private Button saveButton;
    private Button cancelButton;
}
