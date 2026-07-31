using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Tests.Fakes;

internal sealed class AlmacenamientoEnMemoria
{
    public List<Usuario> Usuarios { get; } = [];

    public List<Salida> Salidas { get; } = [];

    public List<Lugar> Lugares { get; } = [];
}
