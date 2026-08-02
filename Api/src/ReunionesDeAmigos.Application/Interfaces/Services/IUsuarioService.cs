using ReunionesDeAmigos.Application.DTOs.Usuarios;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IUsuarioService
{
    Task<UsuarioDto> CrearAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<UsuarioDto> ObtenerPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken);
}
