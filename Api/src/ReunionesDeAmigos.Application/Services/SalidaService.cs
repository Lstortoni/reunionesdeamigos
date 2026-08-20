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
    private readonly IEnlaceInvitacionGenerator _enlaceInvitacionGenerator;

    public SalidaService(
        ISalidaRepository salidaRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IClock clock,
        ICodigoAccesoGenerator codigoAccesoGenerator,
        IEnlaceInvitacionGenerator enlaceInvitacionGenerator)
    {
        _salidaRepository = salidaRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _codigoAccesoGenerator = codigoAccesoGenerator;
        _enlaceInvitacionGenerator = enlaceInvitacionGenerator;
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
        var fechaFinPropuestas = fechaActual.AddDays(
            request.DiasParaPropuestas);
        var fechaFinVotacion = fechaFinPropuestas.AddDays(
            request.DiasParaVotar);
        var codigoAcceso = await GenerarCodigoUnicoAsync(
            cancellationToken);

        var salida = CrearSalidaSegunModalidad(
            request,
            creador,
            fechaActual,
            fechaFinPropuestas,
            fechaFinVotacion,
            codigoAcceso);

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

        return CrearDto(salida, fechaActual);
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

        return CrearDto(salida, _clock.UtcNow);
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
                salida.Modalidad,
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

    private SalidaDto CrearDto(
        Salida salida,
        DateTimeOffset fechaActual)
    {
        var enlaceInvitacion = _enlaceInvitacionGenerator.Generar(
            salida.CodigoAcceso);

        return SalidaMapper.ToDto(
            salida,
            fechaActual,
            enlaceInvitacion);
    }

    private static void ValidarDuraciones(CrearSalidaRequest request)
    {
        if (!Enum.IsDefined(request.Modalidad))
        {
            throw new ApplicationValidationException(
                "La modalidad de fecha no es válida.");
        }

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
            request.PropuestasIniciales.Count is < 1 or > 3)
        {
            throw new ApplicationValidationException(
                "La salida debe tener entre una y tres propuestas iniciales.");
        }
    }

    private static Salida CrearSalidaSegunModalidad(
        CrearSalidaRequest request,
        Usuario creador,
        DateTimeOffset fechaActual,
        DateTimeOffset fechaFinPropuestas,
        DateTimeOffset fechaFinVotacion,
        string codigoAcceso)
    {
        if (request.Modalidad == ModalidadFecha.Fija)
        {
            if (!request.FechaEncuentro.HasValue)
            {
                throw new ApplicationValidationException(
                    "Una salida con fecha fija debe indicar la fecha del encuentro.");
            }

            if (request.OpcionesFechaIniciales is { Count: > 0 })
            {
                throw new ApplicationValidationException(
                    "Una salida con fecha fija no admite opciones de fecha.");
            }

            return Salida.Crear(
                request.Nombre,
                request.Descripcion,
                request.FechaEncuentro.Value.ToUniversalTime(),
                fechaFinPropuestas,
                fechaFinVotacion,
                codigoAcceso,
                creador,
                fechaActual);
        }

        if (request.FechaEncuentro.HasValue)
        {
            throw new ApplicationValidationException(
                "Una salida con fecha a definir no debe indicar una fecha fija.");
        }

        if (request.OpcionesFechaIniciales is null ||
            request.OpcionesFechaIniciales.Count is < 2 or > 3)
        {
            throw new ApplicationValidationException(
                "Una salida con fecha a definir necesita entre dos y tres opciones de fecha.");
        }

        var opcionesUtc = request.OpcionesFechaIniciales
            .Select(x => x.ToUniversalTime())
            .ToArray();

        return Salida.CrearConFechaADefinir(
            request.Nombre,
            request.Descripcion,
            opcionesUtc,
            fechaFinPropuestas,
            fechaFinVotacion,
            codigoAcceso,
            creador,
            fechaActual);
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
