using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class MemberTypeEditorForm
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
        nameLabel = new Label();
        nameText = new TextBox();
        descriptionLabel = new Label();
        descriptionText = new TextBox();
        displayOrderLabel = new Label();
        displayOrderNumeric = new NumericUpDown();
        isActiveCheck = new CheckBox();
        saveButton = new Button();
        cancelButton = new Button();
        ((System.ComponentModel.ISupportInitialize)displayOrderNumeric).BeginInit();
        SuspendLayout();
        //
        // nameLabel
        //
        nameLabel.AutoSize = true;
        nameLabel.Location = new Point(12, 15);
        nameLabel.Name = "nameLabel";
        nameLabel.Size = new Size(39, 15);
        nameLabel.TabIndex = 0;
        nameLabel.Text = "Name";
        //
        // nameText
        //
        nameText.Location = new Point(12, 33);
        nameText.Name = "nameText";
        nameText.Size = new Size(518, 23);
        nameText.TabIndex = 1;
        // 
        // descriptionLabel
        // 
        descriptionLabel.AutoSize = true;
        descriptionLabel.Location = new Point(12, 65);
        descriptionLabel.Name = "descriptionLabel";
        descriptionLabel.Size = new Size(67, 15);
        descriptionLabel.TabIndex = 4;
        descriptionLabel.Text = "Description";
        //
        // descriptionText
        //
        descriptionText.Location = new Point(12, 83);
        descriptionText.Multiline = true;
        descriptionText.Name = "descriptionText";
        descriptionText.Size = new Size(518, 80);
        descriptionText.TabIndex = 5;
        // 
        // displayOrderLabel
        // 
        displayOrderLabel.AutoSize = true;
        displayOrderLabel.Location = new Point(12, 175);
        displayOrderLabel.Name = "displayOrderLabel";
        displayOrderLabel.Size = new Size(83, 15);
        displayOrderLabel.TabIndex = 6;
        displayOrderLabel.Text = "Display order";
        // 
        // displayOrderNumeric
        // 
        displayOrderNumeric.Location = new Point(101, 173);
        displayOrderNumeric.Maximum = new decimal(new int[] { 1000, 0, 0, 0 });
        displayOrderNumeric.Name = "displayOrderNumeric";
        displayOrderNumeric.Size = new Size(60, 23);
        displayOrderNumeric.TabIndex = 7;
        // 
        // isActiveCheck
        // 
        isActiveCheck.AutoSize = true;
        isActiveCheck.Checked = true;
        isActiveCheck.CheckState = CheckState.Checked;
        isActiveCheck.Location = new Point(180, 174);
        isActiveCheck.Name = "isActiveCheck";
        isActiveCheck.Size = new Size(59, 19);
        isActiveCheck.TabIndex = 8;
        isActiveCheck.Text = "Active";
        isActiveCheck.UseVisualStyleBackColor = true;
        //
        // saveButton
        //
        saveButton.ForeColor = Color.Blue;
        saveButton.Location = new Point(374, 210);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(75, 30);
        saveButton.TabIndex = 9;
        saveButton.Text = "Save";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += OnSaveClick;
        //
        // cancelButton
        //
        cancelButton.Location = new Point(455, 210);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(75, 30);
        cancelButton.TabIndex = 10;
        cancelButton.Text = "Cancel";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += OnCancelClick;
        //
        // MemberTypeEditorForm
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(542, 252);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(isActiveCheck);
        Controls.Add(displayOrderNumeric);
        Controls.Add(displayOrderLabel);
        Controls.Add(descriptionText);
        Controls.Add(descriptionLabel);
        Controls.Add(nameText);
        Controls.Add(nameLabel);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MemberTypeEditorForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Member Type";
        Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "pylae_icon.ico"));
        ((System.ComponentModel.ISupportInitialize)displayOrderNumeric).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label nameLabel;
    private TextBox nameText;
    private Label descriptionLabel;
    private TextBox descriptionText;
    private Label displayOrderLabel;
    private NumericUpDown displayOrderNumeric;
    private CheckBox isActiveCheck;
    private Button saveButton;
    private Button cancelButton;
}
