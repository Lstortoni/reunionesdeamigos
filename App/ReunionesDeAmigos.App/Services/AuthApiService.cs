using System.Net.Http.Json;
using ReunionesDeAmigos.App.Models.Api;
using ReunionesDeAmigos.App.Models.Auth;

namespace ReunionesDeAmigos.App.Services;

public sealed class AuthApiService(HttpClient httpClient) : IAuthApiService
{
    public async Task<AutenticacionDto> IniciarSesionAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/auth/login",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content
                .ReadFromJsonAsync<ApiProblemDetails>(
                    cancellationToken: cancellationToken);

            throw new ApiException(
                problem?.Detail ?? "No se pudo iniciar sesión.");
        }

        return await response.Content.ReadFromJsonAsync<AutenticacionDto>(
                   cancellationToken: cancellationToken)
               ?? throw new ApiException(
                   "La API devolvió una respuesta vacía.");
    }
}
