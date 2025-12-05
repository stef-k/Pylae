using Pylae.Core.Security;
using Pylae.Desktop.Resources;
using System.Text.RegularExpressions;

namespace Pylae.Desktop.Forms;

public partial class FirstRunForm : Form
{
    public string SiteCode => siteCodeText.Text.Trim();
    public string SiteDisplayName => siteNameText.Text.Trim();
    public string AdminPassword => adminPasswordText.Text;
    public string PrimaryLanguage => languageCombo.Text.Trim();
    public string EncryptionPassword => encryptionPasswordText.Text;

    public FirstRunForm()
    {
        InitializeComponent();
        languageCombo.Items.AddRange(new[] { "el-GR", "en-US" });
        languageCombo.SelectedIndex = 0;
        Text = Strings.FirstRun_Title;
        siteCodeLabel.Text = Strings.FirstRun_SiteCode;
        siteCodeHint.Text = Strings.FirstRun_SiteCodeHint;
        siteNameLabel.Text = Strings.FirstRun_SiteName;
        siteNameHint.Text = Strings.FirstRun_SiteNameHint;
        adminPasswordLabel.Text = Strings.FirstRun_AdminPassword;
        adminHint.Text = Strings.FirstRun_AdminHint;
        encryptionPasswordLabel.Text = Strings.FirstRun_EncryptionPassword;
        encryptionHint.Text = Strings.FirstRun_EncryptionHint;
        languageLabel.Text = Strings.FirstRun_Language;
        saveButton.Text = Strings.FirstRun_Save;
        cancelButton.Text = Strings.FirstRun_Cancel;
    }

    private void OnSaveClick(object sender, EventArgs e)
    {
        var code = SiteCode.ToLowerInvariant();
        if (!Regex.IsMatch(code, "^[a-z0-9_-]{2,}$"))
        {
            MessageBox.Show(Strings.FirstRun_InvalidSiteCode, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        siteCodeText.Text = code;

        if (string.IsNullOrWhiteSpace(SiteDisplayName) ||
            string.IsNullOrWhiteSpace(AdminPassword) ||
            string.IsNullOrWhiteSpace(EncryptionPassword))
        {
            MessageBox.Show(Strings.FirstRun_RequiredFields, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!PasswordValidator.IsValid(AdminPassword, out var passwordError))
        {
            MessageBox.Show(passwordError, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!PasswordValidator.IsValid(EncryptionPassword, out var encryptionError))
        {
            MessageBox.Show(encryptionError, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void OnCancelClick(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
