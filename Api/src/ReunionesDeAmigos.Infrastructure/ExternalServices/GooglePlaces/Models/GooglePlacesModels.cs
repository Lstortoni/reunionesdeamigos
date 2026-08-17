using System.Text.Json.Serialization;

namespace ReunionesDeAmigos.Infrastructure.ExternalServices.GooglePlaces.Models;

internal sealed record GoogleTextSearchRequest(
    [property: JsonPropertyName("textQuery")] string TextQuery,
    [property: JsonPropertyName("includedType")] string? IncludedType,
    [property: JsonPropertyName("strictTypeFiltering")] bool StrictTypeFiltering,
    [property: JsonPropertyName("languageCode")] string LanguageCode,
    [property: JsonPropertyName("regionCode")] string RegionCode,
    [property: JsonPropertyName("pageSize")] int PageSize);

internal sealed record GoogleTextSearchResponse(
    [property: JsonPropertyName("places")]
    IReadOnlyCollection<GooglePlaceResponse>? Places);

internal sealed record GooglePlaceResponse(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("displayName")] GoogleLocalizedText? DisplayName,
    [property: JsonPropertyName("formattedAddress")] string? FormattedAddress,
    [property: JsonPropertyName("location")] GoogleLocation? Location,
    [property: JsonPropertyName("primaryType")] string? PrimaryType,
    [property: JsonPropertyName("googleMapsUri")] string? GoogleMapsUri,
    [property: JsonPropertyName("websiteUri")] string? WebsiteUri = null,
    [property: JsonPropertyName("nationalPhoneNumber")] string? NationalPhoneNumber = null,
    [property: JsonPropertyName("rating")] decimal? Rating = null,
    [property: JsonPropertyName("userRatingCount")] int? UserRatingCount = null,
    [property: JsonPropertyName("regularOpeningHours")] GoogleOpeningHours? RegularOpeningHours = null);

internal sealed record GoogleLocalizedText(
    [property: JsonPropertyName("text")] string? Text);

internal sealed record GoogleLocation(
    [property: JsonPropertyName("latitude")] decimal Latitude,
    [property: JsonPropertyName("longitude")] decimal Longitude);

internal sealed record GoogleOpeningHours(
    [property: JsonPropertyName("weekdayDescriptions")]
    IReadOnlyCollection<string>? WeekdayDescriptions);
