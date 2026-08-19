using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.ExternalServices;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Infrastructure.ExternalServices.GooglePlaces;
using ReunionesDeAmigos.Infrastructure.Persistence;
using ReunionesDeAmigos.Infrastructure.Persistence.Repositories;
using ReunionesDeAmigos.Infrastructure.Security;
using ReunionesDeAmigos.Infrastructure.Time;
using ReunionesDeAmigos.Infrastructure.Links;

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

        services.Configure<GooglePlacesOptions>(options =>
        {
            options.BaseUrl = configuration["GooglePlaces:BaseUrl"]
                ?? string.Empty;
            options.ApiKey = configuration["GooglePlaces:ApiKey"]
                ?? string.Empty;
            options.DefaultLanguageCode =
                configuration["GooglePlaces:DefaultLanguageCode"]
                ?? string.Empty;
            options.DefaultRegionCode =
                configuration["GooglePlaces:DefaultRegionCode"]
                ?? string.Empty;
            _ = int.TryParse(
                configuration["GooglePlaces:DefaultPageSize"],
                out var defaultPageSize);
            options.DefaultPageSize = defaultPageSize;
            _ = int.TryParse(
                configuration["GooglePlaces:TimeoutSeconds"],
                out var timeoutSeconds);
            options.TimeoutSeconds = timeoutSeconds;
        });
        var googlePlacesBaseUrl =
            configuration["GooglePlaces:BaseUrl"]
            ?? throw new InvalidOperationException(
                "No se configuró GooglePlaces:BaseUrl.");
        var googlePlacesTimeout = int.TryParse(
            configuration["GooglePlaces:TimeoutSeconds"],
            out var configuredTimeout)
            ? configuredTimeout
            : throw new InvalidOperationException(
                "GooglePlaces:TimeoutSeconds no es válido.");
        _ = configuration["GooglePlaces:ApiKey"]
            ?? throw new InvalidOperationException(
                "No se configuró GooglePlaces:ApiKey.");
        services.AddHttpClient<IProveedorLugaresExternos, GooglePlacesClient>(
            client =>
            {
                client.BaseAddress = new Uri(googlePlacesBaseUrl);
                client.Timeout = TimeSpan.FromSeconds(googlePlacesTimeout);
            });

        services.AddScoped<ISalidaRepository, SalidaRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ILugarRepository, LugarRepository>();
        services.AddScoped<ICiudadRepository, CiudadRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IClock, SystemClock>();
        services.AddSingleton<ICodigoAccesoGenerator, CodigoAccesoGenerator>();
        services.Configure<AppLinksOptions>(options =>
        {
            options.PublicBaseUrl = configuration["AppLinks:PublicBaseUrl"]
                ?? string.Empty;
        });
        var publicBaseUrl = configuration["AppLinks:PublicBaseUrl"];
        if (!Uri.TryCreate(publicBaseUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException(
                "AppLinks:PublicBaseUrl debe ser una URL absoluta válida.");
        }
        services.AddSingleton<IEnlaceInvitacionGenerator, EnlaceInvitacionGenerator>();
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
