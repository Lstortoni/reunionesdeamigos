using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class SalidaMapper
{
    public static SalidaDto ToDto(
        Salida salida,
        DateTimeOffset fechaActual,
        string enlaceInvitacion)
    {
        var participantes = salida.Participantes
            .Select(ParticipanteSalidaMapper.ToDto)
            .ToArray();
        var propuestas = salida.Propuestas
            .Select(PropuestaMapper.ToDto)
            .ToArray();

        return new SalidaDto(
            salida.Id,
            salida.Nombre,
            salida.Descripcion,
            salida.FechaEncuentro,
            salida.FechaFinPropuestas,
            salida.FechaFinVotacion,
            salida.CodigoAcceso,
            enlaceInvitacion,
            salida.ObtenerEstado(fechaActual),
            salida.CreadorId,
            participantes,
            propuestas);
    }
}
