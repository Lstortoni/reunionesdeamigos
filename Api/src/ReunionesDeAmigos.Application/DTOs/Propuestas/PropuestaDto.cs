using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Propuestas;

public sealed record PropuestaDto(
    Guid Id,
    Guid ParticipanteSalidaId,
    TipoPropuesta Tipo,
    string? GooglePlaceId,
    string? NombreManual,
    string? DescripcionManual,
    string? DireccionManual,
    DateTimeOffset FechaCreacion);
