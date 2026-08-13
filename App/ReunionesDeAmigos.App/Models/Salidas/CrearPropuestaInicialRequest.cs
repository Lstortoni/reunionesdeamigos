namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record CrearPropuestaInicialRequest(
    TipoPropuesta Tipo,
    Guid? LugarId,
    string? NombreManual,
    string? DescripcionManual,
    string? DireccionManual);
