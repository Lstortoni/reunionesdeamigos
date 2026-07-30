using ReunionesDeAmigos.Application.DTOs.Votos;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IVotoService
{
    Task<VotoDto> RegistrarAsync(
        Guid salidaId,
        Guid participanteSalidaId,
        Guid propuestaId,
        CancellationToken cancellationToken);

    Task<ResultadoVotacionDto> ObtenerResultadoAsync(
        Guid salidaId,
        CancellationToken cancellationToken);
}
