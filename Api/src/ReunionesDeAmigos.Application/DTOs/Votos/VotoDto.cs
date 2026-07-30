namespace ReunionesDeAmigos.Application.DTOs.Votos;

public sealed record VotoDto(
    Guid Id,
    Guid ParticipanteSalidaId,
    Guid PropuestaId,
    DateTimeOffset FechaCreacion,
    DateTimeOffset FechaUltimaModificacion);
