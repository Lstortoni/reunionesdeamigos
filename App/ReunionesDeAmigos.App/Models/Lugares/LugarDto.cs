using ReunionesDeAmigos.App.Models.Ciudades;

namespace ReunionesDeAmigos.App.Models.Lugares;

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
