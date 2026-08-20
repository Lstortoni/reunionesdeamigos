namespace ReunionesDeAmigos.App.Models.Salidas;

public sealed class SalidaCreadaDto
{
    public Guid Id { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public string? Descripcion { get; init; }

    public ModalidadFecha Modalidad { get; init; }

    public DateTimeOffset? FechaEncuentro { get; init; }

    public DateTimeOffset FechaFinPropuestas { get; init; }

    public DateTimeOffset FechaFinVotacion { get; init; }

    public string CodigoAcceso { get; init; } = string.Empty;

    public string EnlaceInvitacion { get; init; } = string.Empty;

    public EstadoSalida Estado { get; init; }

    public Guid CreadorId { get; init; }

    public IReadOnlyCollection<ParticipanteSalidaDto> Participantes { get; init; }
        = [];

    public IReadOnlyCollection<OpcionFechaDto> OpcionesFecha { get; init; }
        = [];

    public IReadOnlyCollection<PropuestaDto> Propuestas { get; init; }
        = [];
}
