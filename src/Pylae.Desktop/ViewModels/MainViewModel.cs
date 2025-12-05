using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private string _subtitle = Strings.App_Subtitle;

    [ObservableProperty]
    private string _siteCode = "default";

    [ObservableProperty]
    private string? _siteDisplayName;

    public MainViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var settings = await _settingsService.GetAllAsync(cancellationToken);

        if (settings.TryGetValue(SettingKeys.SiteCode, out var siteCodeValue))
        {
            SiteCode = siteCodeValue;
        }

        if (settings.TryGetValue(SettingKeys.SiteDisplayName, out var siteDisplay))
        {
            SiteDisplayName = siteDisplay;
        }

        if (settings.TryGetValue(SettingKeys.PrimaryLanguage, out var culture))
        {
            TryApplyCulture(culture);
        }
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
