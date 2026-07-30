namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record CrearSalidaRequest(
    string Nombre,
    string? Descripcion,
    DateTimeOffset FechaEncuentro,
    int DiasParaPropuestas,
    int DiasParaVotar);
