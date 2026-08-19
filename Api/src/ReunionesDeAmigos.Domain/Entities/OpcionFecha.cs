namespace ReunionesDeAmigos.Domain.Entities;

public sealed class OpcionFecha
{
    private readonly List<DisponibilidadFecha> _disponibilidades = [];

    private OpcionFecha()
    {
    }

    private OpcionFecha(
        Guid id,
        Guid salidaId,
        Guid participanteSalidaId,
        DateTimeOffset fechaHora,
        DateTimeOffset fechaCreacion)
    {
        Id = id;
        SalidaId = salidaId;
        ParticipanteSalidaId = participanteSalidaId;
        FechaHora = fechaHora;
        FechaCreacion = fechaCreacion;
    }

    public Guid Id { get; private set; }

    public Guid SalidaId { get; private set; }

    public Guid ParticipanteSalidaId { get; private set; }

    public DateTimeOffset FechaHora { get; private set; }

    public DateTimeOffset FechaCreacion { get; private set; }

    public IReadOnlyCollection<DisponibilidadFecha> Disponibilidades =>
        _disponibilidades.AsReadOnly();

    internal static OpcionFecha Crear(
        Guid salidaId,
        Guid participanteSalidaId,
        DateTimeOffset fechaHora,
        DateTimeOffset fechaCreacion)
    {
        return new OpcionFecha(
            Guid.NewGuid(),
            salidaId,
            participanteSalidaId,
            fechaHora,
            fechaCreacion);
    }

    internal DisponibilidadFecha RegistrarDisponibilidad(
        Guid participanteSalidaId,
        bool disponible,
        DateTimeOffset fechaRespuesta)
    {
        var existente = _disponibilidades.FirstOrDefault(
            x => x.ParticipanteSalidaId == participanteSalidaId);

        if (existente is not null)
        {
            existente.Actualizar(disponible, fechaRespuesta);
            return existente;
        }

        var respuesta = DisponibilidadFecha.Crear(
            Id,
            participanteSalidaId,
            disponible,
            fechaRespuesta);

        _disponibilidades.Add(respuesta);
        return respuesta;
    }
}
