namespace Pylae.Core.Constants;

public static class SettingKeys
{
    // Identity & Locale
    public const string SiteCode = "SiteCode";
    public const string SiteDisplayName = "SiteDisplayName";
    public const string PrimaryLanguage = "PrimaryLanguage";

    // Network
    public const string NetworkEnabled = "NetworkEnabled";
    public const string NetworkPort = "NetworkPort";
    public const string NetworkApiKey = "NetworkApiKey";

    // Security & Session
    public const string IdleTimeoutMinutes = "IdleTimeoutMinutes";

    // Logging
    public const string LogLevel = "LogLevel";
    public const string LogFileMaxSizeMB = "LogFileMaxSizeMB";
    public const string LogRetentionDays = "LogRetentionDays";
    public const string HealthLoggingEnabled = "HealthLoggingEnabled";

    // Badge Validity
    public const string BadgeValidityMonths = "BadgeValidityMonths";
    public const string BadgeExpiryWarningDays = "BadgeExpiryWarningDays";

    // Organization identity
    public const string OrgBusinessTitle = "OrgBusinessTitle";
    public const string OrgBusinessTel = "OrgBusinessTel";

    // Automated Backups
    public const string AutoBackupEnabled = "AutoBackupEnabled";
    public const string AutoBackupIntervalHours = "AutoBackupIntervalHours";
    public const string AutoBackupRetentionCount = "AutoBackupRetentionCount";
    public const string AutoBackupPath = "AutoBackupPath";
    public const string AutoBackupIncludePhotos = "AutoBackupIncludePhotos";

    // Data Retention
    public const string AuditRetentionYears = "AuditRetentionYears";
    public const string VisitRetentionYears = "VisitRetentionYears";

    // Last Execution Timestamps (for catch-up logic)
    public const string LastBackupTime = "LastBackupTime";
    public const string LastAuditCleanupTime = "LastAuditCleanupTime";
    public const string LastVisitCleanupTime = "LastVisitCleanupTime";
}
