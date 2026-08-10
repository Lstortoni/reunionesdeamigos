using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Domain.Entities;

public sealed class Ciudad
{
    private Ciudad()
    {
    }

    private Ciudad(Guid id, string nombre, string provincia, string pais)
    {
        Id = id;
        Nombre = ValidarTexto(nombre, 100, "El nombre de la ciudad es obligatorio.");
        Provincia = ValidarTexto(
            provincia,
            100,
            "La provincia o región es obligatoria.");
        Pais = ValidarTexto(pais, 100, "El país es obligatorio.");
        Activa = true;
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string Provincia { get; private set; } = string.Empty;

    public string Pais { get; private set; } = string.Empty;

    public bool Activa { get; private set; }

    public static Ciudad Crear(string nombre, string provincia, string pais) =>
        new(Guid.NewGuid(), nombre, provincia, pais);

    public void Activar() => Activa = true;

    public void Desactivar() => Activa = false;

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
