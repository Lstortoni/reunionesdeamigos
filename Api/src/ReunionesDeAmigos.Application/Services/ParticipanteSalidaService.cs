using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;

namespace ReunionesDeAmigos.Application.Services;

public sealed class ParticipanteSalidaService : IParticipanteSalidaService
{
    private readonly ISalidaRepository _salidaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ICredencialInvitadoService _credencialInvitadoService;

    public ParticipanteSalidaService(
        ISalidaRepository salidaRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        ICredencialInvitadoService credencialInvitadoService)
    {
        _salidaRepository = salidaRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _credencialInvitadoService = credencialInvitadoService;
    }

    public async Task<ParticipanteSalidaDto> IngresarRegistradoAsync(
        string codigoAcceso,
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var salida = await ObtenerSalidaPorCodigoAsync(
            codigoAcceso,
            cancellationToken);

        var usuario = await _usuarioRepository.ObtenerPorIdAsync(
            usuarioId,
            cancellationToken);

        if (usuario is null)
        {
            throw new NotFoundException(
                "No se encontró el usuario.");
        }

        var participante = salida.AgregarParticipanteRegistrado(
            usuario,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ParticipanteSalidaMapper.ToDto(participante);
    }

    public async Task<IngresoInvitadoDto> IngresarComoInvitadoAsync(
        IngresarInvitadoRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var salida = await ObtenerSalidaPorCodigoAsync(
            request.CodigoAcceso,
            cancellationToken);
        var credencial = _credencialInvitadoService.Generar();

        var participante = salida.AgregarParticipanteInvitado(
            request.NombreVisible,
            credencial.Hash,
            _clock.UtcNow);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new IngresoInvitadoDto(
            ParticipanteSalidaMapper.ToDto(participante),
            credencial.Credencial);
    }

    private async Task<Salida> ObtenerSalidaPorCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken)
    {
        var codigoNormalizado = NormalizarCodigo(codigoAcceso);
        var salida = await _salidaRepository.ObtenerPorCodigoAsync(
            codigoNormalizado,
            cancellationToken);

        if (salida is null)
        {
            throw new NotFoundException(
                "No se encontró una salida con el código indicado.");
        }

        return salida;
    }

    private static string NormalizarCodigo(string codigoAcceso)
    {
        var codigoNormalizado = codigoAcceso?.Trim().ToUpperInvariant()
            ?? string.Empty;

        if (string.IsNullOrWhiteSpace(codigoNormalizado))
        {
            throw new ApplicationValidationException(
                "El código de acceso es obligatorio.");
        }

        return codigoNormalizado;
    }
}
