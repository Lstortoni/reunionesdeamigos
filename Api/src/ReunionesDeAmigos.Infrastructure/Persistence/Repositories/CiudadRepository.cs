using Microsoft.EntityFrameworkCore;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Repositories;

internal sealed class CiudadRepository(AppDbContext dbContext)
    : ICiudadRepository
{
    public async Task<Ciudad?> ObtenerPorIdAsync(
        Guid ciudadId,
        CancellationToken cancellationToken) =>
        await dbContext.Ciudades
            .AsNoTracking()
            .SingleOrDefaultAsync(
                x => x.Id == ciudadId,
                cancellationToken);

    public async Task<IReadOnlyCollection<Ciudad>> ObtenerActivasAsync(
        CancellationToken cancellationToken) =>
        await dbContext.Ciudades
            .AsNoTracking()
            .Where(x => x.Activa)
            .OrderBy(x => x.Pais)
            .ThenBy(x => x.Provincia)
            .ThenBy(x => x.Nombre)
            .ToArrayAsync(cancellationToken);
}
