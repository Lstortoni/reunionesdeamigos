using ReunionesDeAmigos.App.Models.Ciudades;

namespace ReunionesDeAmigos.App.Services;

public interface ICiudadesApiService
{
    Task<IReadOnlyCollection<CiudadDto>> ObtenerAsync(CancellationToken cancellationToken = default);
}
