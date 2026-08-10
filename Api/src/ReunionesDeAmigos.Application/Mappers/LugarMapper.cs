using ReunionesDeAmigos.Application.DTOs.Lugares;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class LugarMapper
{
    public static LugarDto ToDto(Lugar lugar)
    {
        return new LugarDto(
            lugar.Id,
            lugar.Nombre,
            lugar.Descripcion,
            lugar.Direccion,
            lugar.Barrio,
            CiudadMapper.ToDto(lugar.Ciudad),
            lugar.Tipo,
            lugar.Latitud,
            lugar.Longitud,
            lugar.Activo);
    }
}
