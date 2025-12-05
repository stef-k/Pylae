namespace Pylae.Core.Constants;

public static class AuditActionTypes
{
    public const string Login = "Login";
    public const string Logout = "Logout";

    public const string MemberCreated = "MemberCreated";
    public const string MemberUpdated = "MemberUpdated";
    public const string MemberDeleted = "MemberDeleted";
    public const string MemberDeactivated = "MemberDeactivated";

    public const string OfficeCreated = "OfficeCreated";
    public const string OfficeUpdated = "OfficeUpdated";

    public const string MemberTypeCreated = "MemberTypeCreated";
    public const string MemberTypeUpdated = "MemberTypeUpdated";

    public const string UserCreated = "UserCreated";
    public const string UserUpdated = "UserUpdated";
    public const string UserDeleted = "UserDeleted";

    public const string SettingsChanged = "SettingsChanged";
    public const string NetworkSettingsChanged = "NetworkSettingsChanged";

    public const string VisitLogged = "VisitLogged";
    public const string VisitNoteUpdated = "VisitNoteUpdated";
    public const string UseExpiredBadge = "UseExpiredBadge";

    public const string DatabaseExported = "DatabaseExported";
    public const string DatabaseImported = "DatabaseImported";

    public const string SyncPull = "SyncPull";
    public const string SyncPush = "SyncPush";
}
