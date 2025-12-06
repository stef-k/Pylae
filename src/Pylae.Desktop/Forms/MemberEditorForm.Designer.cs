using System.Drawing;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class MemberEditorForm
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
        memberNumberLabel = new Label();
        memberNumberTextBox = new TextBox();
        firstNameLabel = new Label();
        firstNameTextBox = new TextBox();
        lastNameLabel = new Label();
        lastNameTextBox = new TextBox();
        businessRankLabel = new Label();
        businessRankTextBox = new TextBox();
        personalIdLabel = new Label();
        personalIdTextBox = new TextBox();
        businessIdLabel = new Label();
        businessIdTextBox = new TextBox();
        officeLabel = new Label();
        officeTextBox = new TextBox();
        memberTypeLabel = new Label();
        memberTypeCombo = new ComboBox();
        isPermanentCheckBox = new CheckBox();
        saveButton = new Button();
        cancelButton = new Button();
        photoLabel = new Label();
        photoButton = new Button();
        clearPhotoButton = new Button();
        SuspendLayout();
        // 
        // memberNumberLabel
        // 
        memberNumberLabel.AutoSize = true;
        memberNumberLabel.Location = new Point(12, 15);
        memberNumberLabel.Name = "memberNumberLabel";
        memberNumberLabel.Size = new Size(97, 15);
        memberNumberLabel.TabIndex = 0;
        memberNumberLabel.Text = "Member Number";
        // 
        // memberNumberTextBox
        // 
        memberNumberTextBox.Location = new Point(12, 33);
        memberNumberTextBox.Name = "memberNumberTextBox";
        memberNumberTextBox.Size = new Size(120, 23);
        memberNumberTextBox.TabIndex = 1;
        // 
        // firstNameLabel
        // 
        firstNameLabel.AutoSize = true;
        firstNameLabel.Location = new Point(12, 65);
        firstNameLabel.Name = "firstNameLabel";
        firstNameLabel.Size = new Size(64, 15);
        firstNameLabel.TabIndex = 2;
        firstNameLabel.Text = "First Name";
        // 
        // firstNameTextBox
        // 
        firstNameTextBox.Location = new Point(12, 83);
        firstNameTextBox.Name = "firstNameTextBox";
        firstNameTextBox.Size = new Size(180, 23);
        firstNameTextBox.TabIndex = 3;
        // 
        // lastNameLabel
        // 
        lastNameLabel.AutoSize = true;
        lastNameLabel.Location = new Point(210, 65);
        lastNameLabel.Name = "lastNameLabel";
        lastNameLabel.Size = new Size(63, 15);
        lastNameLabel.TabIndex = 4;
        lastNameLabel.Text = "Last Name";
        // 
        // lastNameTextBox
        // 
        lastNameTextBox.Location = new Point(210, 83);
        lastNameTextBox.Name = "lastNameTextBox";
        lastNameTextBox.Size = new Size(180, 23);
        lastNameTextBox.TabIndex = 5;
        // 
        // businessRankLabel
        // 
        businessRankLabel.AutoSize = true;
        businessRankLabel.Location = new Point(12, 115);
        businessRankLabel.Name = "businessRankLabel";
        businessRankLabel.Size = new Size(86, 15);
        businessRankLabel.TabIndex = 6;
        businessRankLabel.Text = "Business Rank";
        // 
        // businessRankTextBox
        // 
        businessRankTextBox.Location = new Point(12, 133);
        businessRankTextBox.Name = "businessRankTextBox";
        businessRankTextBox.Size = new Size(180, 23);
        businessRankTextBox.TabIndex = 7;
        // 
        // personalIdLabel
        // 
        personalIdLabel.AutoSize = true;
        personalIdLabel.Location = new Point(210, 115);
        personalIdLabel.Name = "personalIdLabel";
        personalIdLabel.Size = new Size(70, 15);
        personalIdLabel.TabIndex = 8;
        personalIdLabel.Text = "Personal ID";
        // 
        // personalIdTextBox
        // 
        personalIdTextBox.Location = new Point(210, 133);
        personalIdTextBox.Name = "personalIdTextBox";
        personalIdTextBox.Size = new Size(180, 23);
        personalIdTextBox.TabIndex = 9;
        // 
        // businessIdLabel
        // 
        businessIdLabel.AutoSize = true;
        businessIdLabel.Location = new Point(12, 165);
        businessIdLabel.Name = "businessIdLabel";
        businessIdLabel.Size = new Size(71, 15);
        businessIdLabel.TabIndex = 10;
        businessIdLabel.Text = "Business ID";
        // 
        // businessIdTextBox
        // 
        businessIdTextBox.Location = new Point(12, 183);
        businessIdTextBox.Name = "businessIdTextBox";
        businessIdTextBox.Size = new Size(180, 23);
        businessIdTextBox.TabIndex = 11;
        // 
        // officeLabel
        // 
        officeLabel.AutoSize = true;
        officeLabel.Location = new Point(210, 165);
        officeLabel.Name = "officeLabel";
        officeLabel.Size = new Size(38, 15);
        officeLabel.TabIndex = 12;
        officeLabel.Text = "Office";
        //
        // officeTextBox
        //
        officeTextBox.Location = new Point(210, 183);
        officeTextBox.Name = "officeTextBox";
        officeTextBox.Size = new Size(180, 23);
        officeTextBox.TabIndex = 13;
        // 
        // memberTypeLabel
        // 
        memberTypeLabel.AutoSize = true;
        memberTypeLabel.Location = new Point(12, 215);
        memberTypeLabel.Name = "memberTypeLabel";
        memberTypeLabel.Size = new Size(82, 15);
        memberTypeLabel.TabIndex = 14;
        memberTypeLabel.Text = "Member Type";
        // 
        // memberTypeCombo
        // 
        memberTypeCombo.DropDownStyle = ComboBoxStyle.DropDownList;
        memberTypeCombo.FormattingEnabled = true;
        memberTypeCombo.Location = new Point(12, 233);
        memberTypeCombo.Name = "memberTypeCombo";
        memberTypeCombo.Size = new Size(180, 23);
        memberTypeCombo.TabIndex = 15;
        // 
        // isPermanentCheckBox
        // 
        isPermanentCheckBox.AutoSize = true;
        isPermanentCheckBox.Location = new Point(210, 235);
        isPermanentCheckBox.Name = "isPermanentCheckBox";
        isPermanentCheckBox.Size = new Size(114, 19);
        isPermanentCheckBox.TabIndex = 16;
        isPermanentCheckBox.Text = "Permanent staff";
        isPermanentCheckBox.UseVisualStyleBackColor = true;
        //
        // saveButton
        //
        saveButton.ForeColor = Color.Blue;
        saveButton.Location = new Point(234, 275);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(75, 30);
        saveButton.TabIndex = 17;
        saveButton.Text = "Save";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += OnSaveClick;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(315, 275);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 30);
        cancelButton.TabIndex = 18;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += OnCancelClick;

        // 
        // photoLabel
        // 
        photoLabel.AutoSize = true;
        photoLabel.Location = new Point(12, 215);
        photoLabel.Name = "photoLabel";
        photoLabel.Size = new Size(61, 15);
        photoLabel.TabIndex = 19;
        photoLabel.Text = "Photo: n/a";

        // 
        // photoButton
        // 
        photoButton.Location = new Point(210, 228);
        photoButton.Name = "photoButton";
        photoButton.Size = new Size(180, 23);
        photoButton.TabIndex = 20;
        photoButton.Text = "Choose Photo";
        photoButton.UseVisualStyleBackColor = true;
        photoButton.Click += OnChoosePhotoClick;
        // 
        // clearPhotoButton
        // 
        clearPhotoButton.Location = new Point(210, 257);
        clearPhotoButton.Name = "clearPhotoButton";
        clearPhotoButton.Size = new Size(180, 23);
        clearPhotoButton.TabIndex = 21;
        clearPhotoButton.Text = "Clear Photo";
        clearPhotoButton.UseVisualStyleBackColor = true;
        clearPhotoButton.Click += OnClearPhotoClick;
        // 
        // MemberEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(414, 345);
        Controls.Add(clearPhotoButton);
        Controls.Add(photoButton);
        Controls.Add(photoLabel);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(isPermanentCheckBox);
        Controls.Add(memberTypeCombo);
        Controls.Add(memberTypeLabel);
        Controls.Add(officeTextBox);
        Controls.Add(officeLabel);
        Controls.Add(businessIdTextBox);
        Controls.Add(businessIdLabel);
        Controls.Add(personalIdTextBox);
        Controls.Add(personalIdLabel);
        Controls.Add(businessRankTextBox);
        Controls.Add(businessRankLabel);
        Controls.Add(lastNameTextBox);
        Controls.Add(lastNameLabel);
        Controls.Add(firstNameTextBox);
        Controls.Add(firstNameLabel);
        Controls.Add(memberNumberTextBox);
        Controls.Add(memberNumberLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MemberEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Member";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label memberNumberLabel;
    private TextBox memberNumberTextBox;
    private Label firstNameLabel;
    private TextBox firstNameTextBox;
    private Label lastNameLabel;
    private TextBox lastNameTextBox;
    private Label businessRankLabel;
    private TextBox businessRankTextBox;
    private Label personalIdLabel;
    private TextBox personalIdTextBox;
    private Label businessIdLabel;
    private TextBox businessIdTextBox;
    private Label officeLabel;
    private TextBox officeTextBox;
    private Label memberTypeLabel;
    private ComboBox memberTypeCombo;
    private CheckBox isPermanentCheckBox;
    private Button saveButton;
    private Button cancelButton;
    private Label photoLabel;
    private Button photoButton;
    private Button clearPhotoButton;
}
