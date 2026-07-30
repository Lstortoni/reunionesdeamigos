using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Domain.Entities;

public sealed class ParticipanteSalida
{
    private ParticipanteSalida()
    {
    }

    private ParticipanteSalida(
        Guid id,
        Guid salidaId,
        Guid? usuarioId,
        string nombreVisible,
        DateTimeOffset fechaIngreso,
        string? credencialInvitadoHash)
    {
        Id = id;
        SalidaId = salidaId;
        UsuarioId = usuarioId;
        NombreVisible = ValidarNombre(nombreVisible);
        FechaIngreso = fechaIngreso;
        CredencialInvitadoHash = credencialInvitadoHash;
    }

    public Guid Id { get; private set; }

    public Guid SalidaId { get; private set; }

    public Guid? UsuarioId { get; private set; }

    public string NombreVisible { get; private set; } = string.Empty;

    public DateTimeOffset FechaIngreso { get; private set; }

    public string? CredencialInvitadoHash { get; private set; }

    public bool EsInvitado => UsuarioId is null;

    internal static ParticipanteSalida CrearRegistrado(
        Guid salidaId,
        Usuario usuario,
        DateTimeOffset fechaIngreso)
    {
        ArgumentNullException.ThrowIfNull(usuario);

        return new ParticipanteSalida(
            Guid.NewGuid(),
            salidaId,
            usuario.Id,
            usuario.Nombre,
            fechaIngreso,
            null);
    }

    internal static ParticipanteSalida CrearInvitado(
        Guid salidaId,
        string nombreVisible,
        string credencialInvitadoHash,
        DateTimeOffset fechaIngreso)
    {
        var hashNormalizado = credencialInvitadoHash?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(hashNormalizado))
        {
            throw new DomainException(
                "La credencial segura del participante invitado es obligatoria.");
        }

        return new ParticipanteSalida(
            Guid.NewGuid(),
            salidaId,
            null,
            nombreVisible,
            fechaIngreso,
            hashNormalizado);
    }

    private static string ValidarNombre(string nombre)
    {
        var nombreNormalizado = nombre?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombreNormalizado))
        {
            throw new DomainException("El nombre visible del participante es obligatorio.");
        }

        if (nombreNormalizado.Length > 100)
        {
            throw new DomainException(
                "El nombre visible no puede superar los 100 caracteres.");
        }

        return nombreNormalizado;
    }
}
