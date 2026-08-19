namespace ReunionesDeAmigos.Domain.Entities;

public sealed class DisponibilidadFecha
{
    private DisponibilidadFecha()
    {
    }

    private DisponibilidadFecha(
        Guid id,
        Guid opcionFechaId,
        Guid participanteSalidaId,
        bool disponible,
        DateTimeOffset fechaRespuesta)
    {
        Id = id;
        OpcionFechaId = opcionFechaId;
        ParticipanteSalidaId = participanteSalidaId;
        Disponible = disponible;
        FechaRespuesta = fechaRespuesta;
    }

    public Guid Id { get; private set; }

    public Guid OpcionFechaId { get; private set; }

    public Guid ParticipanteSalidaId { get; private set; }

    public bool Disponible { get; private set; }

    public DateTimeOffset FechaRespuesta { get; private set; }

    internal static DisponibilidadFecha Crear(
        Guid opcionFechaId,
        Guid participanteSalidaId,
        bool disponible,
        DateTimeOffset fechaRespuesta)
    {
        return new DisponibilidadFecha(
            Guid.NewGuid(),
            opcionFechaId,
            participanteSalidaId,
            disponible,
            fechaRespuesta);
    }

    internal void Actualizar(
        bool disponible,
        DateTimeOffset fechaRespuesta)
    {
        Disponible = disponible;
        FechaRespuesta = fechaRespuesta;
    }
}
