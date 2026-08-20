namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed record OpcionFechaDto(
    Guid Id,
    Guid ParticipanteSalidaId,
    DateTimeOffset FechaHora,
    DateTimeOffset FechaCreacion);
