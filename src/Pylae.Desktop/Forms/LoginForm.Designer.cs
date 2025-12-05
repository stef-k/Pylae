using System.Drawing;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class LoginForm
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
        components = new System.ComponentModel.Container();
        titleLabel = new Label();
        usernameLabel = new Label();
        usernameTextBox = new TextBox();
        passwordLabel = new Label();
        passwordTextBox = new TextBox();
        quickCodeLabel = new Label();
        quickCodeTextBox = new TextBox();
        loginButton = new Button();
        quickCodeButton = new Button();
        errorLabel = new Label();
        SuspendLayout();
        // 
        // titleLabel
        // 
        titleLabel.AutoSize = true;
        titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        titleLabel.Location = new Point(20, 15);
        titleLabel.Name = "titleLabel";
        titleLabel.Size = new Size(89, 25);
        titleLabel.TabIndex = 0;
        titleLabel.Text = "Sign in";
        // 
        // usernameLabel
        // 
        usernameLabel.AutoSize = true;
        usernameLabel.Location = new Point(22, 55);
        usernameLabel.Name = "usernameLabel";
        usernameLabel.Size = new Size(63, 15);
        usernameLabel.TabIndex = 1;
        usernameLabel.Text = "Username";
        // 
        // usernameTextBox
        // 
        usernameTextBox.Location = new Point(22, 73);
        usernameTextBox.Name = "usernameTextBox";
        usernameTextBox.Size = new Size(240, 23);
        usernameTextBox.TabIndex = 2;
        // 
        // passwordLabel
        // 
        passwordLabel.AutoSize = true;
        passwordLabel.Location = new Point(22, 105);
        passwordLabel.Name = "passwordLabel";
        passwordLabel.Size = new Size(57, 15);
        passwordLabel.TabIndex = 3;
        passwordLabel.Text = "Password";
        // 
        // passwordTextBox
        // 
        passwordTextBox.Location = new Point(22, 123);
        passwordTextBox.Name = "passwordTextBox";
        passwordTextBox.Size = new Size(240, 23);
        passwordTextBox.TabIndex = 4;
        passwordTextBox.UseSystemPasswordChar = true;
        // 
        // quickCodeLabel
        // 
        quickCodeLabel.AutoSize = true;
        quickCodeLabel.Location = new Point(22, 155);
        quickCodeLabel.Name = "quickCodeLabel";
        quickCodeLabel.Size = new Size(66, 15);
        quickCodeLabel.TabIndex = 5;
        quickCodeLabel.Text = "Quick code";
        // 
        // quickCodeTextBox
        // 
        quickCodeTextBox.Location = new Point(22, 173);
        quickCodeTextBox.MaxLength = 6;
        quickCodeTextBox.Name = "quickCodeTextBox";
        quickCodeTextBox.Size = new Size(120, 23);
        quickCodeTextBox.TabIndex = 6;
        // 
        // loginButton
        // 
        loginButton.Location = new Point(280, 73);
        loginButton.Name = "loginButton";
        loginButton.Size = new Size(100, 40);
        loginButton.TabIndex = 7;
        loginButton.Text = "Login";
        loginButton.UseVisualStyleBackColor = true;
        loginButton.Click += OnPasswordLoginClick;
        // 
        // quickCodeButton
        // 
        quickCodeButton.Location = new Point(280, 123);
        quickCodeButton.Name = "quickCodeButton";
        quickCodeButton.Size = new Size(100, 40);
        quickCodeButton.TabIndex = 8;
        quickCodeButton.Text = "Quick login";
        quickCodeButton.UseVisualStyleBackColor = true;
        quickCodeButton.Click += OnQuickCodeLoginClick;
        // 
        // errorLabel
        // 
        errorLabel.AutoSize = true;
        errorLabel.ForeColor = Color.IndianRed;
        errorLabel.Location = new Point(22, 210);
        errorLabel.Name = "errorLabel";
        errorLabel.Size = new Size(0, 15);
        errorLabel.TabIndex = 9;
        // 
        // LoginForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(400, 240);
        Controls.Add(errorLabel);
        Controls.Add(quickCodeButton);
        Controls.Add(loginButton);
        Controls.Add(quickCodeTextBox);
        Controls.Add(quickCodeLabel);
        Controls.Add(passwordTextBox);
        Controls.Add(passwordLabel);
        Controls.Add(usernameTextBox);
        Controls.Add(usernameLabel);
        Controls.Add(titleLabel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label titleLabel;
    private Label usernameLabel;
    private TextBox usernameTextBox;
    private Label passwordLabel;
    private TextBox passwordTextBox;
    private Label quickCodeLabel;
    private TextBox quickCodeTextBox;
    private Button loginButton;
    private Button quickCodeButton;
    private Label errorLabel;
}
