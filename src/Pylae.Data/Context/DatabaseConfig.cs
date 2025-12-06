using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace Pylae.Data.Context;

public static class DatabaseConfig
{
    static DatabaseConfig()
    {
        EnsureSqliteInitialized();
    }

    public static void EnsureDirectories(DatabaseOptions options)
    {
        var dataRoot = Path.Combine(options.RootPath, options.SiteCode);
        Directory.CreateDirectory(Path.Combine(dataRoot, "Data"));
        Directory.CreateDirectory(Path.Combine(dataRoot, "Photos"));
        Directory.CreateDirectory(Path.Combine(dataRoot, "Logs"));
    }

    public static string BuildConnectionString(string dbPath, string password)
    {
        // Validate password is set
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                $"Database encryption password is required but was not set. Path: {dbPath}");
        }

        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            ForeignKeys = true,
            Password = password,
            Cache = SqliteCacheMode.Private,
            Pooling = true  // Enable pooling to reuse connections and avoid repeated key derivation
        };

        return builder.ToString();
    }

    public static DbContextOptionsBuilder ConfigureMaster(DbContextOptionsBuilder optionsBuilder, string dbPath, string password)
    {
        return optionsBuilder.UseSqlite(BuildConnectionString(dbPath, password), options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory_master");
        });
    }

    public static DbContextOptionsBuilder ConfigureVisits(DbContextOptionsBuilder optionsBuilder, string dbPath, string password)
    {
        return optionsBuilder.UseSqlite(BuildConnectionString(dbPath, password), options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory_visits");
        });
    }

    private static void EnsureSqliteInitialized()
    {
        // Force the encryption-capable provider (e_sqlite3mc) so password= is supported
        SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_e_sqlite3mc());
        SQLitePCL.raw.FreezeProvider();
        SQLitePCL.Batteries_V2.Init();
    }
}
