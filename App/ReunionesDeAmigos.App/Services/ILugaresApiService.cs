using ReunionesDeAmigos.App.Models.Lugares;

namespace ReunionesDeAmigos.App.Services;

public interface ILugaresApiService
{
    Task<IReadOnlyCollection<LugarDto>> BuscarAsync(
        string? texto,
        TipoLugar? tipo,
        Guid? ciudadId,
        CancellationToken cancellationToken = default);
}
