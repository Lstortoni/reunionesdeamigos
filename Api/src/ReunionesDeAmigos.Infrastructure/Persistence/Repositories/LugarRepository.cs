using Microsoft.EntityFrameworkCore;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Repositories;

internal sealed class LugarRepository(AppDbContext dbContext)
    : ILugarRepository
{
    public Task<Lugar?> ObtenerPorIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken) =>
        dbContext.Lugares
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == lugarId, cancellationToken);

    public async Task<IReadOnlyCollection<Lugar>> BuscarAsync(
        string? texto,
        TipoLugar? tipo,
        string? barrio,
        string? ciudad,
        CancellationToken cancellationToken)
    {
        var consulta = dbContext.Lugares
            .AsNoTracking()
            .Where(x => x.Activo);

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var patron = $"%{texto.Trim()}%";
            consulta = consulta.Where(x =>
                EF.Functions.ILike(x.Nombre, patron) ||
                EF.Functions.ILike(x.Direccion, patron));
        }

        if (tipo.HasValue)
        {
            consulta = consulta.Where(x => x.Tipo == tipo.Value);
        }

        if (!string.IsNullOrWhiteSpace(barrio))
        {
            var barrioNormalizado = barrio.Trim();
            consulta = consulta.Where(x =>
                x.Barrio != null && EF.Functions.ILike(x.Barrio, barrioNormalizado));
        }

        if (!string.IsNullOrWhiteSpace(ciudad))
        {
            var ciudadNormalizada = ciudad.Trim();
            consulta = consulta.Where(x =>
                EF.Functions.ILike(x.Ciudad, ciudadNormalizada));
        }

        return await consulta
            .OrderBy(x => x.Nombre)
            .ToArrayAsync(cancellationToken);
    }

    public async Task AgregarAsync(
        Lugar lugar,
        CancellationToken cancellationToken) =>
        await dbContext.Lugares.AddAsync(lugar, cancellationToken);
}
