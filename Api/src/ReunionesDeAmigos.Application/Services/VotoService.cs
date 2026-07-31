using ReunionesDeAmigos.Application.DTOs.Votos;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Services;

public sealed class VotoService : IVotoService
{
    private readonly ISalidaRepository _salidaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public VotoService(
        ISalidaRepository salidaRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _salidaRepository = salidaRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<VotoDto> RegistrarAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        Guid propuestaId,
        CancellationToken cancellationToken)
    {
        var salida = await ObtenerSalidaAsync(
            salidaId,
            cancellationToken);

        var voto = salida.RegistrarVoto(
            participanteSalidaId,
            propuestaId,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return VotoMapper.ToDto(voto);
    }

    public async Task<ResultadoVotacionDto> ObtenerResultadoAsync(
        Guid salidaId,
        CancellationToken cancellationToken)
    {
        var salida = await ObtenerSalidaAsync(
            salidaId,
            cancellationToken);
        var resultado = salida.ObtenerResultado(_clock.UtcNow);

        return VotoMapper.ToDto(resultado);
    }

    private async Task<Salida> ObtenerSalidaAsync(
        Guid salidaId,
        CancellationToken cancellationToken)
    {
        var salida = await _salidaRepository.ObtenerPorIdAsync(
            salidaId,
            cancellationToken);

        if (salida is null)
        {
            throw new NotFoundException(
                "No se encontró la salida.");
        }

        return salida;
    }
}
