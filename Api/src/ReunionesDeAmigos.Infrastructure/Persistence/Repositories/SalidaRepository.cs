using Microsoft.EntityFrameworkCore;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Repositories;

internal sealed class SalidaRepository(AppDbContext dbContext)
    : ISalidaRepository
{
    public Task<Salida?> ObtenerPorIdAsync(
        Guid salidaId,
        CancellationToken cancellationToken) =>
        ConsultaAgregado().SingleOrDefaultAsync(
            x => x.Id == salidaId,
            cancellationToken);

    public Task<Salida?> ObtenerPorCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken)
    {
        var codigoNormalizado = codigoAcceso.Trim().ToUpperInvariant();
        return ConsultaAgregado().SingleOrDefaultAsync(
            x => x.CodigoAcceso == codigoNormalizado,
            cancellationToken);
    }

    public Task<bool> ExisteCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken)
    {
        var codigoNormalizado = codigoAcceso.Trim().ToUpperInvariant();
        return dbContext.Salidas.AnyAsync(
            x => x.CodigoAcceso == codigoNormalizado,
            cancellationToken);
    }

    public async Task AgregarAsync(
        Salida salida,
        CancellationToken cancellationToken) =>
        await dbContext.Salidas.AddAsync(salida, cancellationToken);

    private IQueryable<Salida> ConsultaAgregado() =>
        dbContext.Salidas
            .Include(x => x.Participantes)
            .Include(x => x.Propuestas)
            .Include(x => x.Votos)
            .AsSplitQuery();
}
