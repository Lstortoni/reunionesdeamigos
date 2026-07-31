using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Application.Tests.Fakes;

internal sealed class ClockFijo(DateTimeOffset utcNow) : IClock
{
    public DateTimeOffset UtcNow { get; } = utcNow;
}

internal sealed class CodigoAccesoGeneratorFijo(string codigo)
    : ICodigoAccesoGenerator
{
    public string Generar()
    {
        return codigo;
    }
}
