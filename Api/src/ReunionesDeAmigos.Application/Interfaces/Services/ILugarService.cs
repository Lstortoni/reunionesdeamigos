using ReunionesDeAmigos.Application.DTOs.Lugares;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface ILugarService
{
    Task<LugarDto> ObtenerPorIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<LugarDto>> BuscarAsync(
        BuscarLugaresRequest request,
        CancellationToken cancellationToken);
}
