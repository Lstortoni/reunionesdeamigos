using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record CrearPropuestaInicialRequest(
    TipoPropuesta Tipo,
    string? GooglePlaceId,
    string? NombreManual,
    string? DescripcionManual,
    string? DireccionManual);
