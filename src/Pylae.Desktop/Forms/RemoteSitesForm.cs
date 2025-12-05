using Pylae.Core.Models;
using Pylae.Data.Context;
using Pylae.Desktop.Resources;
using Pylae.Sync.Client;
using Pylae.Sync.Models;
using System.IO.Compression;
using System.IO;
using System.Windows.Forms;
using Pylae.Desktop.Services;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;

namespace Pylae.Desktop.Forms;

public partial class RemoteSitesForm : Form
{
    private readonly IRemoteSiteClient _client;
    private readonly DatabaseOptions _options;
    private readonly MasterMergeService _mergeService;
    private readonly SyncHistoryService _history;
    private readonly IAuditService _auditService;
    private readonly ISettingsService _settingsService;

    public RemoteSitesForm(IRemoteSiteClient client, DatabaseOptions options, MasterMergeService mergeService, SyncHistoryService history, IAuditService auditService, ISettingsService settingsService)
    {
        _client = client;
        _options = options;
        _mergeService = mergeService;
        _history = history;
        _auditService = auditService;
        _settingsService = settingsService;
        InitializeComponent();
        pushMasterButton.Text = Strings.Sync_PushMaster;
        downloadVisitsButton.Text = Strings.Sync_PullVisits;
        includePhotosCheckBox.Text = Strings.Sync_IncludePhotos;
        pullMasterButton.Text = Strings.Sync_PullMaster;
        statusLabel.Text = Strings.Sync_Status;
        Text = Strings.Button_RemoteSites;
        recentEventsLabel.Text = Strings.Sync_RecentEvents;
        LoadRecentHistory();
    }

    private async void OnFetchInfoClick(object sender, EventArgs e)
    {
        var config = BuildConfig();
        var info = await _client.GetInfoAsync(config);
        if (info is null)
        {
            MessageBox.Show(Strings.Sync_DownloadFailed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        infoTextBox.Text = string.Format(
            Strings.Sync_InfoDetails,
            info.SiteCode,
            info.SiteDisplayName,
            info.MemberCount,
            info.VisitsCount,
            info.LastVisitTimestampUtc?.ToString("g") ?? "-");
        AppendStatus(Strings.Sync_InfoReceived);
        _history.Add("pull-info", Strings.Sync_InfoReceived);
        await LogAuditAsync(Strings.Sync_InfoReceived, "info", info.SiteCode);
    }

    private async void OnFetchVisitsClick(object sender, EventArgs e)
    {
        var config = BuildConfig();
        var visits = await _client.GetVisitsAsync(config, fromPicker.Value, toPicker.Value);
        visitsCountLabel.Text = string.Format(Strings.Sync_VisitsCountLabel, visits.Count);
        AppendStatus(string.Format(Strings.Sync_VisitsReceivedCount, visits.Count));
        _history.Add("pull-visits", string.Format(Strings.Sync_VisitsReceivedCount, visits.Count));
        await LogAuditAsync(string.Format(Strings.Sync_VisitsReceivedCount, visits.Count), "visits-range", config.SiteCode);
    }

    private async void OnPushMasterClick(object sender, EventArgs e)
    {
        try
        {
            var config = BuildConfig();
            var package = await BuildMasterPackageAsync(includePhotosCheckBox.Checked);
        var ok = await _client.UploadMasterPackageAsync(config, package);
        MessageBox.Show(ok ? Strings.Sync_PushSuccess : Strings.Sync_PushFailed, Strings.App_Title, MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        AppendStatus(ok ? Strings.Sync_PushSuccess : Strings.Sync_PushFailed);
        _history.Add("push-master", ok ? Strings.Sync_PushSuccess : Strings.Sync_PushFailed);
        if (ok)
        {
            await LogAuditAsync(Strings.Sync_PushSuccess, "master-push", config.SiteCode);
        }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"{Strings.Sync_PushFailed}\n{ex.Message}", Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void OnDownloadVisitsClick(object sender, EventArgs e)
    {
        var config = BuildConfig();
        var bytes = await _client.GetFullVisitsDatabaseAsync(config);
        if (bytes is null)
        {
            MessageBox.Show(Strings.Sync_DownloadFailed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var destination = _options.GetVisitsDbPath();
        if (MessageBox.Show(Strings.Sync_OverwriteVisitsConfirm, Strings.App_Title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        await File.WriteAllBytesAsync(destination, bytes);
        MessageBox.Show(Strings.Sync_VisitsImported, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        AppendStatus(Strings.Sync_VisitsImported);
        _history.Add("pull-visits-db", Strings.Sync_VisitsImported);
        await LogAuditAsync(Strings.Sync_VisitsImported, "visits-db", config.SiteCode);
    }

    private RemoteSiteConfig BuildConfig()
    {
        return new RemoteSiteConfig
        {
            Host = hostTextBox.Text.Trim(),
            Port = (int)portNumeric.Value,
            ApiKey = apiKeyTextBox.Text.Trim()
        };
    }

    private async Task<MasterPackage> BuildMasterPackageAsync(bool includePhotos)
    {
        var masterPath = _options.GetMasterDbPath();
        if (!File.Exists(masterPath))
        {
            throw new FileNotFoundException(Strings.Sync_MasterMissing, masterPath);
        }

        var package = new MasterPackage
        {
            SiteCode = _options.SiteCode,
            MasterDatabase = await File.ReadAllBytesAsync(masterPath)
        };

        if (includePhotos)
        {
            package.PhotosArchive = await BuildPhotosArchiveAsync();
        }

        return package;
    }

    private async Task<byte[]?> BuildPhotosArchiveAsync()
    {
        var photosPath = _options.GetPhotosPath();
        if (!Directory.Exists(photosPath))
        {
            return null;
        }

        await using var buffer = new MemoryStream();
        using var zip = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true);
        foreach (var file in Directory.GetFiles(photosPath))
        {
            var name = Path.GetFileName(file);
            zip.CreateEntryFromFile(file, Path.Combine("Photos", name));
        }

        return buffer.ToArray();
    }

    private async void OnPullMasterClick(object sender, EventArgs e)
    {
        var config = BuildConfig();
        var package = await _client.GetMasterPackageAsync(config);
        if (package is null || package.MasterDatabase.Length == 0)
        {
            MessageBox.Show(Strings.Sync_DownloadFailed, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var choice = MessageBox.Show(Strings.Sync_PullConfirm, Strings.App_Title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        if (choice == DialogResult.Cancel)
        {
            return;
        }

        var merge = choice == DialogResult.Yes;
        if (merge)
        {
            var result = await _mergeService.MergeAsync(package.MasterDatabase);
            var summary = string.Format(Strings.Sync_MergeSummary, result.Offices.Added, result.Offices.Updated, result.MemberTypes.Added, result.MemberTypes.Updated, result.Members.Added, result.Members.Updated);
            _history.Add("merge-master", summary);
            AppendStatus(summary);

            if (result.HasConflicts)
            {
                var conflicts = string.Format(Strings.Sync_MergeConflicts, result.Offices.Skipped, result.MemberTypes.Skipped, result.Members.Skipped);
                AppendStatus(conflicts);
                var details = BuildConflictDetails(result);
                if (!string.IsNullOrWhiteSpace(details))
                {
                    AppendStatus(details);
                }
                await LogAuditAsync($"{summary} {conflicts} {details}".Trim(), "master-merge", config.SiteCode);
            }
            else
            {
                await LogAuditAsync(summary, "master-merge", config.SiteCode);
            }
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_options.GetMasterDbPath())!);
            await File.WriteAllBytesAsync(_options.GetMasterDbPath(), package.MasterDatabase);
            _history.Add("pull-master-overwrite", Strings.Sync_MasterOverwritten);
            await LogAuditAsync(Strings.Sync_MasterOverwritten, "master-overwrite", config.SiteCode);
        }

        if (package.PhotosArchive is { Length: > 0 })
        {
            await ExtractPhotosAsync(package.PhotosArchive, _options.GetPhotosPath());
        }

        MessageBox.Show(Strings.Sync_MasterImported, Strings.App_Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        AppendStatus(Strings.Sync_MasterImported);
        _history.Add("pull-master", Strings.Sync_MasterImported);
        await LogAuditAsync(Strings.Sync_MasterImported, "master-pull", config.SiteCode);
    }

    private static string? BuildConflictDetails(MergeResult result)
    {
        var parts = new List<string>();

        void AddDetails(string label, List<string> items)
        {
            if (items.Count == 0)
            {
                return;
            }

            var preview = string.Join(", ", items.Take(5));
            if (items.Count > 5)
            {
                preview += "...";
            }

            parts.Add(string.Format(label, preview));
        }

        AddDetails(Strings.Sync_MergeConflictsOffices, result.Offices.SkippedItems);
        AddDetails(Strings.Sync_MergeConflictsTypes, result.MemberTypes.SkippedItems);
        AddDetails(Strings.Sync_MergeConflictsMembers, result.Members.SkippedItems);

        return parts.Count == 0 ? null : string.Join(" | ", parts);
    }

    private static async Task ExtractPhotosAsync(byte[] archiveBytes, string destinationFolder)
    {
        Directory.CreateDirectory(destinationFolder);
        await using var stream = new MemoryStream(archiveBytes);
        using var zip = new ZipArchive(stream, ZipArchiveMode.Read);
        var incoming = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in zip.Entries)
        {
            var targetPath = Path.Combine(destinationFolder, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath)!);
            await using var entryStream = entry.Open();
            await using var fileStream = File.Create(targetPath);
            await entryStream.CopyToAsync(fileStream);
            incoming.Add(Path.GetFileName(entry.FullName));
        }

        foreach (var file in Directory.GetFiles(destinationFolder))
        {
            var name = Path.GetFileName(file);
            if (!incoming.Contains(name))
            {
                try
                {
                    File.Delete(file);
                }
                catch
                {
                    // ignore
                }
            }
        }
    }

    private void AppendStatus(string message)
    {
        statusTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
    }

    private void LoadRecentHistory()
    {
        var items = _history.GetRecent()
            .OrderByDescending(e => e.Timestamp)
            .Take(20)
            .Select(e => $"[{e.Timestamp:MM/dd HH:mm}] {e.Direction}: {e.Message}")
            .ToList();
        recentEventsList.Items.Clear();
        foreach (var item in items)
        {
            recentEventsList.Items.Add(item);
        }
    }

    private async Task LogAuditAsync(string message, string action, string? siteCode)
    {
        var settings = await _settingsService.GetAllAsync();
        var localSite = settings.TryGetValue(SettingKeys.SiteCode, out var sc) ? sc : _options.SiteCode;

        await _auditService.LogAsync(new AuditEntry
        {
            TimestampUtc = DateTime.UtcNow,
            SiteCode = localSite,
            Username = "system",
            ActionType = $"Sync:{action}",
            TargetType = "Sync",
            TargetId = siteCode,
            DetailsJson = message
        });
    }
}
