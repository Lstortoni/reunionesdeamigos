namespace ReunionesDeAmigos.Infrastructure.ExternalServices.GooglePlaces;

internal sealed class GooglePlacesOptions
{
    public const string SectionName = "GooglePlaces";

    public string BaseUrl { get; set; } = string.Empty;

    public string ApiKey { get; set; } = string.Empty;

    public string DefaultLanguageCode { get; set; } = string.Empty;

    public string DefaultRegionCode { get; set; } = string.Empty;

    public int DefaultPageSize { get; set; }

    public int TimeoutSeconds { get; set; }
}
