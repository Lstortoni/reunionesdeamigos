using ReunionesDeAmigos.Domain.Enums;
using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Domain.Entities;

public sealed class Propuesta
{
    private Propuesta()
    {
    }

    private Propuesta(
        Guid id,
        Guid salidaId,
        Guid participanteSalidaId,
        TipoPropuesta tipo,
        Guid? lugarId,
        string? nombreManual,
        string? descripcionManual,
        string? direccionManual,
        DateTimeOffset fechaCreacion)
    {
        Id = id;
        SalidaId = salidaId;
        ParticipanteSalidaId = participanteSalidaId;
        Tipo = tipo;
        LugarId = lugarId;
        NombreManual = nombreManual;
        DescripcionManual = descripcionManual;
        DireccionManual = direccionManual;
        FechaCreacion = fechaCreacion;
    }

    public Guid Id { get; private set; }

    public Guid SalidaId { get; private set; }

    public Guid ParticipanteSalidaId { get; private set; }

    public TipoPropuesta Tipo { get; private set; }

    public Guid? LugarId { get; private set; }

    public string? NombreManual { get; private set; }

    public string? DescripcionManual { get; private set; }

    public string? DireccionManual { get; private set; }

    public DateTimeOffset FechaCreacion { get; private set; }

    internal static Propuesta CrearDeCatalogo(
        Guid salidaId,
        Guid participanteSalidaId,
        Lugar lugar,
        DateTimeOffset fechaCreacion)
    {
        ArgumentNullException.ThrowIfNull(lugar);

        if (!lugar.Activo)
        {
            throw new DomainException(
                "No se puede proponer un lugar inactivo.");
        }

        return new Propuesta(
            Guid.NewGuid(),
            salidaId,
            participanteSalidaId,
            TipoPropuesta.LugarCatalogo,
            lugar.Id,
            null,
            null,
            null,
            fechaCreacion);
    }

    internal static Propuesta CrearManual(
        Guid salidaId,
        Guid participanteSalidaId,
        string nombre,
        string? descripcion,
        string? direccion,
        DateTimeOffset fechaCreacion)
    {
        return new Propuesta(
            Guid.NewGuid(),
            salidaId,
            participanteSalidaId,
            TipoPropuesta.Manual,
            null,
            ValidarTextoObligatorio(
                nombre,
                150,
                "El nombre de la propuesta manual es obligatorio."),
            ValidarTextoOpcional(descripcion, 1000),
            ValidarTextoOpcional(direccion, 250),
            fechaCreacion);
    }

    internal bool TieneMismoNombreManual(string nombre)
    {
        if (Tipo != TipoPropuesta.Manual)
        {
            return false;
        }

        return string.Equals(
            NormalizarParaComparar(NombreManual!),
            NormalizarParaComparar(nombre),
            StringComparison.Ordinal);
    }

    private static string NormalizarParaComparar(string valor)
    {
        return string.Join(
                ' ',
                valor.Trim().Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();
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
