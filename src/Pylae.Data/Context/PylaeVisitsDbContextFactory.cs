using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Pylae.Data.Context;

public class PylaeVisitsDbContextFactory : IDesignTimeDbContextFactory<PylaeVisitsDbContext>
{
    public PylaeVisitsDbContext CreateDbContext(string[] args)
    {
        var dbOptions = BuildDatabaseOptions();
        DatabaseConfig.EnsureDirectories(dbOptions);

        var builder = new DbContextOptionsBuilder<PylaeVisitsDbContext>();
        DatabaseConfig.ConfigureVisits(builder, dbOptions.GetVisitsDbPath(), dbOptions.EncryptionPassword);
        return new PylaeVisitsDbContext(builder.Options);
    }

    private static DatabaseOptions BuildDatabaseOptions()
    {
        var password = Environment.GetEnvironmentVariable("PYLAE_DB_PASSWORD") ?? "ChangeMe!";
        var siteCode = Environment.GetEnvironmentVariable("PYLAE_SITE_CODE") ?? "design";
        var rootPath = Environment.GetEnvironmentVariable("PYLAE_DATA_ROOT");

        return new DatabaseOptions
        {
            SiteCode = siteCode,
            EncryptionPassword = password,
            RootPath = string.IsNullOrWhiteSpace(rootPath)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Pylae")
                : rootPath
        };
    }
}
