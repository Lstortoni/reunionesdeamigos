using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Interfaces.Repositories;

public interface ICiudadRepository
{
    Task<IReadOnlyCollection<Ciudad>> ObtenerActivasAsync(
        CancellationToken cancellationToken);
}
