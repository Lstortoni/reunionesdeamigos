using ReunionesDeAmigos.Application.DTOs.Propuestas;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Services;

public sealed class PropuestaService(
    ISalidaRepository salidaRepository,
    IUnitOfWork unitOfWork,
    IClock clock) : IPropuestaService
{
    public async Task<PropuestaDto> AgregarExternaAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        AgregarPropuestaExternaRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var salida = await ObtenerSalidaAsync(salidaId, cancellationToken);
        var propuesta = salida.AgregarPropuestaExterna(
            participanteSalidaId,
            request.GooglePlaceId,
            clock.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PropuestaMapper.ToDto(propuesta);
    }

    public async Task<PropuestaDto> AgregarManualAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        AgregarPropuestaManualRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var salida = await ObtenerSalidaAsync(salidaId, cancellationToken);
        var propuesta = salida.AgregarPropuestaManual(
            participanteSalidaId,
            request.Nombre,
            request.Descripcion,
            request.Direccion,
            clock.UtcNow);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return PropuestaMapper.ToDto(propuesta);
    }

    private async Task<Salida> ObtenerSalidaAsync(
        Guid salidaId,
        CancellationToken cancellationToken)
    {
        var salida = await salidaRepository.ObtenerPorIdAsync(
            salidaId,
            cancellationToken);

        return salida
            ?? throw new NotFoundException("No se encontró la salida.");
    }
}
