using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Interfaces.Repositories;

public interface ISalidaRepository
{
    Task<Salida?> ObtenerPorIdAsync(
        Guid salidaId,
        CancellationToken cancellationToken);

    Task<Salida?> ObtenerPorCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken);

    Task<bool> ExisteCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken);

    Task AgregarAsync(
        Salida salida,
        CancellationToken cancellationToken);
}
