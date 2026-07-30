using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Domain.Entities;

public sealed class Usuario
{
    private Usuario()
    {
    }

    private Usuario(
        Guid id,
        string nombre,
        string email,
        DateTimeOffset fechaCreacion)
    {
        Id = id;
        Nombre = ValidarTexto(nombre, 100, "El nombre del usuario es obligatorio.");
        Email = NormalizarEmail(email);
        FechaCreacion = fechaCreacion;
        Activo = true;
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public DateTimeOffset FechaCreacion { get; private set; }

    public bool Activo { get; private set; }

    public static Usuario Crear(
        string nombre,
        string email,
        DateTimeOffset fechaCreacion)
    {
        return new Usuario(Guid.NewGuid(), nombre, email, fechaCreacion);
    }

    public void CambiarNombre(string nombre)
    {
        Nombre = ValidarTexto(nombre, 100, "El nombre del usuario es obligatorio.");
    }

    public void Desactivar()
    {
        Activo = false;
    }

    public void Activar()
    {
        Activo = true;
    }

    private static string NormalizarEmail(string email)
    {
        var emailNormalizado = ValidarTexto(
            email,
            254,
            "El email del usuario es obligatorio.");

        if (!emailNormalizado.Contains('@', StringComparison.Ordinal))
        {
            throw new DomainException("El email del usuario no tiene un formato válido.");
        }

        return emailNormalizado.ToLowerInvariant();
    }

    private static string ValidarTexto(
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
}
