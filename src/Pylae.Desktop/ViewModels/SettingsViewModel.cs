using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;
    private readonly IAppSettings _appSettings;

    [ObservableProperty]
    private List<Setting> _settings = new();

    public SettingsViewModel(ISettingsService settingsService, IAppSettings appSettings)
    {
        _settingsService = settingsService;
        _appSettings = appSettings;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var values = await _settingsService.GetAllAsync();
        Settings = values.Select(kv => new Setting { Key = kv.Key, Value = kv.Value }).OrderBy(s => s.Key).ToList();
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        await _settingsService.UpsertAsync(Settings, CancellationToken.None);
        await _appSettings.RefreshAsync();
    }
}
