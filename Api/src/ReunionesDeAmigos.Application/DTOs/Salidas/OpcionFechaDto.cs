namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record OpcionFechaDto(
    Guid Id,
    Guid ParticipanteSalidaId,
    DateTimeOffset FechaHora,
    DateTimeOffset FechaCreacion);
