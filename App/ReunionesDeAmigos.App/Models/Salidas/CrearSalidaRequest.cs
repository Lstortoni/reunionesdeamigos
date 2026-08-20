namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record CrearSalidaRequest(
    string Nombre,
    string? Descripcion,
    ModalidadFecha Modalidad,
    DateTimeOffset? FechaEncuentro,
    IReadOnlyCollection<DateTimeOffset> OpcionesFechaIniciales,
    int DiasParaPropuestas,
    int DiasParaVotar,
    IReadOnlyCollection<CrearPropuestaInicialRequest> PropuestasIniciales);
