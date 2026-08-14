using Microsoft.Extensions.DependencyInjection;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Services;

namespace ReunionesDeAmigos.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddScoped<ISalidaService, SalidaService>();
        services.AddScoped<IParticipanteSalidaService, ParticipanteSalidaService>();
        services.AddScoped<IPropuestaService, PropuestaService>();
        services.AddScoped<IVotoService, VotoService>();
        services.AddScoped<ILugarService, LugarService>();
        services.AddScoped<ILugarExternoService, LugarExternoService>();
        services.AddScoped<ICiudadService, CiudadService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
