using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IAppSettings _appSettings;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private string _subtitle = Strings.App_Subtitle;

    [ObservableProperty]
    private string _siteCode = "default";

    [ObservableProperty]
    private string? _siteDisplayName;

    public MainViewModel(IAppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Settings are already loaded in AppSettingsService during app warmup
        SiteCode = _appSettings.GetValue(SettingKeys.SiteCode, "default");
        SiteDisplayName = _appSettings.GetValue(SettingKeys.SiteDisplayName);

        var culture = _appSettings.GetValue(SettingKeys.PrimaryLanguage);
        if (!string.IsNullOrEmpty(culture))
        {
            TryApplyCulture(culture);
        }

        return Task.CompletedTask;
    }

    private static void TryApplyCulture(string cultureName)
    {
        try
        {
            var culture = new CultureInfo(cultureName);
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            Strings.Culture = culture;
        }
        catch (CultureNotFoundException)
        {
            Strings.Culture = CultureInfo.CurrentUICulture;
        }
    }
}
