namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record CrearPropuestaInicialRequest(
    TipoPropuesta Tipo,
    string? GooglePlaceId,
    string? NombreManual,
    string? DescripcionManual,
    string? DireccionManual);
