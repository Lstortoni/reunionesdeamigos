using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record CrearPropuestaInicialRequest(
    TipoPropuesta Tipo,
    Guid? LugarId,
    string? NombreManual,
    string? DescripcionManual,
    string? DireccionManual);
