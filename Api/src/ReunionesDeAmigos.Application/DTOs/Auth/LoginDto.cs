using ReunionesDeAmigos.Application.DTOs.Usuarios;

namespace ReunionesDeAmigos.Application.DTOs.Auth;

public sealed record LoginDto(
    UsuarioDto Usuario,
    string AccessToken,
    DateTimeOffset ExpiraEn);
