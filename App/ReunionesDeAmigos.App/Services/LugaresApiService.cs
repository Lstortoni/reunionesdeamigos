using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ReunionesDeAmigos.App.Models.Api;
using ReunionesDeAmigos.App.Models.Lugares;

namespace ReunionesDeAmigos.App.Services;

public sealed class LugaresApiService(
    HttpClient httpClient,
    ISessionService sessionService) : ILugaresApiService
{
    public async Task<IReadOnlyCollection<LugarDto>> BuscarAsync(
        string? texto,
        TipoLugar? tipo,
        Guid? ciudadId,
        CancellationToken cancellationToken = default)
    {
        var parametros = new List<string>();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            parametros.Add($"texto={Uri.EscapeDataString(texto.Trim())}");
        }

        if (tipo.HasValue)
        {
            parametros.Add($"tipo={(int)tipo.Value}");
        }

        if (ciudadId.HasValue)
        {
            parametros.Add($"ciudadId={ciudadId.Value}");
        }

        var url = parametros.Count == 0
            ? "api/lugares"
            : $"api/lugares?{string.Join("&", parametros)}";

        using var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException("No se pudieron obtener los lugares.");
        }

        return await response.Content.ReadFromJsonAsync<LugarDto[]>(
                   cancellationToken: cancellationToken) ?? [];
    }

    public async Task<IReadOnlyCollection<LugarExternoDto>> BuscarExternosAsync(
        Guid ciudadId,
        TipoLugar? tipo,
        string? barrio,
        string? texto,
        string idioma,
        int cantidad,
        CancellationToken cancellationToken = default)
    {
        var token = await sessionService.ObtenerAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SesionVencidaException();
        }

        var parametros = new List<string>
        {
            $"ciudadId={ciudadId}",
            $"idioma={Uri.EscapeDataString(idioma)}",
            $"cantidad={cantidad}"
        };

        if (tipo.HasValue)
        {
            parametros.Add($"tipo={(int)tipo.Value}");
        }

        if (!string.IsNullOrWhiteSpace(barrio))
        {
            parametros.Add($"barrio={Uri.EscapeDataString(barrio.Trim())}");
        }

        if (!string.IsNullOrWhiteSpace(texto))
        {
            parametros.Add($"texto={Uri.EscapeDataString(texto.Trim())}");
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/lugares/externos?{string.Join("&", parametros)}");
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
            throw new ApiException(
                problem?.Detail ?? "No se pudieron buscar lugares en Google.");
        }

        return await response.Content.ReadFromJsonAsync<LugarExternoDto[]>(
                   cancellationToken: cancellationToken) ?? [];
    }

    public async Task<LugarExternoDetalleDto> ObtenerDetalleExternoAsync(
        string googlePlaceId,
        string idioma,
        CancellationToken cancellationToken = default)
    {
        var token = await sessionService.ObtenerAccessTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new SesionVencidaException();
        }

        var url =
            $"api/lugares/externos/{Uri.EscapeDataString(googlePlaceId)}" +
            $"?idioma={Uri.EscapeDataString(idioma)}";
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
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
            throw new ApiException(
                problem?.Detail ?? "No se pudo obtener el detalle del lugar.");
        }

        return await response.Content.ReadFromJsonAsync<LugarExternoDetalleDto>(
                   cancellationToken: cancellationToken)
               ?? throw new ApiException("La API devolvió una respuesta vacía.");
    }
}
