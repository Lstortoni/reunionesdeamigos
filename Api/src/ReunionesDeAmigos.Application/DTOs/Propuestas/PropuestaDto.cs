using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Propuestas;

public sealed record PropuestaDto(
    Guid Id,
    Guid ParticipanteSalidaId,
    TipoPropuesta Tipo,
    Guid? LugarId,
    string? NombreManual,
    string? DescripcionManual,
    string? DireccionManual,
    DateTimeOffset FechaCreacion);
