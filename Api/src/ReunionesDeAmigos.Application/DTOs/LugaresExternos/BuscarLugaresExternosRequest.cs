using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.LugaresExternos;

public sealed record BuscarLugaresExternosRequest(
    Guid CiudadId,
    TipoLugar? Tipo,
    string? Barrio,
    string? Texto,
    string? Idioma,
    int? Cantidad);
