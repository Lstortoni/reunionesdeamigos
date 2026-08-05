using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Infrastructure.Persistence;
using ReunionesDeAmigos.Infrastructure.Persistence.Repositories;
using ReunionesDeAmigos.Infrastructure.Security;
using ReunionesDeAmigos.Infrastructure.Time;

namespace ReunionesDeAmigos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException(
                "No se configuró la cadena de conexión 'Postgres'.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ISalidaRepository, SalidaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ILugarRepository, LugarRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ICodigoAccesoGenerator, CodigoAccesoGenerator>();
        services.AddSingleton<ICredencialInvitadoService, CredencialInvitadoService>();
        services.Configure<JwtOptions>(options =>
        {
            options.Issuer = configuration["Jwt:Issuer"] ?? string.Empty;
            options.Audience = configuration["Jwt:Audience"] ?? string.Empty;
            options.SigningKey = configuration["Jwt:SigningKey"] ?? string.Empty;
            _ = int.TryParse(
                configuration["Jwt:ExpirationMinutes"],
                out var expirationMinutes);
            options.ExpirationMinutes = expirationMinutes;
        });
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IAccessTokenGenerator, JwtAccessTokenGenerator>();

        return services;
    }
}
