namespace ReunionesDeAmigos.Application.DTOs.LugaresExternos;

public sealed record LugarExternoDto(
    string GooglePlaceId,
    string Nombre,
    string? Direccion,
    decimal Latitud,
    decimal Longitud,
    string? TipoGoogle,
    string? GoogleMapsUri);
