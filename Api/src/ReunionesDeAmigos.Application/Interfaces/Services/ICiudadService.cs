using ReunionesDeAmigos.Application.DTOs.Ciudades;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface ICiudadService
{
    Task<IReadOnlyCollection<CiudadDto>> ObtenerActivasAsync(
        CancellationToken cancellationToken);
}
