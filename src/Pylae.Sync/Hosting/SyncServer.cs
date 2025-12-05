using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pylae.Data.Context;
using Pylae.Sync.Models;
using Pylae.Sync.Middleware;
using System.IO.Compression;
using System.Linq;

namespace Pylae.Sync.Hosting;

public class SyncServer : IAsyncDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncServer> _logger;
    private WebApplication? _app;

    public SyncServer(IServiceScopeFactory scopeFactory, ILogger<SyncServer> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public bool IsRunning => _app is not null;

    public async Task StartAsync(SyncServerOptions options, CancellationToken cancellationToken = default)
    {
        if (!options.NetworkEnabled)
        {
            _logger.LogInformation("Network sync disabled; HTTP server not started.");
            return;
        }

        if (_app is not null)
        {
            return;
        }

        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            ApplicationName = typeof(SyncServer).Assembly.FullName,
            ContentRootPath = AppContext.BaseDirectory
        });

        builder.Services.AddSingleton(_scopeFactory);
        builder.Services.AddSingleton(options);
        builder.Services.AddLogging();
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(options.Port);
        });

        var app = builder.Build();

        // Add rate limiting middleware
        app.UseRateLimiting(maxRequestsPerMinute: 60, maxRequestsPerHour: 1000);

        // Health check endpoint (no authentication required for monitoring)
        app.MapGet("/api/health", async (IServiceScopeFactory scopeFactory) =>
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var masterDb = scope.ServiceProvider.GetRequiredService<PylaeMasterDbContext>();
                var visitsDb = scope.ServiceProvider.GetRequiredService<PylaeVisitsDbContext>();

                // Check database connectivity
                await masterDb.Database.CanConnectAsync(cancellationToken);
                await visitsDb.Database.CanConnectAsync(cancellationToken);

                return Results.Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    checks = new
                    {
                        masterDatabase = "ok",
                        visitsDatabase = "ok"
                    }
                });
            }
            catch (Exception ex)
            {
                return Results.Json(new
                {
                    status = "unhealthy",
                    timestamp = DateTime.UtcNow,
                    error = ex.Message
                }, statusCode: 503);
            }
        });

        app.MapGet("/api/sync/info", async (HttpContext context, IServiceScopeFactory scopeFactory, SyncServerOptions opts) =>
        {
            if (!Authorize(context, opts.ApiKey))
            {
                return Results.Unauthorized();
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var masterDb = scope.ServiceProvider.GetRequiredService<PylaeMasterDbContext>();
            var visitsDb = scope.ServiceProvider.GetRequiredService<PylaeVisitsDbContext>();

            var memberCount = await masterDb.Members.CountAsync(cancellationToken);
            var visitsCount = await visitsDb.Visits.LongCountAsync(cancellationToken);
            var lastVisit = await visitsDb.Visits.OrderByDescending(v => v.TimestampUtc).Select(v => v.TimestampUtc).FirstOrDefaultAsync(cancellationToken);

            var payload = new SyncInfoResponse
            {
                SiteCode = opts.SiteCode,
                SiteDisplayName = opts.SiteDisplayName,
                MemberCount = memberCount,
                VisitsCount = visitsCount,
                LastVisitTimestampUtc = lastVisit == default ? null : lastVisit
            };

            return Results.Ok(payload);
        });

        app.MapGet("/api/sync/visits/full", async (HttpContext context, IServiceScopeFactory scopeFactory, SyncServerOptions opts) =>
        {
            if (!Authorize(context, opts.ApiKey))
            {
                return Results.Unauthorized();
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var options = scope.ServiceProvider.GetRequiredService<DatabaseOptions>();
            var visitsPath = options.GetVisitsDbPath();

            if (!File.Exists(visitsPath))
            {
                return Results.NotFound();
            }

            var bytes = await File.ReadAllBytesAsync(visitsPath, cancellationToken);
            return Results.File(bytes, "application/octet-stream", "visits.db");
        });

        app.MapGet("/api/sync/visits", async (HttpContext context, IServiceScopeFactory scopeFactory, SyncServerOptions opts) =>
        {
            if (!Authorize(context, opts.ApiKey))
            {
                return Results.Unauthorized();
            }

            var fromQuery = context.Request.Query["from"].FirstOrDefault();
            var toQuery = context.Request.Query["to"].FirstOrDefault();
            DateTime? from = ParseDate(fromQuery);
            DateTime? to = ParseDate(toQuery);

            await using var scope = scopeFactory.CreateAsyncScope();
            var visitsDb = scope.ServiceProvider.GetRequiredService<PylaeVisitsDbContext>();
            var query = visitsDb.Visits.AsNoTracking().AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(v => v.TimestampUtc >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(v => v.TimestampUtc <= to.Value);
            }

            var payload = await query.ToListAsync(cancellationToken);
            return Results.Ok(payload);
        });

        app.MapPost("/api/sync/master", async (HttpContext context, IServiceScopeFactory scopeFactory, SyncServerOptions opts) =>
        {
            if (!Authorize(context, opts.ApiKey))
            {
                return Results.Unauthorized();
            }

            var package = await context.Request.ReadFromJsonAsync<MasterPackage>(cancellationToken);
            if (package is null)
            {
                return Results.BadRequest();
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var options = scope.ServiceProvider.GetRequiredService<DatabaseOptions>();

            if (package.MasterDatabase?.Length > 0)
            {
                await File.WriteAllBytesAsync(options.GetMasterDbPath(), package.MasterDatabase, cancellationToken);
            }

            if (package.PhotosArchive is { Length: > 0 })
            {
                await ExtractPhotosAsync(package.PhotosArchive, options.GetPhotosPath(), pruneMissing: true, cancellationToken);
            }

            return Results.Accepted();
        });

        app.MapGet("/api/sync/master", async (HttpContext context, IServiceScopeFactory scopeFactory, SyncServerOptions opts) =>
        {
            if (!Authorize(context, opts.ApiKey))
            {
                return Results.Unauthorized();
            }

            await using var scope = scopeFactory.CreateAsyncScope();
            var options = scope.ServiceProvider.GetRequiredService<DatabaseOptions>();

            var masterPath = options.GetMasterDbPath();
            if (!File.Exists(masterPath))
            {
                return Results.NotFound();
            }

            var package = new MasterPackage
            {
                SiteCode = opts.SiteCode,
                MasterDatabase = await File.ReadAllBytesAsync(masterPath, cancellationToken),
                PhotosArchive = await BuildPhotosArchiveAsync(options.GetPhotosPath(), cancellationToken)
            };

            return Results.Ok(package);
        });

        await app.StartAsync(cancellationToken);
        _app = app;
        _logger.LogInformation("Sync server started on port {Port}", options.Port);
    }

    private static bool Authorize(HttpContext context, string expectedApiKey)
    {
        if (string.IsNullOrWhiteSpace(expectedApiKey))
        {
            return false;
        }

        var header = context.Request.Headers["X-Api-Key"].FirstOrDefault();
        return string.Equals(header, expectedApiKey, StringComparison.Ordinal);
    }

    private static DateTime? ParseDate(string? value)
    {
        if (DateTime.TryParse(value, out var parsed))
        {
            return parsed;
        }

        return null;
    }

    private static async Task<byte[]?> BuildPhotosArchiveAsync(string photosPath, CancellationToken cancellationToken)
    {
        if (!Directory.Exists(photosPath))
        {
            return null;
        }

        await using var buffer = new MemoryStream();
        using var zip = new ZipArchive(buffer, ZipArchiveMode.Create, leaveOpen: true);
        foreach (var file in Directory.GetFiles(photosPath))
        {
            var name = Path.GetFileName(file);
            zip.CreateEntryFromFile(file, Path.Combine("Photos", name), CompressionLevel.Fastest);
        }

        return buffer.ToArray();
    }

    private static async Task ExtractPhotosAsync(byte[] archiveBytes, string destinationFolder, bool pruneMissing, CancellationToken cancellationToken)
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
            await entryStream.CopyToAsync(fileStream, cancellationToken);
            incoming.Add(Path.GetFileName(entry.FullName));
        }

        if (pruneMissing)
        {
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
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is null)
        {
            return;
        }

        await _app.StopAsync();
        await _app.DisposeAsync();
        _app = null;
    }
}
