using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;

namespace Pylae.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly ISettingsService _settingsService;

    [ObservableProperty]
    private List<Setting> _settings = new();

    public SettingsViewModel(ISettingsService settingsService)
    {
        _settingsService = settingsService;
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
    }
}
