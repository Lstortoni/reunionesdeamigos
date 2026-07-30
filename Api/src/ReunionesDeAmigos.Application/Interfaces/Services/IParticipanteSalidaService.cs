using ReunionesDeAmigos.Application.DTOs.Salidas;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IParticipanteSalidaService
{
    Task<ParticipanteSalidaDto> IngresarRegistradoAsync(
        string codigoAcceso,
        Guid usuarioId,
        CancellationToken cancellationToken);

    Task<IngresoInvitadoDto> IngresarComoInvitadoAsync(
        IngresarInvitadoRequest request,
        CancellationToken cancellationToken);
}
