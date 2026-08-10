using Microsoft.Maui.Storage;
using ReunionesDeAmigos.App.Models.Auth;

namespace ReunionesDeAmigos.App.Services;

public sealed class SessionService : ISessionService
{
    private const string AccessTokenKey = "auth_access_token";

    public UsuarioDto? UsuarioActual { get; private set; }

    public async Task GuardarAsync(AutenticacionDto autenticacion)
    {
        ArgumentNullException.ThrowIfNull(autenticacion);

        await SecureStorage.Default.SetAsync(
            AccessTokenKey,
            autenticacion.AccessToken);
        UsuarioActual = autenticacion.Usuario;
    }

    public Task<string?> ObtenerAccessTokenAsync() =>
        SecureStorage.Default.GetAsync(AccessTokenKey);

    public void Cerrar()
    {
        SecureStorage.Default.Remove(AccessTokenKey);
        UsuarioActual = null;
    }
}
