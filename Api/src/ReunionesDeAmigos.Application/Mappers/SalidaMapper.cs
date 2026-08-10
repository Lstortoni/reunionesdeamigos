using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class SalidaMapper
{
    public static SalidaDto ToDto(
        Salida salida,
        DateTimeOffset fechaActual)
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
            salida.ObtenerEstado(fechaActual),
            salida.CreadorId,
            participantes,
            propuestas);
    }
}
