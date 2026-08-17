namespace ReunionesDeAmigos.App.Models.Lugares;

public sealed record LugarExternoDto(
    string GooglePlaceId,
    string Nombre,
    string Direccion,
    double Latitud,
    double Longitud,
    string? TipoGoogle,
    string GoogleMapsUri);
