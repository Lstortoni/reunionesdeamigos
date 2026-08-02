namespace ReunionesDeAmigos.Application.DTOs.Usuarios;

public sealed record UsuarioDto(
    Guid Id,
    string Nombre,
    string Email,
    DateTimeOffset FechaCreacion,
    bool Activo);
