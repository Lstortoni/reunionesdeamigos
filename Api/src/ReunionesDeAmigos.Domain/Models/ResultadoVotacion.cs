namespace ReunionesDeAmigos.Domain.Models;

public sealed record PropuestaVotada(
    Guid PropuestaId,
    int CantidadVotos);

public sealed record ResultadoVotacion(
    int CantidadTotalVotos,
    IReadOnlyCollection<PropuestaVotada> Ganadoras)
{
    public bool TieneGanador => Ganadoras.Count > 0;

    public bool HayEmpate => Ganadoras.Count > 1;
}
