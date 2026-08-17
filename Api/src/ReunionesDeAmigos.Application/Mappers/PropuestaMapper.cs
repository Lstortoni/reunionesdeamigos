using ReunionesDeAmigos.Application.DTOs.Propuestas;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class PropuestaMapper
{
    public static PropuestaDto ToDto(Propuesta propuesta)
    {
        return new PropuestaDto(
            propuesta.Id,
            propuesta.ParticipanteSalidaId,
            propuesta.Tipo,
            propuesta.GooglePlaceId,
            propuesta.NombreManual,
            propuesta.DescripcionManual,
            propuesta.DireccionManual,
            propuesta.FechaCreacion);
    }
}
