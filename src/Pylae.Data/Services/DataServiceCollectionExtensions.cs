using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pylae.Core.Interfaces;
using Pylae.Core.Security;
using Pylae.Data.Context;
using Pylae.Data.Seed;

namespace Pylae.Data.Services;

public static class DataServiceCollectionExtensions
{
    public static IServiceCollection AddPylaeData(this IServiceCollection services, Action<DatabaseOptions> configureOptions)
    {
        var options = new DatabaseOptions();
        configureOptions(options);
        return services.AddPylaeData(options);
    }

    public static IServiceCollection AddPylaeData(this IServiceCollection services, DatabaseOptions options)
    {
        DatabaseConfig.EnsureDirectories(options);

        services.AddSingleton(options);

        services.AddDbContext<PylaeMasterDbContext>((serviceProvider, builder) =>
        {
            var opts = serviceProvider.GetRequiredService<DatabaseOptions>();
            DatabaseConfig.ConfigureMaster(builder, opts.GetMasterDbPath(), opts.EncryptionPassword);
        });

        services.AddDbContext<PylaeVisitsDbContext>((serviceProvider, builder) =>
        {
            var opts = serviceProvider.GetRequiredService<DatabaseOptions>();
            DatabaseConfig.ConfigureVisits(builder, opts.GetVisitsDbPath(), opts.EncryptionPassword);
        });

        services.AddScoped<ISecretHasher, SecretHasher>();
        services.AddSingleton<IBadgeEvaluator, Core.Services.BadgeEvaluator>();
        services.AddSingleton<IClock, Core.Services.SystemClock>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IMemberTypeService, MemberTypeService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IVisitService, VisitService>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }
}
