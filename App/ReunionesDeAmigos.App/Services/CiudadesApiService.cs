using System.Net.Http.Json;
using ReunionesDeAmigos.App.Models.Ciudades;

namespace ReunionesDeAmigos.App.Services;

public sealed class CiudadesApiService(HttpClient httpClient) : ICiudadesApiService
{
    public async Task<IReadOnlyCollection<CiudadDto>> ObtenerAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync("api/ciudades", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException("No se pudieron obtener las ciudades.");
        }

        return await response.Content.ReadFromJsonAsync<CiudadDto[]>(
                   cancellationToken: cancellationToken) ?? [];
    }
}
