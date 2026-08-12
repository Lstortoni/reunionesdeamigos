using ReunionesDeAmigos.App.Models.Auth;

namespace ReunionesDeAmigos.App.Services;

public interface ISessionService
{
    UsuarioDto? UsuarioActual { get; }

    Task GuardarAsync(AutenticacionDto autenticacion);

    Task<string?> ObtenerAccessTokenAsync();

    void EstablecerUsuario(UsuarioDto usuario);

    void Cerrar();
}
