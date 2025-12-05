using System.Drawing;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class UserEditorForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
        usernameLabel = new Label();
        usernameText = new TextBox();
        firstNameLabel = new Label();
        firstNameText = new TextBox();
        lastNameLabel = new Label();
        lastNameText = new TextBox();
        roleLabel = new Label();
        roleCombo = new ComboBox();
        sharedCheck = new CheckBox();
        activeCheck = new CheckBox();
        passwordLabel = new Label();
        passwordText = new TextBox();
        quickCodeLabel = new Label();
        quickCodeText = new TextBox();
        saveButton = new Button();
        cancelButton = new Button();
        SuspendLayout();
        // 
        // usernameLabel
        // 
        usernameLabel.AutoSize = true;
        usernameLabel.Location = new Point(12, 15);
        usernameLabel.Name = "usernameLabel";
        usernameLabel.Size = new Size(63, 15);
        usernameLabel.TabIndex = 0;
        usernameLabel.Text = "Username";
        // 
        // usernameText
        // 
        usernameText.Location = new Point(12, 33);
        usernameText.Name = "usernameText";
        usernameText.Size = new Size(220, 23);
        usernameText.TabIndex = 1;
        // 
        // firstNameLabel
        // 
        firstNameLabel.AutoSize = true;
        firstNameLabel.Location = new Point(12, 65);
        firstNameLabel.Name = "firstNameLabel";
        firstNameLabel.Size = new Size(64, 15);
        firstNameLabel.TabIndex = 2;
        firstNameLabel.Text = "First name";
        // 
        // firstNameText
        // 
        firstNameText.Location = new Point(12, 83);
        firstNameText.Name = "firstNameText";
        firstNameText.Size = new Size(220, 23);
        firstNameText.TabIndex = 3;
        // 
        // lastNameLabel
        // 
        lastNameLabel.AutoSize = true;
        lastNameLabel.Location = new Point(12, 115);
        lastNameLabel.Name = "lastNameLabel";
        lastNameLabel.Size = new Size(63, 15);
        lastNameLabel.TabIndex = 4;
        lastNameLabel.Text = "Last name";
        // 
        // lastNameText
        // 
        lastNameText.Location = new Point(12, 133);
        lastNameText.Name = "lastNameText";
        lastNameText.Size = new Size(220, 23);
        lastNameText.TabIndex = 5;
        // 
        // roleLabel
        // 
        roleLabel.AutoSize = true;
        roleLabel.Location = new Point(12, 165);
        roleLabel.Name = "roleLabel";
        roleLabel.Size = new Size(29, 15);
        roleLabel.TabIndex = 6;
        roleLabel.Text = "Role";
        // 
        // roleCombo
        // 
        roleCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        roleCombo.FormattingEnabled = true;
        roleCombo.Location = new Point(12, 183);
        roleCombo.Name = "roleCombo";
        roleCombo.Size = new Size(140, 23);
        roleCombo.TabIndex = 7;
        roleCombo.SelectedIndexChanged += OnRoleChanged;
        // 
        // sharedCheck
        // 
        sharedCheck.AutoSize = true;
        sharedCheck.Location = new Point(170, 185);
        sharedCheck.Name = "sharedCheck";
        sharedCheck.Size = new Size(95, 19);
        sharedCheck.TabIndex = 8;
        sharedCheck.Text = "Shared login";
        sharedCheck.UseVisualStyleBackColor = true;
        // 
        // activeCheck
        // 
        activeCheck.AutoSize = true;
        activeCheck.Location = new Point(12, 212);
        activeCheck.Name = "activeCheck";
        activeCheck.Size = new Size(58, 19);
        activeCheck.TabIndex = 9;
        activeCheck.Text = "Active";
        activeCheck.UseVisualStyleBackColor = true;
        // 
        // passwordLabel
        // 
        passwordLabel.AutoSize = true;
        passwordLabel.Location = new Point(12, 244);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(57, 15);
        passwordLabel.TabIndex = 10;
        passwordLabel.Text = "Password";
        // 
        // passwordText
        // 
        passwordText.Location = new Point(12, 262);
        passwordText.Name = "passwordText";
        passwordText.Size = new Size(220, 23);
        passwordText.TabIndex = 11;
        passwordText.UseSystemPasswordChar = true;
        // 
        // quickCodeLabel
        // 
        quickCodeLabel.AutoSize = true;
        quickCodeLabel.Location = new Point(12, 294);
        quickCodeLabel.Name = "quickCodeLabel";
        quickCodeLabel.Size = new Size(66, 15);
        quickCodeLabel.TabIndex = 12;
        quickCodeLabel.Text = "Quick code";
        // 
        // quickCodeText
        // 
        quickCodeText.Location = new Point(12, 312);
        quickCodeText.MaxLength = 6;
        quickCodeText.Name = "quickCodeText";
        quickCodeText.Size = new Size(100, 23);
        quickCodeText.TabIndex = 13;
        // 
        // saveButton
        // 
        saveButton.Location = new Point(152, 350);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(80, 30);
        saveButton.TabIndex = 14;
        saveButton.Text = "Save";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += OnSaveClick;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(238, 350);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(80, 30);
        cancelButton.TabIndex = 15;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += OnCancelClick;
        // 
        // UserEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(334, 392);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(quickCodeText);
        Controls.Add(quickCodeLabel);
        Controls.Add(passwordText);
        Controls.Add(passwordLabel);
        Controls.Add(activeCheck);
        Controls.Add(sharedCheck);
        Controls.Add(roleCombo);
        Controls.Add(roleLabel);
        Controls.Add(lastNameText);
        Controls.Add(lastNameLabel);
        Controls.Add(firstNameText);
        Controls.Add(firstNameLabel);
        Controls.Add(usernameText);
        Controls.Add(usernameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "UserEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "User";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label usernameLabel;
    private TextBox usernameText;
    private Label firstNameLabel;
    private TextBox firstNameText;
    private Label lastNameLabel;
    private TextBox lastNameText;
    private Label roleLabel;
    private ComboBox roleCombo;
    private CheckBox sharedCheck;
    private CheckBox activeCheck;
    private Label passwordLabel;
    private TextBox passwordText;
    private Label quickCodeLabel;
    private TextBox quickCodeText;
    private Button saveButton;
    private Button cancelButton;
}
