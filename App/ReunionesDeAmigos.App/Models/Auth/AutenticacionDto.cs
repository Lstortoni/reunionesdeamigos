namespace ReunionesDeAmigos.App.Models.Auth;

public sealed record AutenticacionDto(
    UsuarioDto Usuario,
    string AccessToken,
    DateTimeOffset ExpiraEn);
