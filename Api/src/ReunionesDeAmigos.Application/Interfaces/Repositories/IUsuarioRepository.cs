using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObtenerPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken);

    Task<Usuario?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken);

    Task AgregarAsync(
        Usuario usuario,
        CancellationToken cancellationToken);
}
