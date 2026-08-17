namespace ReunionesDeAmigos.Application.DTOs.LugaresExternos;

public sealed record LugarExternoDetalleDto(
    string GooglePlaceId,
    string Nombre,
    string? Direccion,
    decimal Latitud,
    decimal Longitud,
    string? TipoGoogle,
    string? GoogleMapsUri,
    string? SitioWebUri,
    string? Telefono,
    decimal? Calificacion,
    int? CantidadCalificaciones,
    IReadOnlyCollection<string> Horarios);
