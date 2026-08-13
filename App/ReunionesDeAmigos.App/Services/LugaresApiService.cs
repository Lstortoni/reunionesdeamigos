using System.Net.Http.Json;
using ReunionesDeAmigos.App.Models.Lugares;

namespace ReunionesDeAmigos.App.Services;

public sealed class LugaresApiService(HttpClient httpClient) : ILugaresApiService
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
}
