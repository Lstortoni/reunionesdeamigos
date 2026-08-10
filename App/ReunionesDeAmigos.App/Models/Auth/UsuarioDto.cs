namespace ReunionesDeAmigos.App.Models.Auth;

public sealed record UsuarioDto(
    Guid Id,
    string Nombre,
    string Email,
    DateTimeOffset FechaCreacion,
    bool Activo);
