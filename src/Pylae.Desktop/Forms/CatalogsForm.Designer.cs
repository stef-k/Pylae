using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Pylae.Desktop.Resources;

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
        memberTypesGrid = new DataGridView();
        addMemberTypeButton = new Button();
        editMemberTypeButton = new Button();
        ((System.ComponentModel.ISupportInitialize)memberTypesGrid).BeginInit();
        SuspendLayout();
        //
        // memberTypesGrid - full height, full width
        //
        memberTypesGrid.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        memberTypesGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        memberTypesGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        memberTypesGrid.Location = new Point(12, 12);
        memberTypesGrid.Name = "memberTypesGrid";
        memberTypesGrid.ReadOnly = true;
        memberTypesGrid.RowTemplate.Height = 25;
        memberTypesGrid.Size = new Size(476, 350);
        memberTypesGrid.TabIndex = 0;
        //
        // addMemberTypeButton
        //
        addMemberTypeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        addMemberTypeButton.Location = new Point(12, 370);
        addMemberTypeButton.Name = "addMemberTypeButton";
        addMemberTypeButton.Size = new Size(140, 28);
        addMemberTypeButton.TabIndex = 1;
        addMemberTypeButton.Text = Strings.Button_AddMemberType;
        addMemberTypeButton.UseVisualStyleBackColor = true;
        addMemberTypeButton.Click += OnAddMemberTypeClick;
        //
        // editMemberTypeButton
        //
        editMemberTypeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        editMemberTypeButton.Location = new Point(158, 370);
        editMemberTypeButton.Name = "editMemberTypeButton";
        editMemberTypeButton.Size = new Size(140, 28);
        editMemberTypeButton.TabIndex = 2;
        editMemberTypeButton.Text = Strings.Button_EditMemberType;
        editMemberTypeButton.UseVisualStyleBackColor = true;
        editMemberTypeButton.Click += OnEditMemberTypeClick;
        //
        // CatalogsForm - Member Types only
        //
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(850, 410);
        Controls.Add(editMemberTypeButton);
        Controls.Add(addMemberTypeButton);
        memberTypesGrid.Dock = DockStyle.Fill;
        Controls.Add(memberTypesGrid);
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(850, 300);
        MaximizeBox = true;
        MinimizeBox = false;
        Name = "CatalogsForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = Strings.Catalogs_Title;
        Icon = new Icon(Path.Combine(AppContext.BaseDirectory, "Resources", "Images", "pylae_icon.ico"));
        ((System.ComponentModel.ISupportInitialize)memberTypesGrid).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView memberTypesGrid;
    private Button addMemberTypeButton;
    private Button editMemberTypeButton;
}
