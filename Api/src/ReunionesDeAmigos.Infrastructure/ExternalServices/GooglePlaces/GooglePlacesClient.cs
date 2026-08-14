using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReunionesDeAmigos.Application.DTOs.LugaresExternos;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.ExternalServices;
using ReunionesDeAmigos.Infrastructure.ExternalServices.GooglePlaces.Models;

namespace ReunionesDeAmigos.Infrastructure.ExternalServices.GooglePlaces;

internal sealed class GooglePlacesClient(
    HttpClient httpClient,
    IOptions<GooglePlacesOptions> options,
    ILogger<GooglePlacesClient> logger)
    : IProveedorLugaresExternos
{
    private const string FieldMask =
        "places.id,places.displayName,places.formattedAddress," +
        "places.location,places.primaryType,places.googleMapsUri";

    private readonly GooglePlacesOptions _options = options.Value;

    public async Task<IReadOnlyCollection<LugarExternoDto>> BuscarAsync(
        ConsultaLugaresExternos consulta,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(consulta);

        var contenido = new GoogleTextSearchRequest(
            consulta.Texto,
            consulta.Tipo,
            consulta.Tipo is not null,
            consulta.Idioma ?? _options.DefaultLanguageCode,
            _options.DefaultRegionCode,
            consulta.Cantidad ?? _options.DefaultPageSize);

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "v1/places:searchText");
        request.Headers.Add("X-Goog-Api-Key", _options.ApiKey);
        request.Headers.Add("X-Goog-FieldMask", FieldMask);
        request.Content = JsonContent.Create(contenido);

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Google Places respondió con el código HTTP {StatusCode}.",
                (int)response.StatusCode);
            throw new ExternalServiceException(
                "No se pudo completar la búsqueda de lugares externos.");
        }

        var resultado = await response.Content
            .ReadFromJsonAsync<GoogleTextSearchResponse>(
                cancellationToken: cancellationToken);

        return resultado?.Places?
            .Where(EsLugarValido)
            .Select(ToDto)
            .ToArray()
            ?? [];
    }

    private static bool EsLugarValido(GooglePlaceResponse lugar) =>
        !string.IsNullOrWhiteSpace(lugar.Id) &&
        !string.IsNullOrWhiteSpace(lugar.DisplayName?.Text) &&
        lugar.Location is not null;

    private static LugarExternoDto ToDto(GooglePlaceResponse lugar) =>
        new(
            lugar.Id!,
            lugar.DisplayName!.Text!,
            lugar.FormattedAddress,
            lugar.Location!.Latitude,
            lugar.Location.Longitude,
            lugar.PrimaryType,
            lugar.GoogleMapsUri);
}
