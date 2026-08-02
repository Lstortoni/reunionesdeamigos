using ReunionesDeAmigos.Application.DTOs.Usuarios;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Mappers;

internal static class UsuarioMapper
{
    public static UsuarioDto ToDto(Usuario usuario)
    {
        return new UsuarioDto(
            usuario.Id,
            usuario.Nombre,
            usuario.Email,
            usuario.FechaCreacion,
            usuario.Activo);
    }
}
