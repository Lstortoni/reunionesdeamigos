using ReunionesDeAmigos.Application.DTOs.Auth;
using ReunionesDeAmigos.Application.DTOs.Usuarios;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AutenticacionDto> RegistrarAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken);

    Task<AutenticacionDto> IniciarSesionAsync(
        LoginRequest request,
        CancellationToken cancellationToken);
}
