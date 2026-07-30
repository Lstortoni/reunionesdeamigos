namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface ICredencialInvitadoService
{
    CredencialInvitadoGenerada Generar();
}

public sealed record CredencialInvitadoGenerada(
    string Credencial,
    string Hash);
