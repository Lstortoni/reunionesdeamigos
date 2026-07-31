using ReunionesDeAmigos.Application.DTOs.Votos;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Models;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class VotoMapper
{
    public static VotoDto ToDto(Voto voto)
    {
        return new VotoDto(
            voto.Id,
            voto.ParticipanteSalidaId,
            voto.PropuestaId,
            voto.FechaCreacion,
            voto.FechaUltimaModificacion);
    }

    public static ResultadoVotacionDto ToDto(
        ResultadoVotacion resultado)
    {
        var ganadoras = resultado.Ganadoras
            .Select(ganadora => new PropuestaVotadaDto(
                ganadora.PropuestaId,
                ganadora.CantidadVotos))
            .ToArray();

        return new ResultadoVotacionDto(
            resultado.CantidadTotalVotos,
            ganadoras,
            resultado.TieneGanador,
            resultado.HayEmpate);
    }
}
