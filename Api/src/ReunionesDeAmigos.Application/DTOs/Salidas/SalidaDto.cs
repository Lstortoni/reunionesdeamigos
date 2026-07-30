using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record SalidaDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    DateTimeOffset FechaEncuentro,
    DateTimeOffset FechaFinPropuestas,
    DateTimeOffset FechaFinVotacion,
    string CodigoAcceso,
    EstadoSalida Estado,
    Guid CreadorId,
    IReadOnlyCollection<ParticipanteSalidaDto> Participantes);
