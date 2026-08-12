namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record SalidaResumenDto(
    Guid Id,
    string Nombre,
    DateTimeOffset FechaEncuentro,
    EstadoSalida Estado,
    bool EsCreador,
    int CantidadParticipantes);
