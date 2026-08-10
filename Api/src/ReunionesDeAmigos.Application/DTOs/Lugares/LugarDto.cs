using ReunionesDeAmigos.Domain.Enums;
using ReunionesDeAmigos.Application.DTOs.Ciudades;

namespace ReunionesDeAmigos.Application.DTOs.Lugares;

public sealed record LugarDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    string Direccion,
    string? Barrio,
    CiudadDto Ciudad,
    TipoLugar Tipo,
    decimal? Latitud,
    decimal? Longitud,
    bool Activo);
