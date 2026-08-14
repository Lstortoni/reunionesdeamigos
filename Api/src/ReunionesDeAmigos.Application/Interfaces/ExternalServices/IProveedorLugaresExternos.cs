using ReunionesDeAmigos.Application.DTOs.LugaresExternos;

namespace ReunionesDeAmigos.Application.Interfaces.ExternalServices;

public interface IProveedorLugaresExternos
{
    Task<IReadOnlyCollection<LugarExternoDto>> BuscarAsync(
        ConsultaLugaresExternos consulta,
        CancellationToken cancellationToken);
}

public sealed record ConsultaLugaresExternos(
    string Texto,
    string? Tipo,
    string? Idioma,
    int? Cantidad);
