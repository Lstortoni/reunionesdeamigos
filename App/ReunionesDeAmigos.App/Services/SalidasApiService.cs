using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public sealed class SalidasApiService(
    HttpClient httpClient,
    ISessionService sessionService) : ISalidasApiService
{
    public async Task<IReadOnlyCollection<SalidaResumenDto>> ObtenerMiasAsync(
        CancellationToken cancellationToken = default)
    {
        var token = await sessionService.ObtenerAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SesionVencidaException();
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            "api/salidas/mias");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token);

        using var response = await httpClient.SendAsync(
            request,
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new SesionVencidaException();
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException("No se pudieron obtener tus salidas.");
        }

        return await response.Content
                   .ReadFromJsonAsync<SalidaResumenDto[]>(
                       cancellationToken: cancellationToken)
               ?? [];
    }
}
