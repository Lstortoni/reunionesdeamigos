using Microsoft.AspNetCore.Identity;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Infrastructure.Security;

internal sealed class PasswordHasher : IPasswordHasher
{
    private static readonly object UsuarioMarcador = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string GenerarHash(string password)
    {
        return _hasher.HashPassword(UsuarioMarcador, password);
    }

    public bool Verificar(
        string password,
        string passwordHash)
    {
        var resultado = _hasher.VerifyHashedPassword(
            UsuarioMarcador,
            passwordHash,
            password);

        return resultado != PasswordVerificationResult.Failed;
    }
}
