using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using Pylae.Data.Context;
using Pylae.Desktop.Resources;

namespace Pylae.Desktop.Services;

public interface IBackupService
{
    Task<byte[]> CreateBackupAsync(bool includePhotos, CancellationToken cancellationToken = default);

    Task CreateBackupAsync(string destinationPath, bool includePhotos, CancellationToken cancellationToken = default);

    Task<BackupRestoreResult> RestoreBackupAsync(string archivePath, CancellationToken cancellationToken = default);
}

public record BackupRestoreResult(bool IsValid, string? Reason);

public class BackupService : IBackupService
{
    private readonly DatabaseOptions _options;

    public BackupService(DatabaseOptions options)
    {
        _options = options;
    }

    public async Task<byte[]> CreateBackupAsync(bool includePhotos, CancellationToken cancellationToken = default)
    {
        // Create temporary directory for backup files
        var tempDir = Path.Combine(Path.GetTempPath(), $"pylae_backup_{Guid.NewGuid():N}");
        Directory.CreateDirectory(tempDir);

        try
        {
            var tempMasterPath = Path.Combine(tempDir, "master.db");
            var tempVisitsPath = Path.Combine(tempDir, "visits.db");

            // Use SQLite online backup API to safely backup open databases
            await BackupDatabaseSafelyAsync(_options.GetMasterDbPath(), tempMasterPath, _options.EncryptionPassword, cancellationToken);
            await BackupDatabaseSafelyAsync(_options.GetVisitsDbPath(), tempVisitsPath, _options.EncryptionPassword, cancellationToken);

            // Create ZIP archive with backed up files
            await using var buffer = new MemoryStream();
            using (var zip = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true))
            {
                AddFileToArchive(zip, tempMasterPath, Path.Combine("Data", "master.db"));
                AddFileToArchive(zip, tempVisitsPath, Path.Combine("Data", "visits.db"));
                AddManifest(zip, tempMasterPath, tempVisitsPath);

                if (includePhotos && Directory.Exists(_options.GetPhotosPath()))
                {
                    foreach (var photo in Directory.GetFiles(_options.GetPhotosPath()))
                    {
                        var name = Path.GetFileName(photo);
                        zip.CreateEntryFromFile(photo, Path.Combine("Photos", name), CompressionLevel.Optimal);
                    }
                }
            }

            return buffer.ToArray();
        }
        finally
        {
            // Clean up temporary directory
            try
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, recursive: true);
                }
            }
            catch
            {
                // Ignore cleanup failures
            }
        }
    }

    /// <summary>
    /// Safely backs up a SQLite database using the online backup API.
    /// This works even if the database is currently open and being written to.
    /// </summary>
    private static async Task BackupDatabaseSafelyAsync(string sourcePath, string destinationPath, string password, CancellationToken cancellationToken)
    {
        if (!File.Exists(sourcePath))
        {
            return;
        }

        // Use SQLite online backup API
        var sourceConnStr = new SqliteConnectionStringBuilder
        {
            DataSource = sourcePath,
            Mode = SqliteOpenMode.ReadOnly,
            Password = password
        }.ToString();

        var destConnStr = new SqliteConnectionStringBuilder
        {
            DataSource = destinationPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Password = password
        }.ToString();

        await using var sourceConn = new SqliteConnection(sourceConnStr);
        await using var destConn = new SqliteConnection(destConnStr);

        await sourceConn.OpenAsync(cancellationToken);
        await destConn.OpenAsync(cancellationToken);

        // Perform online backup (thread-safe, consistent snapshot)
        sourceConn.BackupDatabase(destConn);
    }

    private static void AddFileToArchive(ZipArchive archive, string path, string entryName)
    {
        if (!File.Exists(path))
        {
            return;
        }

        archive.CreateEntryFromFile(path, entryName, CompressionLevel.Fastest);
    }

    public async Task CreateBackupAsync(string destinationPath, bool includePhotos, CancellationToken cancellationToken = default)
    {
        var backupData = await CreateBackupAsync(includePhotos, cancellationToken);
        await File.WriteAllBytesAsync(destinationPath, backupData, cancellationToken);
    }

    public async Task<BackupRestoreResult> RestoreBackupAsync(string archivePath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_options.GetMasterDbPath())!);
        Directory.CreateDirectory(Path.GetDirectoryName(_options.GetVisitsDbPath())!);
        Directory.CreateDirectory(_options.GetPhotosPath());

        await using var stream = File.OpenRead(archivePath);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        var manifest = ReadManifest(zip);

        foreach (var entry in zip.Entries)
        {
            if (entry.FullName.StartsWith("Data/", StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.StartsWith("Data\\", StringComparison.OrdinalIgnoreCase))
            {
                var destination = entry.Name.Equals("master.db", StringComparison.OrdinalIgnoreCase)
                    ? _options.GetMasterDbPath()
                    : _options.GetVisitsDbPath();

                await using var entryStream = entry.Open();
                await using var destinationStream = File.Create(destination);
                await entryStream.CopyToAsync(destinationStream, cancellationToken);
            }
            else if (entry.FullName.StartsWith("Photos/", StringComparison.OrdinalIgnoreCase) ||
                     entry.FullName.StartsWith("Photos\\", StringComparison.OrdinalIgnoreCase))
            {
                var target = Path.Combine(_options.GetPhotosPath(), Path.GetFileName(entry.FullName));
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);

                await using var entryStream = entry.Open();
                await using var destinationStream = File.Create(target);
                await entryStream.CopyToAsync(destinationStream, cancellationToken);
            }
            else if (entry.FullName.Equals("manifest.txt", StringComparison.OrdinalIgnoreCase))
            {
                // skip, used only for verification
            }
        }

        var verify = await VerifyAsync(manifest, cancellationToken);
        return verify;
    }

    private void AddManifest(ZipArchive archive, string tempMasterPath, string tempVisitsPath)
    {
        var manifest = $"created:{DateTime.UtcNow:o}\n" +
                       $"master:{ComputeHash(tempMasterPath)}\n" +
                       $"visits:{ComputeHash(tempVisitsPath)}\n";
        var entry = archive.CreateEntry("manifest.txt");
        using var writer = new StreamWriter(entry.Open());
        writer.Write(manifest);
    }

    private string ComputeHash(string path)
    {
        if (!File.Exists(path))
        {
            return string.Empty;
        }

        using var sha = SHA256.Create();
        using var stream = File.OpenRead(path);
        var hash = sha.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }

    private static Dictionary<string, string>? ReadManifest(ZipArchive zip)
    {
        var entry = zip.GetEntry("manifest.txt");
        if (entry is null)
        {
            return null;
        }

        using var reader = new StreamReader(entry.Open());
        var content = reader.ReadToEnd();
        var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var line in lines)
        {
            var parts = line.Split(':', 2);
            if (parts.Length == 2)
            {
                map[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return map;
    }

    private async Task<BackupRestoreResult> VerifyAsync(IDictionary<string, string>? manifest, CancellationToken cancellationToken)
    {
        if (!File.Exists(_options.GetMasterDbPath()) || !File.Exists(_options.GetVisitsDbPath()))
        {
            return new BackupRestoreResult(false, Strings.Backup_ReasonMissingDb);
        }

        // Manifest is optional; verify presence of DB files and non-empty
        var masterInfo = new FileInfo(_options.GetMasterDbPath());
        var visitsInfo = new FileInfo(_options.GetVisitsDbPath());
        if (masterInfo.Length == 0 || visitsInfo.Length == 0)
        {
            return new BackupRestoreResult(false, Strings.Backup_ReasonEmptyDb);
        }

        var masterHash = ComputeHash(_options.GetMasterDbPath());
        var visitsHash = ComputeHash(_options.GetVisitsDbPath());

        if (manifest is null || !manifest.ContainsKey("master") || !manifest.ContainsKey("visits"))
        {
            return new BackupRestoreResult(false, Strings.Backup_ReasonNoManifest);
        }

        var masterMatches = string.Equals(masterHash, manifest["master"], StringComparison.OrdinalIgnoreCase);
        var visitsMatches = string.Equals(visitsHash, manifest["visits"], StringComparison.OrdinalIgnoreCase);

        await Task.CompletedTask;
        return masterMatches && visitsMatches
            ? new BackupRestoreResult(true, null)
            : new BackupRestoreResult(false, Strings.Backup_ReasonChecksumMismatch);
    }
}
