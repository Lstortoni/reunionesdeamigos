using ReunionesDeAmigos.Application.DTOs.LugaresExternos;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.ExternalServices;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Services;

public sealed class LugarExternoService(
    ICiudadRepository ciudadRepository,
    IProveedorLugaresExternos proveedorLugaresExternos)
    : ILugarExternoService
{
    public async Task<LugarExternoDetalleDto> ObtenerDetalleAsync(
        string googlePlaceId,
        string? idioma,
        CancellationToken cancellationToken)
    {
        var id = Normalizar(googlePlaceId);
        if (id is null)
        {
            throw new ApplicationValidationException(
                "El identificador de Google Places es obligatorio.");
        }

        var detalle = await proveedorLugaresExternos.ObtenerDetalleAsync(
            id,
            Normalizar(idioma),
            cancellationToken);

        return detalle
            ?? throw new NotFoundException("No se encontró el lugar en Google Places.");
    }

    public async Task<IReadOnlyCollection<LugarExternoDto>> BuscarAsync(
        BuscarLugaresExternosRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CiudadId == Guid.Empty)
        {
            throw new ApplicationValidationException(
                "La ciudad es obligatoria.");
        }

        var texto = Normalizar(request.Texto);
        if (!request.Tipo.HasValue && texto is null)
        {
            throw new ApplicationValidationException(
                "Debe indicar un tipo de lugar o un texto de búsqueda.");
        }

        if (request.Cantidad is <= 0 or > 20)
        {
            throw new ApplicationValidationException(
                "La cantidad debe estar comprendida entre 1 y 20.");
        }

        var ciudad = await ciudadRepository.ObtenerPorIdAsync(
            request.CiudadId,
            cancellationToken);

        if (ciudad is null || !ciudad.Activa)
        {
            throw new NotFoundException("No se encontró la ciudad.");
        }

        var (descripcionTipo, tipoGoogle) = ObtenerTipoGoogle(request.Tipo);
        var busqueda = string.Join(
            ' ',
            new[] { descripcionTipo, texto }
                .Where(x => !string.IsNullOrWhiteSpace(x)));
        var ubicacion = string.Join(
            ", ",
            new[]
            {
                Normalizar(request.Barrio),
                ciudad.Nombre,
                ciudad.Provincia,
                ciudad.Pais
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return await proveedorLugaresExternos.BuscarAsync(
            new ConsultaLugaresExternos(
                $"{busqueda} en {ubicacion}",
                tipoGoogle,
                Normalizar(request.Idioma),
                request.Cantidad),
            cancellationToken);
    }

    private static (string? Descripcion, string? TipoGoogle) ObtenerTipoGoogle(
        TipoLugar? tipo) => tipo switch
        {
            TipoLugar.Restaurante => ("restaurantes", "restaurant"),
            TipoLugar.Bar => ("bares", "bar"),
            TipoLugar.Cafe => ("cafés", "cafe"),
            TipoLugar.Parrilla => ("parrillas", "barbecue_restaurant"),
            TipoLugar.Pizzeria => ("pizzerías", "pizza_restaurant"),
            TipoLugar.Cerveceria => ("cervecerías", "brewery"),
            _ => (null, null)
        };

    private static string? Normalizar(string? valor)
    {
        var normalizado = valor?.Trim();
        return string.IsNullOrWhiteSpace(normalizado) ? null : normalizado;
    }
}
