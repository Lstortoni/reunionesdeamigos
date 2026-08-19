namespace ReunionesDeAmigos.Domain.Models;

public sealed record ResultadoDisponibilidadFecha(
    int CantidadParticipantes,
    IReadOnlyCollection<DisponibilidadOpcionFecha> Opciones,
    IReadOnlyCollection<DisponibilidadOpcionFecha> OpcionesDestacadas);

public sealed record DisponibilidadOpcionFecha(
    Guid OpcionFechaId,
    DateTimeOffset FechaHora,
    int CantidadDisponibles,
    IReadOnlyCollection<Guid> ParticipantesDisponibles,
    IReadOnlyCollection<Guid> ParticipantesNoDisponibles,
    IReadOnlyCollection<Guid> ParticipantesSinResponder);
