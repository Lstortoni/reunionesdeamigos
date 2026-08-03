namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IPasswordHasher
{
    string GenerarHash(string password);

    bool Verificar(
        string password,
        string passwordHash);
}
