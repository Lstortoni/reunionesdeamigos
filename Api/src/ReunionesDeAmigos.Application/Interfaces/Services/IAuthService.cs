using ReunionesDeAmigos.Application.DTOs.Auth;
using ReunionesDeAmigos.Application.DTOs.Usuarios;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IAuthService
{
    Task<UsuarioDto> RegistrarAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<LoginDto> IniciarSesionAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}
