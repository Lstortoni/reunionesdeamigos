using ReunionesDeAmigos.Application.DTOs.Salidas;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface ISalidaService
{
    Task<SalidaDto> CrearAsync(
        CrearSalidaRequest request,
        Guid creadorId,
        CancellationToken cancellationToken);

    Task<SalidaDto> ObtenerPorIdAsync(
        Guid salidaId,
        Guid usuarioId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<SalidaResumenDto>> ObtenerMiasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken);
}
