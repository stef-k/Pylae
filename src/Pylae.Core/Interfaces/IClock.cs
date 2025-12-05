namespace Pylae.Core.Interfaces;

public interface IClock
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}
