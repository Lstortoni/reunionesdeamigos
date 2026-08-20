using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record CrearSalidaRequest(
    string Nombre,
    string? Descripcion,
    ModalidadFecha Modalidad,
    DateTimeOffset? FechaEncuentro,
    IReadOnlyCollection<DateTimeOffset>? OpcionesFechaIniciales,
    int DiasParaPropuestas,
    int DiasParaVotar,
    IReadOnlyCollection<CrearPropuestaInicialRequest> PropuestasIniciales);
