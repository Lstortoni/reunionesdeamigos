using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Application.Exceptions;
using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Application.Interfaces.Services;
using ReunionesDeAmigos.Application.Mappers;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Services;

public sealed class SalidaService : ISalidaService
{
    private const int MaximosIntentosGeneracionCodigo = 10;

    private readonly ISalidaRepository _salidaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ICodigoAccesoGenerator _codigoAccesoGenerator;

    public SalidaService(
        ISalidaRepository salidaRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        ICodigoAccesoGenerator codigoAccesoGenerator)
    {
        _salidaRepository = salidaRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _codigoAccesoGenerator = codigoAccesoGenerator;
    }

    public async Task<SalidaDto> CrearAsync(
        CrearSalidaRequest request,
        Guid creadorId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidarDuraciones(request);

        var creador = await _usuarioRepository.ObtenerPorIdAsync(
            creadorId,
            cancellationToken);

        if (creador is null)
        {
            throw new NotFoundException(
                "No se encontró el usuario creador.");
        }

        var fechaActual = _clock.UtcNow;
        var fechaEncuentroUtc = request.FechaEncuentro.ToUniversalTime();
        var fechaFinPropuestas = fechaActual.AddDays(
            request.DiasParaPropuestas);
        var fechaFinVotacion = fechaFinPropuestas.AddDays(
            request.DiasParaVotar);
        var codigoAcceso = await GenerarCodigoUnicoAsync(
            cancellationToken);

        var salida = Salida.Crear(
            request.Nombre,
            request.Descripcion,
            fechaEncuentroUtc,
            fechaFinPropuestas,
            fechaFinVotacion,
            codigoAcceso,
            creador,
            fechaActual);

        var participanteCreador = salida.Participantes.Single();
        AgregarPropuestasIniciales(
            salida,
            participanteCreador.Id,
            request.PropuestasIniciales,
            fechaActual);

        await _salidaRepository.AgregarAsync(
            salida,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return SalidaMapper.ToDto(salida, fechaActual);
    }

    public async Task<SalidaDto> ObtenerPorIdAsync(
        Guid salidaId,
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var salida = await _salidaRepository.ObtenerPorIdAsync(
            salidaId,
            cancellationToken);

        if (salida is null ||
            !salida.TieneParticipanteRegistrado(usuarioId))
        {
            throw new NotFoundException(
                "No se encontró la salida.");
        }

        return SalidaMapper.ToDto(salida, _clock.UtcNow);
    }

    public async Task<IReadOnlyCollection<SalidaResumenDto>> ObtenerMiasAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var salidas = await _salidaRepository.ObtenerPorUsuarioAsync(
            usuarioId,
            cancellationToken);
        var fechaActual = _clock.UtcNow;

        return salidas
            .Select(salida => new SalidaResumenDto(
                salida.Id,
                salida.Nombre,
                salida.FechaEncuentro,
                salida.ObtenerEstado(fechaActual),
                salida.CreadorId == usuarioId,
                salida.Participantes.Count))
            .ToArray();
    }

    private async Task<string> GenerarCodigoUnicoAsync(
        CancellationToken cancellationToken)
    {
        for (var intento = 0;
             intento < MaximosIntentosGeneracionCodigo;
             intento++)
        {
            var codigo = _codigoAccesoGenerator.Generar();
            var existe = await _salidaRepository.ExisteCodigoAsync(
                codigo,
                cancellationToken);

            if (!existe)
            {
                return codigo;
            }
        }

        throw new ConflictException(
            "No se pudo generar un código de acceso único.");
    }

    private static void ValidarDuraciones(CrearSalidaRequest request)
    {
        if (request.DiasParaPropuestas <= 0)
        {
            throw new ApplicationValidationException(
                "La cantidad de días para propuestas debe ser mayor que cero.");
        }

        if (request.DiasParaVotar <= 0)
        {
            throw new ApplicationValidationException(
                "La cantidad de días para votar debe ser mayor que cero.");
        }

        if (request.PropuestasIniciales is null ||
            request.PropuestasIniciales.Count != 3)
        {
            throw new ApplicationValidationException(
                "La salida debe tener exactamente tres propuestas iniciales.");
        }
    }

    private static void AgregarPropuestasIniciales(
        Salida salida,
        Guid participanteCreadorId,
        IReadOnlyCollection<CrearPropuestaInicialRequest> propuestas,
        DateTimeOffset fechaActual)
    {
        foreach (var propuesta in propuestas)
        {
            ArgumentNullException.ThrowIfNull(propuesta);

            switch (propuesta.Tipo)
            {
                case TipoPropuesta.LugarExterno:
                    if (string.IsNullOrWhiteSpace(propuesta.GooglePlaceId))
                    {
                        throw new ApplicationValidationException(
                            "Una propuesta externa debe indicar GooglePlaceId.");
                    }

                    salida.AgregarPropuestaExterna(
                        participanteCreadorId,
                        propuesta.GooglePlaceId,
                        fechaActual);
                    break;

                case TipoPropuesta.Manual:
                    if (!string.IsNullOrWhiteSpace(propuesta.GooglePlaceId))
                    {
                        throw new ApplicationValidationException(
                            "Una propuesta manual no puede indicar GooglePlaceId.");
                    }

                    salida.AgregarPropuestaManual(
                        participanteCreadorId,
                        propuesta.NombreManual ?? string.Empty,
                        propuesta.DescripcionManual,
                        propuesta.DireccionManual,
                        fechaActual);
                    break;

                default:
                    throw new ApplicationValidationException(
                        "El tipo de propuesta inicial no es válido.");
            }
        }
    }

}
