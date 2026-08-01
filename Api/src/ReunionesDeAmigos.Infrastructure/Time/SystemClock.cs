using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Infrastructure.Time;

internal sealed class SystemClock : IClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
