using System.Collections.Concurrent;

namespace Pylae.Desktop.Services;

public record SyncEvent(DateTime Timestamp, string Direction, string Message);

public class SyncHistoryService
{
    private readonly ConcurrentQueue<SyncEvent> _events = new();
    private const int MaxEvents = 200;

    public void Add(string direction, string message)
    {
        _events.Enqueue(new SyncEvent(DateTime.Now, direction, message));
        while (_events.Count > MaxEvents && _events.TryDequeue(out _)) { }
    }

    public IReadOnlyCollection<SyncEvent> GetRecent()
    {
        return _events.ToArray();
    }
}
