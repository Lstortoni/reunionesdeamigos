using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public interface ISalidasApiService
{
    Task<SalidaCreadaDto> CrearAsync(
        CrearSalidaRequest request,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SalidaResumenDto>> ObtenerMiasAsync(
        CancellationToken cancellationToken = default);

    Task<SalidaCreadaDto> ObtenerPorIdAsync(
        Guid salidaId,
        CancellationToken cancellationToken = default);
}
