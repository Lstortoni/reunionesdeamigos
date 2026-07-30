namespace ReunionesDeAmigos.Application.DTOs.Votos;

public sealed record PropuestaVotadaDto(
    Guid PropuestaId,
    int CantidadVotos);

public sealed record ResultadoVotacionDto(
    int CantidadTotalVotos,
    IReadOnlyCollection<PropuestaVotadaDto> Ganadoras,
    bool TieneGanador,
    bool HayEmpate);
