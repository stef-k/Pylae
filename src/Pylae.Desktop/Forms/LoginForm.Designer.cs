using System.Drawing;
using System.IO;
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
        orLabel = new Label();
        quickCodeLabel = new Label();
        quickCodeTextBox = new TextBox();
        loginButton = new Button();
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
        usernameTextBox.TextChanged += OnCredentialTextChanged;
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
        passwordTextBox.TextChanged += OnCredentialTextChanged;
        //
        // orLabel
        //
        orLabel.AutoSize = true;
        orLabel.ForeColor = Color.Gray;
        orLabel.Location = new Point(22, 155);
        orLabel.Name = "orLabel";
        orLabel.Size = new Size(18, 15);
        orLabel.TabIndex = 5;
        orLabel.Text = "—";
        //
        // quickCodeLabel
        //
        quickCodeLabel.AutoSize = true;
        quickCodeLabel.Location = new Point(22, 180);
        quickCodeLabel.Name = "quickCodeLabel";
        quickCodeLabel.Size = new Size(66, 15);
        quickCodeLabel.TabIndex = 6;
        quickCodeLabel.Text = "Quick code";
        //
        // quickCodeTextBox
        //
        quickCodeTextBox.Location = new Point(22, 198);
        quickCodeTextBox.MaxLength = 6;
        quickCodeTextBox.Name = "quickCodeTextBox";
        quickCodeTextBox.Size = new Size(120, 23);
        quickCodeTextBox.TabIndex = 7;
        quickCodeTextBox.TextChanged += OnQuickCodeTextChanged;
        //
        // loginButton
        //
        loginButton.Location = new Point(22, 235);
        loginButton.Name = "loginButton";
        loginButton.Size = new Size(240, 35);
        loginButton.TabIndex = 8;
        loginButton.Text = "Login";
        loginButton.UseVisualStyleBackColor = true;
        loginButton.Click += OnLoginClick;
        //
        // errorLabel
        //
        errorLabel.AutoSize = true;
        errorLabel.ForeColor = Color.IndianRed;
        errorLabel.Location = new Point(22, 280);
        errorLabel.Name = "errorLabel";
        errorLabel.Size = new Size(0, 15);
        errorLabel.TabIndex = 9;
        //
        // LoginForm
        //
        AcceptButton = loginButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(285, 310);
        Controls.Add(errorLabel);
        Controls.Add(loginButton);
        Controls.Add(quickCodeTextBox);
        Controls.Add(quickCodeLabel);
        Controls.Add(orLabel);
        Controls.Add(passwordTextBox);
        Controls.Add(passwordLabel);
        Controls.Add(usernameTextBox);
        Controls.Add(usernameLabel);
        Controls.Add(titleLabel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "LoginForm";
        Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "pylae_icon.ico"));
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
    private Label orLabel;
    private Label quickCodeLabel;
    private TextBox quickCodeTextBox;
    private Button loginButton;
    private Label errorLabel;
}
