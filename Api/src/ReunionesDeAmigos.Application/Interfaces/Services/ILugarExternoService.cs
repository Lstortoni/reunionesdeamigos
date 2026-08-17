using ReunionesDeAmigos.Application.DTOs.LugaresExternos;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface ILugarExternoService
{
    Task<IReadOnlyCollection<LugarExternoDto>> BuscarAsync(
        BuscarLugaresExternosRequest request,
        CancellationToken cancellationToken);

    Task<LugarExternoDetalleDto> ObtenerDetalleAsync(
        string googlePlaceId,
        string? idioma,
        CancellationToken cancellationToken);
}
