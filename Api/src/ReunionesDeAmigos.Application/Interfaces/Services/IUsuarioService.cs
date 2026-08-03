using ReunionesDeAmigos.Application.DTOs.Usuarios;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IUsuarioService
{
    Task<UsuarioDto> ObtenerPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken);
}
