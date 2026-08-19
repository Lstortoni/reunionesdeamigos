using Microsoft.Extensions.Options;
using ReunionesDeAmigos.Application.Interfaces.Services;

namespace ReunionesDeAmigos.Infrastructure.Links;

internal sealed class EnlaceInvitacionGenerator(
    IOptions<AppLinksOptions> options) : IEnlaceInvitacionGenerator
{
    private readonly AppLinksOptions _options = options.Value;

    public string Generar(string codigoAcceso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(codigoAcceso);

        var baseUrl = _options.PublicBaseUrl.TrimEnd('/');
        return $"{baseUrl}/unirse/{Uri.EscapeDataString(codigoAcceso)}";
    }
}
