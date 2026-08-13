using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public interface ISalidasApiService
{
    Task<SalidaCreadaDto> CrearAsync(
        CrearSalidaRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalidaResumenDto>> ObtenerMiasAsync(
        CancellationToken cancellationToken = default);
}
