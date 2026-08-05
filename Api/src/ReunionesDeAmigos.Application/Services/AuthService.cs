using ReunionesDeAmigos.Application.DTOs.Auth;
using ReunionesDeAmigos.Application.DTOs.Usuarios;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Services;

public sealed class AuthService : IAuthService
{
    private const int LongitudMinimaPassword = 8;
    private const int LongitudMaximaPassword = 128;

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IAccessTokenGenerator _accessTokenGenerator;
    private readonly IClock _clock;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IAccessTokenGenerator accessTokenGenerator,
        IClock clock)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _accessTokenGenerator = accessTokenGenerator;
        _clock = clock;
    }

    public async Task<AutenticacionDto> RegistrarAsync(
        CrearUsuarioRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidarPassword(request.Password);

        var passwordHash = _passwordHasher.GenerarHash(request.Password);
        var usuario = Usuario.Crear(
            request.Nombre,
            request.Email,
            passwordHash,
            _clock.UtcNow);

        var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(
            usuario.Email,
            cancellationToken);

        if (usuarioExistente is not null)
        {
            throw new ConflictException(
                "Ya existe un usuario con ese email.");
        }

        await _usuarioRepository.AgregarAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return CrearRespuestaAutenticacion(usuario);
    }

    public async Task<AutenticacionDto> IniciarSesionAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var email = request.Email?.Trim().ToLowerInvariant() ?? string.Empty;
        var password = request.Password ?? string.Empty;
        var usuario = string.IsNullOrWhiteSpace(email)
            ? null
            : await _usuarioRepository.ObtenerPorEmailAsync(
                email,
                cancellationToken);

        if (usuario is null)
        {
            _passwordHasher.GenerarHash(password);
            throw CrearExcepcionCredencialesInvalidas();
        }

        if (!usuario.Activo ||
            !_passwordHasher.Verificar(password, usuario.PasswordHash))
        {
            throw CrearExcepcionCredencialesInvalidas();
        }

        return CrearRespuestaAutenticacion(usuario);
    }

    private static void ValidarPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            password.Length < LongitudMinimaPassword)
        {
            throw new ApplicationValidationException(
                $"La contraseña debe tener al menos {LongitudMinimaPassword} caracteres.");
        }

        if (password.Length > LongitudMaximaPassword)
        {
            throw new ApplicationValidationException(
                $"La contraseña no puede superar los {LongitudMaximaPassword} caracteres.");
        }
    }

    private static InvalidCredentialsException CrearExcepcionCredencialesInvalidas()
    {
        return new InvalidCredentialsException(
            "El email o la contraseña son incorrectos.");
    }

    private AutenticacionDto CrearRespuestaAutenticacion(Usuario usuario)
    {
        var token = _accessTokenGenerator.Generar(usuario);

        return new AutenticacionDto(
            UsuarioMapper.ToDto(usuario),
            token.AccessToken,
            token.ExpiraEn);
    }
}
