using ReunionesDeAmigos.Application.DTOs.Lugares;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;

namespace ReunionesDeAmigos.Application.Services;

public sealed class LugarService : ILugarService
{
    private readonly ILugarRepository _lugarRepository;

    public LugarService(ILugarRepository lugarRepository)
    {
        _lugarRepository = lugarRepository;
    }

    public async Task<LugarDto> ObtenerPorIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken)
    {
        var lugar = await _lugarRepository.ObtenerPorIdAsync(
            lugarId,
            cancellationToken);

        if (lugar is null || !lugar.Activo)
        {
            throw new NotFoundException(
                "No se encontró el lugar.");
        }

        return LugarMapper.ToDto(lugar);
    }

    public async Task<IReadOnlyCollection<LugarDto>> BuscarAsync(
        BuscarLugaresRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var lugares = await _lugarRepository.BuscarAsync(
            NormalizarFiltro(request.Texto),
            request.Tipo,
            NormalizarFiltro(request.Barrio),
            request.CiudadId,
            cancellationToken);

        return lugares
            .Where(lugar => lugar.Activo)
            .Select(LugarMapper.ToDto)
            .ToArray();
    }

    private static string? NormalizarFiltro(string? valor)
    {
        var valorNormalizado = valor?.Trim();
        return string.IsNullOrWhiteSpace(valorNormalizado)
            ? null
            : valorNormalizado;
    }
}
