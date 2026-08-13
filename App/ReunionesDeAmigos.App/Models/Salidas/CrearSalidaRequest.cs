namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record CrearSalidaRequest(
    string Nombre,
    string? Descripcion,
    DateTimeOffset FechaEncuentro,
    int DiasParaPropuestas,
    int DiasParaVotar,
    IReadOnlyCollection<CrearPropuestaInicialRequest> PropuestasIniciales);
