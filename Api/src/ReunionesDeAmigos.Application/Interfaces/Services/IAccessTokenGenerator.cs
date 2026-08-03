using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Interfaces.Services;

public interface IAccessTokenGenerator
{
    AccessTokenGenerado Generar(Usuario usuario);
}

public sealed record AccessTokenGenerado(
    string AccessToken,
    DateTimeOffset ExpiraEn);
