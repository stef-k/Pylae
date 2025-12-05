using Pylae.Core.Interfaces;

namespace Pylae.Core.Services;

public class SystemClock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime Now => DateTime.Now;
}
