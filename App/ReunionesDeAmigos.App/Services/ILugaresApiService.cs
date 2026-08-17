using ReunionesDeAmigos.App.Models.Lugares;

namespace ReunionesDeAmigos.App.Services;

public interface ILugaresApiService
{
    Task<IReadOnlyCollection<LugarDto>> BuscarAsync(
        string? texto,
        TipoLugar? tipo,
        Guid? ciudadId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<LugarExternoDto>> BuscarExternosAsync(
        Guid ciudadId,
        TipoLugar? tipo,
        string? barrio,
        string? texto,
        string idioma,
        int cantidad,
        CancellationToken cancellationToken = default);

    Task<LugarExternoDetalleDto> ObtenerDetalleExternoAsync(
        string googlePlaceId,
        string idioma,
        CancellationToken cancellationToken = default);
}
