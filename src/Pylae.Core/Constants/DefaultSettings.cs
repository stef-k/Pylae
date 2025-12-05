namespace Pylae.Core.Constants;

public static class DefaultSettings
{
    public static IReadOnlyDictionary<string, string> All { get; } = new Dictionary<string, string>
    {
        { SettingKeys.SiteCode, "default" },
        { SettingKeys.SiteDisplayName, "Default Site" },
        { SettingKeys.PrimaryLanguage, "el-GR" },
        { SettingKeys.NetworkEnabled, "0" },
        { SettingKeys.NetworkPort, "8080" },
        { SettingKeys.NetworkApiKey, string.Empty },
        { SettingKeys.IdleTimeoutMinutes, "0" },
        { SettingKeys.LogLevel, "Information" },
        { SettingKeys.LogFileMaxSizeMB, "10" },
        { SettingKeys.LogRetentionDays, "30" },
        { SettingKeys.HealthLoggingEnabled, "1" },
        { SettingKeys.BadgeValidityMonths, "-1" },
        { SettingKeys.BadgeExpiryWarningDays, "30" },
        { SettingKeys.OrgBusinessTitle, string.Empty },
        { SettingKeys.OrgBusinessTel, string.Empty },
        { SettingKeys.AutoBackupEnabled, "0" },
        { SettingKeys.AutoBackupIntervalHours, "24" },
        { SettingKeys.AutoBackupRetentionCount, "7" },
        { SettingKeys.AutoBackupPath, string.Empty },
        { SettingKeys.AutoBackupIncludePhotos, "1" },
        { SettingKeys.AuditRetentionYears, "3" },
        { SettingKeys.VisitRetentionYears, "3" }
    };
}
