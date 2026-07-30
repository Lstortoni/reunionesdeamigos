using ReunionesDeAmigos.Domain.Enums;
using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Domain.Entities;

public sealed class Lugar
{
    private Lugar()
    {
    }

    private Lugar(
        Guid id,
        string nombre,
        string? descripcion,
        string direccion,
        string? barrio,
        string ciudad,
        TipoLugar tipo,
        decimal? latitud,
        decimal? longitud)
    {
        Id = id;
        Nombre = ValidarTextoObligatorio(nombre, 150, "El nombre del lugar es obligatorio.");
        Descripcion = ValidarTextoOpcional(descripcion, 1000);
        Direccion = ValidarTextoObligatorio(
            direccion,
            250,
            "La dirección del lugar es obligatoria.");
        Barrio = ValidarTextoOpcional(barrio, 100);
        Ciudad = ValidarTextoObligatorio(ciudad, 100, "La ciudad del lugar es obligatoria.");
        Tipo = tipo;
        ValidarCoordenadas(latitud, longitud);
        Latitud = latitud;
        Longitud = longitud;
        Activo = true;
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public string Direccion { get; private set; } = string.Empty;

    public string? Barrio { get; private set; }

    public string Ciudad { get; private set; } = string.Empty;

    public TipoLugar Tipo { get; private set; }

    public decimal? Latitud { get; private set; }

    public decimal? Longitud { get; private set; }

    public bool Activo { get; private set; }

    public static Lugar Crear(
        string nombre,
        string? descripcion,
        string direccion,
        string? barrio,
        string ciudad,
        TipoLugar tipo,
        decimal? latitud = null,
        decimal? longitud = null)
    {
        return new Lugar(
            Guid.NewGuid(),
            nombre,
            descripcion,
            direccion,
            barrio,
            ciudad,
            tipo,
            latitud,
            longitud);
    }

    public void Desactivar()
    {
        Activo = false;
    }

    public void Activar()
    {
        Activo = true;
    }

    private static void ValidarCoordenadas(decimal? latitud, decimal? longitud)
    {
        if (latitud.HasValue != longitud.HasValue)
        {
            throw new DomainException(
                "La latitud y la longitud deben informarse juntas.");
        }

        if (latitud is < -90 or > 90)
        {
            throw new DomainException("La latitud debe estar entre -90 y 90.");
        }

        if (longitud is < -180 or > 180)
        {
            throw new DomainException("La longitud debe estar entre -180 y 180.");
        }
    }

    private static string ValidarTextoObligatorio(
        string valor,
        int longitudMaxima,
        string mensajeObligatorio)
    {
        var valorNormalizado = valor?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(valorNormalizado))
        {
            throw new DomainException(mensajeObligatorio);
        }

        if (valorNormalizado.Length > longitudMaxima)
        {
            throw new DomainException(
                $"El valor no puede superar los {longitudMaxima} caracteres.");
        }

        return valorNormalizado;
    }

    private static string? ValidarTextoOpcional(string? valor, int longitudMaxima)
    {
        var valorNormalizado = valor?.Trim();

        if (string.IsNullOrWhiteSpace(valorNormalizado))
        {
            return null;
        }

        if (valorNormalizado.Length > longitudMaxima)
        {
            throw new DomainException(
                $"El valor no puede superar los {longitudMaxima} caracteres.");
        }

        return valorNormalizado;
    }
}
