using ReunionesDeAmigos.Application.DTOs.Usuarios;

namespace ReunionesDeAmigos.Application.DTOs.Auth;

public sealed record AutenticacionDto(
    UsuarioDto Usuario,
    string AccessToken,
    DateTimeOffset ExpiraEn);
