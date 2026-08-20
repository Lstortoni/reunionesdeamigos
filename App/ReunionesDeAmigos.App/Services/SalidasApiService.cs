using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ReunionesDeAmigos.App.Models.Api;
using ReunionesDeAmigos.App.Models.Salidas;

namespace ReunionesDeAmigos.App.Services;

public sealed class SalidasApiService(
    HttpClient httpClient,
    ISessionService sessionService) : ISalidasApiService
{
    public async Task<SalidaCreadaDto> CrearAsync(
        CrearSalidaRequest salida,
        CancellationToken cancellationToken = default)
    {
        var token = await sessionService.ObtenerAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SesionVencidaException();
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "api/salidas")
        {
            Content = JsonContent.Create(salida)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        using var response = await httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new SesionVencidaException();
        }

        if (!response.IsSuccessStatusCode)
        {
            var problem = await response.Content.ReadFromJsonAsync<ApiProblemDetails>(
                cancellationToken: cancellationToken);
            throw new ApiException(problem?.Detail ?? "No se pudo crear la salida.");
        }

        return await response.Content.ReadFromJsonAsync<SalidaCreadaDto>(
                   cancellationToken: cancellationToken)
               ?? throw new ApiException("La API devolvió una respuesta vacía.");
    }

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

    public async Task<SalidaCreadaDto> ObtenerPorIdAsync(
        Guid salidaId,
        CancellationToken cancellationToken = default)
    {
        var token = await sessionService.ObtenerAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token)) throw new SesionVencidaException();

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/salidas/{salidaId}");
        request.Headers.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            token);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
            throw new SesionVencidaException();
        if (response.StatusCode == HttpStatusCode.NotFound)
            throw new ApiException("No se encontró la salida.");
        if (!response.IsSuccessStatusCode)
            throw new ApiException("No se pudo obtener el detalle de la salida.");

        return await response.Content.ReadFromJsonAsync<SalidaCreadaDto>(
                   cancellationToken: cancellationToken)
               ?? throw new ApiException("La API devolvió una respuesta vacía.");
    }
}
