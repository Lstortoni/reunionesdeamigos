using ReunionesDeAmigos.Application.DTOs.Propuestas;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IPropuestaService
{
    Task<PropuestaDto> AgregarDeCatalogoAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        Guid lugarId,
        CancellationToken cancellationToken);

    Task<PropuestaDto> AgregarManualAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        AgregarPropuestaManualRequest request,
        CancellationToken cancellationToken);
}
