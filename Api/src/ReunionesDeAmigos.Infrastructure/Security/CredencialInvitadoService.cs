using System.Security.Cryptography;
using System.Text;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Infrastructure.Security;

internal sealed class CredencialInvitadoService
    : ICredencialInvitadoService
{
    private const int CantidadBytesCredencial = 32;

    public CredencialInvitadoGenerada Generar()
    {
        var bytesCredencial = RandomNumberGenerator.GetBytes(
            CantidadBytesCredencial);
        var credencial = Convert.ToBase64String(bytesCredencial)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');

        var bytesHash = SHA256.HashData(
            Encoding.UTF8.GetBytes(credencial));
        var hash = Convert.ToHexString(bytesHash);

        return new CredencialInvitadoGenerada(
            credencial,
            hash);
    }
}
