using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Lugares;

public sealed record LugarDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    string Direccion,
    string? Barrio,
    string Ciudad,
    TipoLugar Tipo,
    decimal? Latitud,
    decimal? Longitud,
    bool Activo);
