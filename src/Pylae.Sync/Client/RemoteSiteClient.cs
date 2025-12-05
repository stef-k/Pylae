using System.Net.Http.Headers;
using System.Net.Http.Json;
using Pylae.Sync.Models;

namespace Pylae.Sync.Client;

public class RemoteSiteClient : IRemoteSiteClient
{
    private readonly HttpClient _httpClient;

    public RemoteSiteClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SyncInfoResponse?> GetInfoAsync(RemoteSiteConfig config, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest(HttpMethod.Get, config, "/api/sync/info");
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<SyncInfoResponse>(cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyCollection<Pylae.Core.Models.Visit>> GetVisitsAsync(RemoteSiteConfig config, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var path = BuildVisitsPath(from, to);
        using var request = BuildRequest(HttpMethod.Get, config, path);
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return Array.Empty<Pylae.Core.Models.Visit>();
        }

        var visits = await response.Content.ReadFromJsonAsync<List<Pylae.Core.Models.Visit>>(cancellationToken: cancellationToken);
        return visits?.ToArray() ?? Array.Empty<Pylae.Core.Models.Visit>();
    }

    public async Task<byte[]?> GetFullVisitsDatabaseAsync(RemoteSiteConfig config, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest(HttpMethod.Get, config, "/api/sync/visits/full");
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public async Task<MasterPackage?> GetMasterPackageAsync(RemoteSiteConfig config, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest(HttpMethod.Get, config, "/api/sync/master");
        using var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<MasterPackage>(cancellationToken: cancellationToken);
    }

    public async Task<bool> UploadMasterPackageAsync(RemoteSiteConfig config, MasterPackage package, CancellationToken cancellationToken = default)
    {
        using var request = BuildRequest(HttpMethod.Post, config, "/api/sync/master");
        request.Content = JsonContent.Create(package);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    private HttpRequestMessage BuildRequest(HttpMethod method, RemoteSiteConfig config, string path)
    {
        var request = new HttpRequestMessage(method, new Uri(config.ToBaseUri(), path));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Add("X-Api-Key", config.ApiKey);
        return request;
    }

    private static string BuildVisitsPath(DateTime? from, DateTime? to)
    {
        var parameters = new List<string>();
        if (from.HasValue)
        {
            parameters.Add($"from={from:yyyy-MM-dd}");
        }

        if (to.HasValue)
        {
            parameters.Add($"to={to:yyyy-MM-dd}");
        }

        var suffix = parameters.Any() ? "?" + string.Join("&", parameters) : string.Empty;
        return "/api/sync/visits" + suffix;
    }
}
