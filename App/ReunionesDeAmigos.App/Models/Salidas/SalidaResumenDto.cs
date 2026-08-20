namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record SalidaResumenDto(
    Guid Id,
    string Nombre,
    ModalidadFecha Modalidad,
    DateTimeOffset? FechaEncuentro,
    EstadoSalida Estado,
    bool EsCreador,
    int CantidadParticipantes);
