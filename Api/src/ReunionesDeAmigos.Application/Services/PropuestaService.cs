using ReunionesDeAmigos.Application.DTOs.Propuestas;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Services;

public sealed class PropuestaService : IPropuestaService
{
    private readonly ISalidaRepository _salidaRepository;
    private readonly ILugarRepository _lugarRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public PropuestaService(
        ISalidaRepository salidaRepository,
        ILugarRepository lugarRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _salidaRepository = salidaRepository;
        _lugarRepository = lugarRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<PropuestaDto> AgregarDeCatalogoAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        Guid lugarId,
        CancellationToken cancellationToken)
    {
        var salida = await ObtenerSalidaAsync(
            salidaId,
            cancellationToken);
        var lugar = await _lugarRepository.ObtenerPorIdAsync(
            lugarId,
            cancellationToken);

        if (lugar is null)
        {
            throw new NotFoundException(
                "No se encontró el lugar.");
        }

        var propuesta = salida.AgregarPropuestaDeCatalogo(
            participanteSalidaId,
            lugar,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return PropuestaMapper.ToDto(propuesta);
    }

    public async Task<PropuestaDto> AgregarManualAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        AgregarPropuestaManualRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var salida = await ObtenerSalidaAsync(
            salidaId,
            cancellationToken);

        var propuesta = salida.AgregarPropuestaManual(
            participanteSalidaId,
            request.Nombre,
            request.Descripcion,
            request.Direccion,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return PropuestaMapper.ToDto(propuesta);
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
