using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Infrastructure.Persistence;
using ReunionesDeAmigos.Infrastructure.Persistence.Repositories;

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

        return services;
    }
}
