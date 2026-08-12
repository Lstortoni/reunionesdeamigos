using ReunionesDeAmigos.App.Models.Auth;

namespace ReunionesDeAmigos.App.Services;

public interface IAuthApiService
{
    Task<AutenticacionDto> IniciarSesionAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default);

    Task<UsuarioDto?> ObtenerUsuarioActualAsync(
        string accessToken,
        CancellationToken cancellationToken = default);
}
