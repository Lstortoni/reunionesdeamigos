using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class ParticipanteSalidaMapper
{
    public static ParticipanteSalidaDto ToDto(
        ParticipanteSalida participante)
    {
        return new ParticipanteSalidaDto(
            participante.Id,
            participante.UsuarioId,
            participante.NombreVisible,
            participante.FechaIngreso,
            participante.EsInvitado);
    }
}
