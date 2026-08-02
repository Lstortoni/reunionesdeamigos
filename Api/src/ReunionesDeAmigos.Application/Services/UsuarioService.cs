using ReunionesDeAmigos.Application.DTOs.Usuarios;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Services;

public sealed class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;

    public UsuarioService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IClock clock)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
    }

    public async Task<UsuarioDto> CrearAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var usuario = Usuario.Crear(
            request.Nombre,
            request.Email,
            _clock.UtcNow);

        var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(
            usuario.Email,
            cancellationToken);

        if (usuarioExistente is not null)
        {
            throw new ConflictException(
                "Ya existe un usuario con ese email.");
        }

        await _usuarioRepository.AgregarAsync(
            usuario,
            cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return UsuarioMapper.ToDto(usuario);
    }

    public async Task<UsuarioDto> ObtenerPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
        {
            throw new NotFoundException(
                "No se encontró el usuario.");
        }

        return UsuarioMapper.ToDto(usuario);
    }
}
