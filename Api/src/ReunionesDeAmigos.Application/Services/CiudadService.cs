using ReunionesDeAmigos.Application.DTOs.Ciudades;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;

namespace ReunionesDeAmigos.Application.Services;

public sealed class CiudadService(ICiudadRepository ciudadRepository)
    : ICiudadService
{
    public async Task<IReadOnlyCollection<CiudadDto>> ObtenerActivasAsync(
        CancellationToken cancellationToken)
    {
        var ciudades = await ciudadRepository.ObtenerActivasAsync(
            cancellationToken);

        return ciudades.Select(CiudadMapper.ToDto).ToArray();
    }
}
