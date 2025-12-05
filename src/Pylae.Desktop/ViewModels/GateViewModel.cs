using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.ViewModels;

public partial class GateViewModel : ObservableObject
{
    private readonly IVisitService _visitService;
    private readonly ISettingsService _settingsService;
    private readonly ILogger<GateViewModel>? _logger;

    [ObservableProperty]
    private string _memberNumberInput = string.Empty;

    [ObservableProperty]
    private VisitDirection _selectedDirection = VisitDirection.Entry;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private string? _lastResult;

    [ObservableProperty]
    private string? _badgeWarning;

    [ObservableProperty]
    private bool _isBusy;

    public BadgeStatus? LastBadgeStatus { get; private set; }

    public GateViewModel(IVisitService visitService, ISettingsService settingsService, ILogger<GateViewModel>? logger = null)
    {
        _visitService = visitService;
        _settingsService = settingsService;
        _logger = logger;
    }

    public async Task LogVisitAsync(User currentUser, string siteCode, string? workstationId = null)
    {
        if (IsBusy)
        {
            return;
        }

        LastBadgeStatus = null;
        BadgeWarning = null;

        if (!int.TryParse(MemberNumberInput, out var memberNumber))
        {
            LastResult = Strings.Gate_InvalidMemberNumber;
            return;
        }

        try
        {
            IsBusy = true;
            var (validityMonths, warningDays) = await LoadBadgeValidityAsync();

            var result = await _visitService.LogVisitAsync(
                memberNumber,
                SelectedDirection,
                VisitMethod.Manual,
                string.IsNullOrWhiteSpace(Notes) ? null : Notes,
                currentUser,
                siteCode,
                workstationId,
                validityMonths,
                warningDays);

            LastBadgeStatus = result.BadgeStatus;
            LastResult = string.Format(
                Strings.Gate_LogSuccess,
                result.Member.FirstName,
                result.Member.LastName,
                SelectedDirection == VisitDirection.Entry ? Strings.Gate_Mode_Entry : Strings.Gate_Mode_Exit);
            BadgeWarning = BuildBadgeWarning(result.BadgeStatus);
            Notes = string.Empty;
            MemberNumberInput = string.Empty;
        }
        catch (InvalidOperationException)
        {
            LastResult = Strings.Gate_MemberNotFound;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to log visit.");
            LastResult = string.Format(Strings.Gate_LogFailed, ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task<(int validityMonths, int warningDays)> LoadBadgeValidityAsync()
    {
        var settings = await _settingsService.GetAllAsync();
        var validityMonths = ParseIntSetting(settings, SettingKeys.BadgeValidityMonths, -1);
        var warningDays = ParseIntSetting(settings, SettingKeys.BadgeExpiryWarningDays, 0);
        return (validityMonths, warningDays);
    }

    private static int ParseIntSetting(IDictionary<string, string> settings, string key, int defaultValue)
    {
        if (settings.TryGetValue(key, out var value) && int.TryParse(value, out var parsed))
        {
            return parsed;
        }

        return defaultValue;
    }

    private static string? BuildBadgeWarning(BadgeStatus status)
    {
        if (status.IsExpired && status.ExpiryDate.HasValue)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Strings.Gate_BadgeExpired,
                status.ExpiryDate.Value.ToString("d", CultureInfo.CurrentCulture));
        }

        if (status.IsWarning && status.ExpiryDate.HasValue)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                Strings.Gate_BadgeExpires,
                status.ExpiryDate.Value.ToString("d", CultureInfo.CurrentCulture));
        }

        return null;
    }
}
