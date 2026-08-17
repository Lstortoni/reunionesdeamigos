using ReunionesDeAmigos.Application.DTOs.Propuestas;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IPropuestaService
{
    Task<PropuestaDto> AgregarExternaAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        AgregarPropuestaExternaRequest request,
        CancellationToken cancellationToken);

    Task<PropuestaDto> AgregarManualAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        AgregarPropuestaManualRequest request,
        CancellationToken cancellationToken);
}
