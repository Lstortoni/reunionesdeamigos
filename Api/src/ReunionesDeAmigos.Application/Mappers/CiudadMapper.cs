using ReunionesDeAmigos.Application.DTOs.Ciudades;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class CiudadMapper
{
    public static CiudadDto ToDto(Ciudad ciudad) =>
        new(
            ciudad.Id,
            ciudad.Nombre,
            ciudad.Provincia,
            ciudad.Pais);
}
