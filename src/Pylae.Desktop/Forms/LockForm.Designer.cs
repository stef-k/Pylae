using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class LockForm
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
        promptLabel = new Label();
        passwordText = new TextBox();
        quickCodeText = new TextBox();
        unlockButton = new Button();
        cancelButton = new Button();
        passwordLabel = new Label();
        quickCodeLabel = new Label();
        switchUserButton = new Button();
        currentUserLabel = new Label();
        SuspendLayout();
        // 
        // promptLabel
        // 
        promptLabel.AutoSize = true;
        promptLabel.Location = new Point(12, 9);
        promptLabel.Name = "promptLabel";
        promptLabel.Size = new Size(38, 15);
        promptLabel.TabIndex = 0;
        promptLabel.Text = "Prompt";
        // 
        // currentUserLabel
        // 
        currentUserLabel.AutoSize = true;
        currentUserLabel.Location = new Point(12, 30);
        currentUserLabel.Name = "currentUserLabel";
        currentUserLabel.Size = new Size(76, 15);
        currentUserLabel.TabIndex = 1;
        currentUserLabel.Text = "Current user";
        // 
        // passwordText
        // 
        passwordText.Location = new Point(12, 73);
        passwordText.Name = "passwordText";
        passwordText.Size = new Size(200, 23);
        passwordText.TabIndex = 3;
        passwordText.UseSystemPasswordChar = true;
        // 
        // quickCodeText
        // 
        quickCodeText.Location = new Point(12, 119);
        quickCodeText.MaxLength = 6;
        quickCodeText.Name = "quickCodeText";
        quickCodeText.Size = new Size(120, 23);
        quickCodeText.TabIndex = 4;
        // 
        // unlockButton
        // 
        unlockButton.Location = new Point(126, 162);
        unlockButton.Name = "unlockButton";
        unlockButton.Size = new Size(86, 30);
        unlockButton.TabIndex = 6;
        unlockButton.Text = "Unlock";
        unlockButton.UseVisualStyleBackColor = true;
        unlockButton.Click += OnUnlockClick;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(218, 162);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(86, 30);
        cancelButton.TabIndex = 7;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += OnCancelClick;
        // 
        // passwordLabel
        // 
        passwordLabel.AutoSize = true;
        passwordLabel.Location = new Point(12, 55);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(57, 15);
        passwordLabel.TabIndex = 2;
        passwordLabel.Text = "Password";
        // 
        // quickCodeLabel
        // 
        quickCodeLabel.AutoSize = true;
        quickCodeLabel.Location = new Point(12, 101);
        quickCodeLabel.Name = "quickCodeLabel";
        quickCodeLabel.Size = new Size(66, 15);
        quickCodeLabel.TabIndex = 5;
        quickCodeLabel.Text = "Quick code";
        // 
        // switchUserButton
        // 
        switchUserButton.Location = new Point(12, 162);
        switchUserButton.Name = "switchUserButton";
        switchUserButton.Size = new Size(108, 30);
        switchUserButton.TabIndex = 8;
        switchUserButton.Text = "Switch user";
        switchUserButton.UseVisualStyleBackColor = true;
        switchUserButton.Click += OnSwitchUserClick;
        // 
        // LockForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(316, 210);
        Controls.Add(switchUserButton);
        Controls.Add(currentUserLabel);
        Controls.Add(quickCodeLabel);
        Controls.Add(passwordLabel);
        Controls.Add(cancelButton);
        Controls.Add(unlockButton);
        Controls.Add(quickCodeText);
        Controls.Add(passwordText);
        Controls.Add(promptLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LockForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Locked";
        Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "pylae_icon.ico"));
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label promptLabel;
    private TextBox passwordText;
    private TextBox quickCodeText;
    private Button unlockButton;
    private Button cancelButton;
    private Label passwordLabel;
    private Label quickCodeLabel;
    private Button switchUserButton;
    private Label currentUserLabel;
}
