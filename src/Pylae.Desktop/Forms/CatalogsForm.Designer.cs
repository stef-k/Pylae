using System.Drawing;
using System.Windows.Forms;

namespace Pylae.Desktop.Forms;

partial class CatalogsForm
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
        officesGrid = new DataGridView();
        memberTypesGrid = new DataGridView();
        addOfficeButton = new Button();
        editOfficeButton = new Button();
        addMemberTypeButton = new Button();
        editMemberTypeButton = new Button();
        ((System.ComponentModel.ISupportInitialize)officesGrid).BeginInit();
        ((System.ComponentModel.ISupportInitialize)memberTypesGrid).BeginInit();
        SuspendLayout();
        // 
        // officesGrid
        // 
        officesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
        officesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        officesGrid.Location = new Point(12, 12);
        officesGrid.Name = "officesGrid";
        officesGrid.ReadOnly = true;
        officesGrid.RowTemplate.Height = 25;
        officesGrid.Size = new Size(360, 300);
        officesGrid.TabIndex = 0;
        // 
        // memberTypesGrid
        // 
        memberTypesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        memberTypesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        memberTypesGrid.Location = new Point(388, 12);
        memberTypesGrid.Name = "memberTypesGrid";
        memberTypesGrid.ReadOnly = true;
        memberTypesGrid.RowTemplate.Height = 25;
        memberTypesGrid.Size = new Size(360, 300);
        memberTypesGrid.TabIndex = 1;
        // 
        // addOfficeButton
        // 
        addOfficeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        addOfficeButton.Location = new Point(12, 318);
        addOfficeButton.Name = "addOfficeButton";
        addOfficeButton.Size = new Size(90, 30);
        addOfficeButton.TabIndex = 2;
        addOfficeButton.Text = "Add Office";
        addOfficeButton.UseVisualStyleBackColor = true;
        addOfficeButton.Click += OnAddOfficeClick;
        // 
        // editOfficeButton
        // 
        editOfficeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        editOfficeButton.Location = new Point(108, 318);
        editOfficeButton.Name = "editOfficeButton";
        editOfficeButton.Size = new Size(90, 30);
        editOfficeButton.TabIndex = 3;
        editOfficeButton.Text = "Edit Office";
        editOfficeButton.UseVisualStyleBackColor = true;
        editOfficeButton.Click += OnEditOfficeClick;
        // 
        // addMemberTypeButton
        // 
        addMemberTypeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        addMemberTypeButton.Location = new Point(476, 318);
        addMemberTypeButton.Name = "addMemberTypeButton";
        addMemberTypeButton.Size = new Size(120, 30);
        addMemberTypeButton.TabIndex = 4;
        addMemberTypeButton.Text = "Add Member Type";
        addMemberTypeButton.UseVisualStyleBackColor = true;
        addMemberTypeButton.Click += OnAddMemberTypeClick;
        // 
        // editMemberTypeButton
        // 
        editMemberTypeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        editMemberTypeButton.Location = new Point(602, 318);
        editMemberTypeButton.Name = "editMemberTypeButton";
        editMemberTypeButton.Size = new Size(146, 30);
        editMemberTypeButton.TabIndex = 5;
        editMemberTypeButton.Text = "Edit Member Type";
        editMemberTypeButton.UseVisualStyleBackColor = true;
        editMemberTypeButton.Click += OnEditMemberTypeClick;
        // 
        // CatalogsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(760, 360);
        Controls.Add(editMemberTypeButton);
        Controls.Add(addMemberTypeButton);
        Controls.Add(editOfficeButton);
        Controls.Add(addOfficeButton);
        Controls.Add(memberTypesGrid);
        Controls.Add(officesGrid);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "CatalogsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Catalogs";
        ((System.ComponentModel.ISupportInitialize)officesGrid).EndInit();
        ((System.ComponentModel.ISupportInitialize)memberTypesGrid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView officesGrid;
    private DataGridView memberTypesGrid;
    private Button addOfficeButton;
    private Button editOfficeButton;
    private Button addMemberTypeButton;
    private Button editMemberTypeButton;
}
