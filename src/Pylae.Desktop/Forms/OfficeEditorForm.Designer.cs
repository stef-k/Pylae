using System.Drawing;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class OfficeEditorForm
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
        codeLabel = new Label();
        codeText = new TextBox();
        nameLabel = new Label();
        nameText = new TextBox();
        phoneLabel = new Label();
        phoneText = new TextBox();
        headNameLabel = new Label();
        headNameText = new TextBox();
        headTitleLabel = new Label();
        headTitleText = new TextBox();
        headRankLabel = new Label();
        headRankText = new TextBox();
        notesLabel = new Label();
        notesText = new TextBox();
        isActiveCheck = new CheckBox();
        displayOrderLabel = new Label();
        displayOrderNumeric = new NumericUpDown();
        saveButton = new Button();
        cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)displayOrderNumeric).BeginInit();
        SuspendLayout();
        // 
        // codeLabel
        // 
        codeLabel.AutoSize = true;
        codeLabel.Location = new Point(12, 15);
        codeLabel.Name = "codeLabel";
        codeLabel.Size = new Size(34, 15);
        codeLabel.TabIndex = 0;
        codeLabel.Text = "Code";
        // 
        // codeText
        // 
        codeText.Location = new Point(12, 33);
        codeText.Name = "codeText";
        codeText.Size = new Size(120, 23);
        codeText.TabIndex = 1;
        // 
        // nameLabel
        // 
        nameLabel.AutoSize = true;
        nameLabel.Location = new Point(150, 15);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(39, 15);
        nameLabel.TabIndex = 2;
        nameLabel.Text = "Name";
        // 
        // nameText
        // 
        nameText.Location = new Point(150, 33);
        nameText.Name = "nameText";
        nameText.Size = new Size(220, 23);
        nameText.TabIndex = 3;
        // 
        // phoneLabel
        // 
        phoneLabel.AutoSize = true;
        phoneLabel.Location = new Point(12, 65);
        phoneLabel.Name = "phoneLabel";
        phoneLabel.Size = new Size(41, 15);
        phoneLabel.TabIndex = 4;
        phoneLabel.Text = "Phone";
        // 
        // phoneText
        // 
        phoneText.Location = new Point(12, 83);
        phoneText.Name = "phoneText";
        phoneText.Size = new Size(120, 23);
        phoneText.TabIndex = 5;
        // 
        // headNameLabel
        // 
        headNameLabel.AutoSize = true;
        headNameLabel.Location = new Point(150, 65);
        headNameLabel.Name = "headNameLabel";
        headNameLabel.Size = new Size(109, 15);
        headNameLabel.TabIndex = 6;
        headNameLabel.Text = "Head Full Name";
        // 
        // headNameText
        // 
        headNameText.Location = new Point(150, 83);
        headNameText.Name = "headNameText";
        headNameText.Size = new Size(220, 23);
        headNameText.TabIndex = 7;
        // 
        // headTitleLabel
        // 
        headTitleLabel.AutoSize = true;
        headTitleLabel.Location = new Point(12, 115);
        headTitleLabel.Name = "headTitleLabel";
        headTitleLabel.Size = new Size(96, 15);
        headTitleLabel.TabIndex = 8;
        headTitleLabel.Text = "Head Job Title";
        // 
        // headTitleText
        // 
        headTitleText.Location = new Point(12, 133);
        headTitleText.Name = "headTitleText";
        headTitleText.Size = new Size(120, 23);
        headTitleText.TabIndex = 9;
        // 
        // headRankLabel
        // 
        headRankLabel.AutoSize = true;
        headRankLabel.Location = new Point(150, 115);
        headRankLabel.Name = "headRankLabel";
        headRankLabel.Size = new Size(90, 15);
        headRankLabel.TabIndex = 10;
        headRankLabel.Text = "Head Rank";
        // 
        // headRankText
        // 
        headRankText.Location = new Point(150, 133);
        headRankText.Name = "headRankText";
        headRankText.Size = new Size(120, 23);
        headRankText.TabIndex = 11;
        // 
        // notesLabel
        // 
        notesLabel.AutoSize = true;
        notesLabel.Location = new Point(12, 165);
        notesLabel.Name = "notesLabel";
        notesLabel.Size = new Size(38, 15);
        notesLabel.TabIndex = 12;
        notesLabel.Text = "Notes";
        // 
        // notesText
        // 
        notesText.Location = new Point(12, 183);
        notesText.Multiline = true;
        notesText.Name = "notesText";
        notesText.Size = new Size(358, 60);
        notesText.TabIndex = 13;
        // 
        // isActiveCheck
        // 
        isActiveCheck.AutoSize = true;
        isActiveCheck.Checked = true;
        isActiveCheck.CheckState = CheckState.Checked;
        isActiveCheck.Location = new Point(12, 249);
        isActiveCheck.Name = "isActiveCheck";
        isActiveCheck.Size = new Size(59, 19);
        isActiveCheck.TabIndex = 14;
        isActiveCheck.Text = "Active";
        isActiveCheck.UseVisualStyleBackColor = true;
        // 
        // displayOrderLabel
        // 
        displayOrderLabel.AutoSize = true;
        displayOrderLabel.Location = new Point(150, 249);
        displayOrderLabel.Name = "displayOrderLabel";
        displayOrderLabel.Size = new Size(83, 15);
        displayOrderLabel.TabIndex = 15;
        displayOrderLabel.Text = "Display order";
        // 
        // displayOrderNumeric
        // 
        displayOrderNumeric.Location = new Point(239, 247);
        displayOrderNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        displayOrderNumeric.Name = "displayOrderNumeric";
        displayOrderNumeric.Size = new Size(60, 23);
        displayOrderNumeric.TabIndex = 16;
        // 
        // saveButton
        // 
        saveButton.Location = new Point(214, 290);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(75, 30);
        saveButton.TabIndex = 17;
        saveButton.Text = "Save";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += OnSaveClick;
        // 
        // cancelButton
        // 
        cancelButton.Location = new Point(295, 290);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 30);
        cancelButton.TabIndex = 18;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += OnCancelClick;
        // 
        // OfficeEditorForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(384, 332);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(displayOrderNumeric);
        Controls.Add(displayOrderLabel);
        Controls.Add(isActiveCheck);
        Controls.Add(notesText);
        Controls.Add(notesLabel);
        Controls.Add(headRankText);
        Controls.Add(headRankLabel);
        Controls.Add(headTitleText);
        Controls.Add(headTitleLabel);
        Controls.Add(headNameText);
        Controls.Add(headNameLabel);
        Controls.Add(phoneText);
        Controls.Add(phoneLabel);
        Controls.Add(nameText);
        Controls.Add(nameLabel);
        Controls.Add(codeText);
        Controls.Add(codeLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "OfficeEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Office";
        ((System.ComponentModel.ISupportInitialize)displayOrderNumeric).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label codeLabel;
    private TextBox codeText;
    private Label nameLabel;
    private TextBox nameText;
    private Label phoneLabel;
    private TextBox phoneText;
    private Label headNameLabel;
    private TextBox headNameText;
    private Label headTitleLabel;
    private TextBox headTitleText;
    private Label headRankLabel;
    private TextBox headRankText;
    private Label notesLabel;
    private TextBox notesText;
    private CheckBox isActiveCheck;
    private Label displayOrderLabel;
    private NumericUpDown displayOrderNumeric;
    private Button saveButton;
    private Button cancelButton;
}
