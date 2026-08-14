using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Interfaces.Repositories;

public interface ICiudadRepository
{
    Task<Ciudad?> ObtenerPorIdAsync(
        Guid ciudadId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Ciudad>> ObtenerActivasAsync(
        CancellationToken cancellationToken);
}
