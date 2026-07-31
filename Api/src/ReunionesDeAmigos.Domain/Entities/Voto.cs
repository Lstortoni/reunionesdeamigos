namespace ReunionesDeAmigos.Domain.Entities;

public sealed class Voto
{
    private Voto()
    {
    }

    private Voto(
        Guid id,
        Guid salidaId,
        Guid participanteSalidaId,
        Guid propuestaId,
        DateTimeOffset fechaCreacion)
    {
        Id = id;
        SalidaId = salidaId;
        ParticipanteSalidaId = participanteSalidaId;
        PropuestaId = propuestaId;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaCreacion;
    }

    public Guid Id { get; private set; }

    public Guid SalidaId { get; private set; }

    public Guid ParticipanteSalidaId { get; private set; }

    public Guid PropuestaId { get; private set; }

    public DateTimeOffset FechaCreacion { get; private set; }

    public DateTimeOffset FechaUltimaModificacion { get; private set; }

    internal static Voto Crear(
        Guid salidaId,
        Guid participanteSalidaId,
        Guid propuestaId,
        DateTimeOffset fechaCreacion)
    {
        return new Voto(
            Guid.NewGuid(),
            salidaId,
            participanteSalidaId,
            propuestaId,
            fechaCreacion);
    }

    internal void CambiarPropuesta(
        Guid propuestaId,
        DateTimeOffset fechaModificacion)
    {
        PropuestaId = propuestaId;
        FechaUltimaModificacion = fechaModificacion;
    }
}
