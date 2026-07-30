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
        CancellationToken cancellationToken);
}
