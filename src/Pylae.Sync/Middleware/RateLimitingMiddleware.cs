using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Pylae.Sync.Middleware;

/// <summary>
/// Simple rate limiting middleware to prevent brute force and resource exhaustion.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly int _maxRequestsPerMinute;
    private readonly int _maxRequestsPerHour;

    // Tracks requests by IP address
    private static readonly ConcurrentDictionary<string, RequestCounter> _requestCounters = new();

    public RateLimitingMiddleware(
        RequestDelegate next,
        ILogger<RateLimitingMiddleware> logger,
        int maxRequestsPerMinute = 60,
        int maxRequestsPerHour = 1000)
    {
        _next = next;
        _logger = logger;
        _maxRequestsPerMinute = maxRequestsPerMinute;
        _maxRequestsPerHour = maxRequestsPerHour;

        // Start cleanup timer to remove old entries
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(5));
                CleanupOldCounters();
            }
        });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var counter = _requestCounters.GetOrAdd(clientIp, _ => new RequestCounter());

        counter.IncrementRequests();

        if (counter.MinuteRequestCount > _maxRequestsPerMinute)
        {
            _logger.LogWarning("Rate limit exceeded (per-minute) for IP {ClientIp}: {Count} requests",
                clientIp, counter.MinuteRequestCount);
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        if (counter.HourRequestCount > _maxRequestsPerHour)
        {
            _logger.LogWarning("Rate limit exceeded (per-hour) for IP {ClientIp}: {Count} requests",
                clientIp, counter.HourRequestCount);
            context.Response.StatusCode = 429; // Too Many Requests
            await context.Response.WriteAsync("Rate limit exceeded. Please try again later.");
            return;
        }

        await _next(context);
    }

    private static void CleanupOldCounters()
    {
        var cutoffTime = DateTime.UtcNow.AddHours(-2);
        var keysToRemove = _requestCounters
            .Where(kvp => kvp.Value.LastRequestTime < cutoffTime)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _requestCounters.TryRemove(key, out _);
        }
    }

    private class RequestCounter
    {
        private readonly object _lock = new();
        private readonly Queue<DateTime> _minuteRequests = new();
        private readonly Queue<DateTime> _hourRequests = new();

        public DateTime LastRequestTime { get; private set; } = DateTime.UtcNow;
        public int MinuteRequestCount => _minuteRequests.Count;
        public int HourRequestCount => _hourRequests.Count;

        public void IncrementRequests()
        {
            lock (_lock)
            {
                var now = DateTime.UtcNow;
                LastRequestTime = now;

                // Clean up old minute requests
                while (_minuteRequests.Count > 0 && _minuteRequests.Peek() < now.AddMinutes(-1))
                {
                    _minuteRequests.Dequeue();
                }

                // Clean up old hour requests
                while (_hourRequests.Count > 0 && _hourRequests.Peek() < now.AddHours(-1))
                {
                    _hourRequests.Dequeue();
                }

                // Add new request
                _minuteRequests.Enqueue(now);
                _hourRequests.Enqueue(now);
            }
        }
    }
}

/// <summary>
/// Extension methods for adding rate limiting middleware.
/// </summary>
public static class RateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseRateLimiting(
        this IApplicationBuilder builder,
        int maxRequestsPerMinute = 60,
        int maxRequestsPerHour = 1000)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>(maxRequestsPerMinute, maxRequestsPerHour);
    }
}
