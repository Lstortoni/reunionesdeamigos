using Microsoft.EntityFrameworkCore;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Persistence.Repositories;

internal sealed class UsuarioRepository(AppDbContext dbContext)
    : IUsuarioRepository
{
    public Task<Usuario?> ObtenerPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken) =>
        dbContext.Usuarios.SingleOrDefaultAsync(
            x => x.Id == usuarioId,
            cancellationToken);

    public Task<Usuario?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var emailNormalizado = email.Trim().ToLowerInvariant();
        return dbContext.Usuarios.SingleOrDefaultAsync(
            x => x.Email == emailNormalizado,
            cancellationToken);
    }

    public async Task AgregarAsync(
        Usuario usuario,
        CancellationToken cancellationToken) =>
        await dbContext.Usuarios.AddAsync(usuario, cancellationToken);
}
