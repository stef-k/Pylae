namespace Pylae.Data.Context;

public class DatabaseOptions
{
    public string RootPath { get; set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Pylae");

    public string SiteCode { get; set; } = "default";

    public string SiteDisplayName { get; set; } = "Default Site";

    public string EncryptionPassword { get; set; } = string.Empty;

    public string GetMasterDbPath() => Path.Combine(RootPath, SiteCode, "Data", "master.db");

    public string GetVisitsDbPath() => Path.Combine(RootPath, SiteCode, "Data", "visits.db");

    public string GetPhotosPath() => Path.Combine(RootPath, SiteCode, "Photos");

    public string GetLogsPath() => Path.Combine(RootPath, SiteCode, "Logs");
}
