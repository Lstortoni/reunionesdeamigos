namespace ReunionesDeAmigos.Application.DTOs.Usuarios;

public sealed record CrearUsuarioRequest(
    string Nombre,
    string Email,
    string Password);
