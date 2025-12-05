using Microsoft.Extensions.DependencyInjection;
using Pylae.Sync.Client;
using Pylae.Sync.Hosting;

namespace Pylae.Sync;

public static class SyncServiceCollectionExtensions
{
    public static IServiceCollection AddPylaeSync(this IServiceCollection services)
    {
        services.AddHttpClient<IRemoteSiteClient, RemoteSiteClient>();
        services.AddSingleton<SyncServer>();
        services.AddSingleton<SyncServerOptions>();
        return services;
    }
}
