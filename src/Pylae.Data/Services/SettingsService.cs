using Microsoft.EntityFrameworkCore;
using Pylae.Core.Constants;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using SettingEntity = Pylae.Data.Entities.Master.Setting;

namespace Pylae.Data.Services;

public class SettingsService : ISettingsService
{
    private readonly PylaeMasterDbContext _dbContext;

    public SettingsService(PylaeMasterDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<string?> GetValueAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.Settings.AsNoTracking().FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
        return setting?.Value;
    }

    public async Task<IDictionary<string, string>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var values = await _dbContext.Settings.AsNoTracking().ToListAsync(cancellationToken);
        return values.ToDictionary(x => x.Key, x => x.Value);
    }

    public async Task UpsertAsync(IEnumerable<Setting> settings, CancellationToken cancellationToken = default)
    {
        foreach (var setting in settings)
        {
            await SetValueAsync(setting.Key, setting.Value, cancellationToken);
        }
    }

    public async Task SetValueAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
        if (existing is null)
        {
            _dbContext.Settings.Add(new SettingEntity
            {
                Key = key,
                Value = value,
                UpdatedAtUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.Value = value;
            existing.UpdatedAtUtc = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task EnsureDefaultsAsync(CancellationToken cancellationToken = default)
    {
        foreach (var (key, value) in DefaultSettings.All)
        {
            var existing = await _dbContext.Settings.FirstOrDefaultAsync(x => x.Key == key, cancellationToken);
            if (existing is null)
            {
                _dbContext.Settings.Add(new SettingEntity
                {
                    Key = key,
                    Value = value,
                    UpdatedAtUtc = DateTime.UtcNow
                });
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
