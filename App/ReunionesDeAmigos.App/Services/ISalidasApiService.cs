using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public interface ISalidasApiService
{
    Task<IReadOnlyCollection<SalidaResumenDto>> ObtenerMiasAsync(
        CancellationToken cancellationToken = default);
}
