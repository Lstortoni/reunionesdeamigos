using ReunionesDeAmigos.Domain.Enums;
using ReunionesDeAmigos.Application.DTOs.Propuestas;

namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record SalidaDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    ModalidadFecha Modalidad,
    DateTimeOffset? FechaEncuentro,
    DateTimeOffset FechaFinPropuestas,
    DateTimeOffset FechaFinVotacion,
    string CodigoAcceso,
    string EnlaceInvitacion,
    EstadoSalida Estado,
    Guid CreadorId,
    IReadOnlyCollection<ParticipanteSalidaDto> Participantes,
    IReadOnlyCollection<OpcionFechaDto> OpcionesFecha,
    IReadOnlyCollection<PropuestaDto> Propuestas);
