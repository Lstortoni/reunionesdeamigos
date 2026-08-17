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
    private const string DetailFieldMask =
        "id,displayName,formattedAddress,location,primaryType,googleMapsUri," +
        "websiteUri,nationalPhoneNumber,rating,userRatingCount,regularOpeningHours";

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

    public async Task<LugarExternoDetalleDto?> ObtenerDetalleAsync(
        string googlePlaceId,
        string? idioma,
        CancellationToken cancellationToken)
    {
        var languageCode = idioma ?? _options.DefaultLanguageCode;
        var url =
            $"v1/places/{Uri.EscapeDataString(googlePlaceId)}" +
            $"?languageCode={Uri.EscapeDataString(languageCode)}" +
            $"&regionCode={Uri.EscapeDataString(_options.DefaultRegionCode)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("X-Goog-Api-Key", _options.ApiKey);
        request.Headers.Add("X-Goog-FieldMask", DetailFieldMask);

        using var response = await httpClient.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "Google Places respondió con el código HTTP {StatusCode} al obtener el detalle.",
                (int)response.StatusCode);
            throw new ExternalServiceException(
                "No se pudo obtener el detalle del lugar externo.");
        }

        var lugar = await response.Content.ReadFromJsonAsync<GooglePlaceResponse>(
            cancellationToken: cancellationToken);

        return lugar is null || !EsLugarValido(lugar)
            ? null
            : ToDetalleDto(lugar);
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

    private static LugarExternoDetalleDto ToDetalleDto(GooglePlaceResponse lugar) =>
        new(
            lugar.Id!,
            lugar.DisplayName!.Text!,
            lugar.FormattedAddress,
            lugar.Location!.Latitude,
            lugar.Location.Longitude,
            lugar.PrimaryType,
            lugar.GoogleMapsUri,
            lugar.WebsiteUri,
            lugar.NationalPhoneNumber,
            lugar.Rating,
            lugar.UserRatingCount,
            lugar.RegularOpeningHours?.WeekdayDescriptions ?? []);
}
