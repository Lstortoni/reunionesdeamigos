using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Infrastructure.Security;

internal sealed class JwtAccessTokenGenerator : IAccessTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly IClock _clock;

    public JwtAccessTokenGenerator(
        IOptions<JwtOptions> options,
        IClock clock)
    {
        _options = options.Value;
        _clock = clock;
        ValidarConfiguracion(_options);
    }

    public AccessTokenGenerado Generar(Usuario usuario)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        var fechaEmision = _clock.UtcNow;
        var fechaExpiracion = fechaEmision.AddMinutes(
            _options.ExpirationMinutes);
        var clave = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_options.SigningKey));
        var credenciales = new SigningCredentials(
            clave,
            SecurityAlgorithms.HmacSha256);
        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = usuario.Id.ToString(),
            [JwtRegisteredClaimNames.Email] = usuario.Email,
            [JwtRegisteredClaimNames.Name] = usuario.Nombre,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
        };
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            IssuedAt = fechaEmision.UtcDateTime,
            NotBefore = fechaEmision.UtcDateTime,
            Expires = fechaExpiracion.UtcDateTime,
            Claims = claims,
            SigningCredentials = credenciales
        };
        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        return new AccessTokenGenerado(token, fechaExpiracion);
    }

    private static void ValidarConfiguracion(JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer) ||
            string.IsNullOrWhiteSpace(options.Audience) ||
            string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new InvalidOperationException(
                "La configuración JWT está incompleta.");
        }

        if (Encoding.UTF8.GetByteCount(options.SigningKey) < 32)
        {
            throw new InvalidOperationException(
                "La clave de firma JWT debe tener al menos 32 bytes.");
        }

        if (options.ExpirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "La duración del JWT debe ser mayor que cero.");
        }
    }
}
