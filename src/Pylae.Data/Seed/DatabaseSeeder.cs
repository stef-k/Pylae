using Microsoft.EntityFrameworkCore;
using Pylae.Core.Constants;
using Pylae.Core.Enums;
using Pylae.Core.Interfaces;
using Pylae.Core.Models;
using Pylae.Data.Context;
using MemberTypeEntity = Pylae.Data.Entities.Master.MemberType;

namespace Pylae.Data.Seed;

public class DatabaseSeeder
{
    private readonly PylaeMasterDbContext _masterDbContext;
    private readonly PylaeVisitsDbContext _visitsDbContext;
    private readonly IUserService _userService;
    private readonly ISettingsService _settingsService;

    public DatabaseSeeder(
        PylaeMasterDbContext masterDbContext,
        PylaeVisitsDbContext visitsDbContext,
        IUserService userService,
        ISettingsService settingsService)
    {
        _masterDbContext = masterDbContext;
        _visitsDbContext = visitsDbContext;
        _userService = userService;
        _settingsService = settingsService;
    }

    public async Task SeedAsync(string initialAdminPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            await _masterDbContext.Database.MigrateAsync(cancellationToken);
            await _visitsDbContext.Database.MigrateAsync(cancellationToken);
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 26)
        {
            // Error 26: "file is not a database" - corrupted/invalid database file
            throw new InvalidOperationException(
                "Database files are corrupted. Please delete the database directory and restart the application. " +
                "Common causes: interrupted initialization, wrong encryption password, or permission issues.", ex);
        }
        catch (Microsoft.Data.Sqlite.SqliteException ex) when (ex.SqliteErrorCode == 11)
        {
            // Error 11: "database disk image is malformed" - usually encryption password issue
            throw new InvalidOperationException(
                "Database schema is malformed. This usually indicates an encryption password problem. " +
                "Ensure the encryption password is set correctly before initializing the database. " +
                "Please delete C:\\ProgramData\\Pylae and restart.", ex);
        }

        await _settingsService.EnsureDefaultsAsync(cancellationToken);

        var hasUsers = await _masterDbContext.Users.AnyAsync(cancellationToken);
        if (!hasUsers)
        {
            var adminUser = new User
            {
                Username = "admin",
                FirstName = "System",
                LastName = "Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                IsSystem = true
            };

            await _userService.CreateAsync(adminUser, initialAdminPassword, null, cancellationToken);
        }

        await SeedMemberTypesAsync(cancellationToken);
    }

    private async Task SeedMemberTypesAsync(CancellationToken cancellationToken)
    {
        if (await _masterDbContext.MemberTypes.AnyAsync(cancellationToken))
        {
            return;
        }

        var defaults = new[]
        {
            new MemberTypeEntity { Code = "staff", DisplayName = "Staff", IsActive = true, DisplayOrder = 1 },
            new MemberTypeEntity { Code = "visitor", DisplayName = "Visitor", IsActive = true, DisplayOrder = 2 },
            new MemberTypeEntity { Code = "supplier", DisplayName = "Supplier", IsActive = true, DisplayOrder = 3 }
        };

        _masterDbContext.MemberTypes.AddRange(defaults);
        await _masterDbContext.SaveChangesAsync(cancellationToken);
    }
}
