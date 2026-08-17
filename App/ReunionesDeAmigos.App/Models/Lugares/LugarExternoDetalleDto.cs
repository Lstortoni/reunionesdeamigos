namespace ReunionesDeAmigos.App.Models.Lugares;

public sealed record LugarExternoDetalleDto(
    string GooglePlaceId,
    string Nombre,
    string? Direccion,
    double Latitud,
    double Longitud,
    string? TipoGoogle,
    string? GoogleMapsUri,
    string? SitioWebUri,
    string? Telefono,
    decimal? Calificacion,
    int? CantidadCalificaciones,
    IReadOnlyCollection<string> Horarios);
