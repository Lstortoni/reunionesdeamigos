using ReunionesDeAmigos.Domain.Enums;
using ReunionesDeAmigos.Domain.Exceptions;
using ReunionesDeAmigos.Domain.Models;

namespace ReunionesDeAmigos.Domain.Entities;

public sealed class Salida
{
    private readonly List<ParticipanteSalida> _participantes = [];
    private readonly List<Propuesta> _propuestas = [];
    private readonly List<Voto> _votos = [];
    private readonly List<OpcionFecha> _opcionesFecha = [];

    private Salida()
    {
    }

    private Salida(
        Guid id,
        string nombre,
        string? descripcion,
        ModalidadFecha modalidadFecha,
        DateTimeOffset? fechaEncuentro,
        DateTimeOffset fechaFinPropuestas,
        DateTimeOffset fechaFinVotacion,
        string codigoAcceso,
        Usuario creador,
        DateTimeOffset fechaCreacion)
    {
        Id = id;
        Nombre = ValidarNombre(nombre);
        Descripcion = ValidarDescripcion(descripcion);
        ValidarPlazos(
            fechaCreacion,
            fechaFinPropuestas,
            fechaFinVotacion);

        if (modalidadFecha == ModalidadFecha.Fija)
        {
            ValidarFechaEncuentro(fechaEncuentro, fechaFinVotacion);
        }

        Modalidad = modalidadFecha;
        FechaEncuentro = fechaEncuentro;
        FechaFinPropuestas = fechaFinPropuestas;
        FechaFinVotacion = fechaFinVotacion;
        CodigoAcceso = ValidarCodigo(codigoAcceso);
        CreadorId = creador.Id;
        FechaCreacion = fechaCreacion;

        _participantes.Add(
            ParticipanteSalida.CrearRegistrado(Id, creador, fechaCreacion));
    }

    public Guid Id { get; private set; }

    public string Nombre { get; private set; } = string.Empty;

    public string? Descripcion { get; private set; }

    public ModalidadFecha Modalidad { get; private set; }

    public DateTimeOffset? FechaEncuentro { get; private set; }

    public DateTimeOffset FechaFinPropuestas { get; private set; }

    public DateTimeOffset FechaFinVotacion { get; private set; }

    public string CodigoAcceso { get; private set; } = string.Empty;

    public Guid CreadorId { get; private set; }

    public DateTimeOffset FechaCreacion { get; private set; }

    public DateTimeOffset? FechaCancelacion { get; private set; }

    public IReadOnlyCollection<ParticipanteSalida> Participantes => _participantes.AsReadOnly();

    public IReadOnlyCollection<Propuesta> Propuestas => _propuestas.AsReadOnly();

    public IReadOnlyCollection<Voto> Votos => _votos.AsReadOnly();

    public IReadOnlyCollection<OpcionFecha> OpcionesFecha => _opcionesFecha.AsReadOnly();

    public static Salida Crear(
        string nombre,
        string? descripcion,
        DateTimeOffset fechaEncuentro,
        DateTimeOffset fechaFinPropuestas,
        DateTimeOffset fechaFinVotacion,
        string codigoAcceso,
        Usuario creador,
        DateTimeOffset fechaCreacion)
    {
        ArgumentNullException.ThrowIfNull(creador);

        if (!creador.Activo)
        {
            throw new DomainException("Un usuario inactivo no puede crear una salida.");
        }

        return new Salida(
            Guid.NewGuid(),
            nombre,
            descripcion,
            ModalidadFecha.Fija,
            fechaEncuentro,
            fechaFinPropuestas,
            fechaFinVotacion,
            codigoAcceso,
            creador,
            fechaCreacion);
    }

    public static Salida CrearConFechaADefinir(
        string nombre,
        string? descripcion,
        IReadOnlyCollection<DateTimeOffset> opcionesFechaIniciales,
        DateTimeOffset fechaFinPropuestas,
        DateTimeOffset fechaFinVotacion,
        string codigoAcceso,
        Usuario creador,
        DateTimeOffset fechaCreacion)
    {
        ArgumentNullException.ThrowIfNull(creador);
        ArgumentNullException.ThrowIfNull(opcionesFechaIniciales);

        if (!creador.Activo)
        {
            throw new DomainException("Un usuario inactivo no puede crear una salida.");
        }

        if (opcionesFechaIniciales.Count is < 2 or > 3)
        {
            throw new DomainException(
                "Una salida con fecha a definir necesita entre dos y tres opciones iniciales.");
        }

        var salida = new Salida(
            Guid.NewGuid(),
            nombre,
            descripcion,
            ModalidadFecha.ADefinir,
            null,
            fechaFinPropuestas,
            fechaFinVotacion,
            codigoAcceso,
            creador,
            fechaCreacion);
        var participanteCreador = salida._participantes.Single();

        foreach (var fechaHora in opcionesFechaIniciales)
        {
            salida.AgregarOpcionFecha(
                participanteCreador.Id,
                fechaHora,
                fechaCreacion);
        }

        return salida;
    }

    public bool TieneParticipanteRegistrado(Guid usuarioId)
    {
        return _participantes.Any(
            participante => participante.UsuarioId == usuarioId);
    }

    public EstadoSalida ObtenerEstado(DateTimeOffset fechaActual)
    {
        if (FechaCancelacion.HasValue)
        {
            return EstadoSalida.Cancelada;
        }

        if (fechaActual < FechaFinPropuestas)
        {
            return EstadoSalida.RecibiendoPropuestas;
        }

        if (fechaActual < FechaFinVotacion)
        {
            return EstadoSalida.VotacionAbierta;
        }

        if (Modalidad == ModalidadFecha.ADefinir)
        {
            var ultimaFechaPosible = ObtenerOpcionesFechaOrdenadas()
                .Take(3)
                .Select(x => (DateTimeOffset?)x.FechaHora)
                .Max();

            return ultimaFechaPosible.HasValue && fechaActual >= ultimaFechaPosible.Value
                ? EstadoSalida.Finalizada
                : EstadoSalida.Confirmada;
        }

        if (fechaActual < FechaEncuentro!.Value)
        {
            return EstadoSalida.Confirmada;
        }

        return EstadoSalida.Finalizada;
    }

    public ParticipanteSalida AgregarParticipanteRegistrado(
        Usuario usuario,
        DateTimeOffset fechaIngreso)
    {
        ArgumentNullException.ThrowIfNull(usuario);
        ValidarIngresoPermitido(fechaIngreso);

        if (!usuario.Activo)
        {
            throw new DomainException("Un usuario inactivo no puede ingresar a una salida.");
        }

        if (_participantes.Any(participante => participante.UsuarioId == usuario.Id))
        {
            throw new DomainException("El usuario ya participa en esta salida.");
        }

        var participante = ParticipanteSalida.CrearRegistrado(
            Id,
            usuario,
            fechaIngreso);

        _participantes.Add(participante);
        return participante;
    }

    public OpcionFecha AgregarOpcionFecha(
        Guid participanteSalidaId,
        DateTimeOffset fechaHora,
        DateTimeOffset fechaCreacion)
    {
        if (Modalidad != ModalidadFecha.ADefinir)
        {
            throw new DomainException(
                "Una salida con fecha fija no admite opciones de fecha.");
        }

        ValidarPropuestaPermitida(participanteSalidaId, fechaCreacion);

        if (fechaHora <= FechaFinVotacion)
        {
            throw new DomainException(
                "La opción de fecha debe ser posterior al fin de la votación.");
        }

        if (_opcionesFecha.Any(x => x.FechaHora == fechaHora))
        {
            throw new DomainException(
                "La fecha y hora ya fueron propuestas en esta salida.");
        }


        if (_opcionesFecha.Count >= 6)
        {
            throw new DomainException(
                "La salida puede tener como máximo seis opciones de fecha.");
        }

        if (_opcionesFecha.Count(x =>
                x.ParticipanteSalidaId == participanteSalidaId) >= 3)
        {
            throw new DomainException(
                "Cada participante puede proponer como máximo tres opciones de fecha.");
        }

        var opcion = OpcionFecha.Crear(
            Id,
            participanteSalidaId,
            fechaHora,
            fechaCreacion);

        _opcionesFecha.Add(opcion);
        return opcion;
    }

    public DisponibilidadFecha RegistrarDisponibilidadFecha(
        Guid participanteSalidaId,
        Guid opcionFechaId,
        bool disponible,
        DateTimeOffset fechaRespuesta)
    {
        if (Modalidad != ModalidadFecha.ADefinir)
        {
            throw new DomainException(
                "Una salida con fecha fija no registra disponibilidades.");
        }

        if (ObtenerEstado(fechaRespuesta) != EstadoSalida.VotacionAbierta)
        {
            throw new DomainException(
                "La disponibilidad solo puede informarse durante la votación.");
        }

        ValidarParticipante(participanteSalidaId);
        var opcion = _opcionesFecha.FirstOrDefault(x => x.Id == opcionFechaId)
            ?? throw new DomainException(
                "La opción de fecha no pertenece a esta salida.");

        return opcion.RegistrarDisponibilidad(
            participanteSalidaId,
            disponible,
            fechaRespuesta);
    }

    public ResultadoDisponibilidadFecha ObtenerResultadoDisponibilidad(
        DateTimeOffset fechaActual)
    {
        if (Modalidad != ModalidadFecha.ADefinir)
        {
            throw new DomainException(
                "Una salida con fecha fija no tiene resultados de disponibilidad.");
        }

        if (fechaActual < FechaFinVotacion)
        {
            throw new DomainException(
                "El resultado estará disponible cuando finalice la votación.");
        }

        var participantes = _participantes.Select(x => x.Id).ToArray();
        var opciones = ObtenerOpcionesFechaOrdenadas(participantes);

        return new ResultadoDisponibilidadFecha(
            participantes.Length,
            opciones,
            opciones.Take(3).ToArray());
    }

    private DisponibilidadOpcionFecha[] ObtenerOpcionesFechaOrdenadas(
        IReadOnlyCollection<Guid>? participantesSalidaIds = null)
    {
        var participantes = participantesSalidaIds?.ToArray()
            ?? _participantes.Select(x => x.Id).ToArray();

        return _opcionesFecha
            .Select(opcion =>
            {
                var disponibles = opcion.Disponibilidades
                    .Where(x => x.Disponible)
                    .Select(x => x.ParticipanteSalidaId)
                    .ToArray();
                var noDisponibles = opcion.Disponibilidades
                    .Where(x => !x.Disponible)
                    .Select(x => x.ParticipanteSalidaId)
                    .ToArray();
                var respondieron = disponibles.Concat(noDisponibles).ToHashSet();
                var sinResponder = participantes
                    .Where(x => !respondieron.Contains(x))
                    .ToArray();

                return new DisponibilidadOpcionFecha(
                    opcion.Id,
                    opcion.FechaHora,
                    disponibles.Length,
                    disponibles,
                    noDisponibles,
                    sinResponder);
            })
            .OrderByDescending(x => x.CantidadDisponibles)
            .ThenBy(x => x.FechaHora)
            .ToArray();
    }

    public ParticipanteSalida AgregarParticipanteInvitado(
        string nombreVisible,
        string credencialInvitadoHash,
        DateTimeOffset fechaIngreso)
    {
        ValidarIngresoPermitido(fechaIngreso);

        var participante = ParticipanteSalida.CrearInvitado(
            Id,
            nombreVisible,
            credencialInvitadoHash,
            fechaIngreso);

        _participantes.Add(participante);
        return participante;
    }

    public Propuesta AgregarPropuestaExterna(
        Guid participanteSalidaId,
        string googlePlaceId,
        DateTimeOffset fechaCreacion)
    {
        ValidarPropuestaPermitida(participanteSalidaId, fechaCreacion);
        ValidarLimitePropuestasParticipante(participanteSalidaId);

        if (_propuestas.Any(propuesta =>
                string.Equals(
                    propuesta.GooglePlaceId,
                    googlePlaceId?.Trim(),
                    StringComparison.Ordinal)))
        {
            throw new DomainException(
                "El lugar ya fue propuesto en esta salida.");
        }

        var propuesta = Propuesta.CrearExterna(
            Id,
            participanteSalidaId,
            googlePlaceId,
            fechaCreacion);

        _propuestas.Add(propuesta);
        return propuesta;
    }

    public Propuesta AgregarPropuestaManual(
        Guid participanteSalidaId,
        string nombre,
        string? descripcion,
        string? direccion,
        DateTimeOffset fechaCreacion)
    {
        ValidarPropuestaPermitida(participanteSalidaId, fechaCreacion);
        ValidarLimitePropuestasParticipante(participanteSalidaId);

        if (_propuestas.Any(propuesta => propuesta.TieneMismoNombreManual(nombre)))
        {
            throw new DomainException(
                "Ya existe una propuesta manual con el mismo nombre.");
        }

        var propuesta = Propuesta.CrearManual(
            Id,
            participanteSalidaId,
            nombre,
            descripcion,
            direccion,
            fechaCreacion);

        _propuestas.Add(propuesta);
        return propuesta;
    }

    public Voto RegistrarVoto(
        Guid participanteSalidaId,
        Guid propuestaId,
        DateTimeOffset fechaVoto)
    {
        if (ObtenerEstado(fechaVoto) != EstadoSalida.VotacionAbierta)
        {
            throw new DomainException(
                "Solo se puede votar mientras la votación está abierta.");
        }

        ValidarParticipante(participanteSalidaId);

        if (_propuestas.All(propuesta => propuesta.Id != propuestaId))
        {
            throw new DomainException(
                "La propuesta no pertenece a esta salida.");
        }

        var votoExistente = _votos.FirstOrDefault(
            voto => voto.ParticipanteSalidaId == participanteSalidaId);

        if (votoExistente is not null)
        {
            votoExistente.CambiarPropuesta(propuestaId, fechaVoto);
            return votoExistente;
        }

        var voto = Voto.Crear(
            Id,
            participanteSalidaId,
            propuestaId,
            fechaVoto);

        _votos.Add(voto);
        return voto;
    }

    public ResultadoVotacion ObtenerResultado(DateTimeOffset fechaActual)
    {
        var estado = ObtenerEstado(fechaActual);

        if (estado == EstadoSalida.Cancelada)
        {
            throw new DomainException(
                "Una salida cancelada no tiene resultado.");
        }

        if (estado is EstadoSalida.RecibiendoPropuestas
            or EstadoSalida.VotacionAbierta)
        {
            throw new DomainException(
                "El resultado estará disponible cuando finalice la votación.");
        }

        if (_votos.Count == 0)
        {
            return new ResultadoVotacion(0, Array.Empty<PropuestaVotada>());
        }

        var resultados = _votos
            .GroupBy(voto => voto.PropuestaId)
            .Select(grupo => new PropuestaVotada(grupo.Key, grupo.Count()))
            .ToArray();

        var cantidadGanadora = resultados.Max(resultado => resultado.CantidadVotos);
        var ganadoras = resultados
            .Where(resultado => resultado.CantidadVotos == cantidadGanadora)
            .ToArray();

        return new ResultadoVotacion(_votos.Count, ganadoras);
    }

    public void ActualizarDatos(
        Guid usuarioSolicitanteId,
        string nombre,
        string? descripcion,
        DateTimeOffset fechaEncuentro,
        DateTimeOffset fechaActual)
    {
        ValidarCreador(usuarioSolicitanteId);
        ValidarSalidaActiva();

        if (Modalidad != ModalidadFecha.Fija)
        {
            throw new DomainException(
                "Una salida con fecha a definir no admite una única fecha de encuentro.");
        }

        if (fechaActual >= FechaFinVotacion)
        {
            throw new DomainException(
                "No se puede modificar una salida cuya votación finalizó.");
        }

        if (fechaEncuentro <= FechaFinVotacion)
        {
            throw new DomainException(
                "La fecha del encuentro debe ser posterior al fin de votación.");
        }

        Nombre = ValidarNombre(nombre);
        Descripcion = ValidarDescripcion(descripcion);
        FechaEncuentro = fechaEncuentro;
    }

    public void ActualizarFinPropuestas(
        Guid usuarioSolicitanteId,
        DateTimeOffset nuevaFecha,
        DateTimeOffset fechaActual)
    {
        ValidarCreador(usuarioSolicitanteId);
        ValidarSalidaActiva();

        if (ObtenerEstado(fechaActual) != EstadoSalida.RecibiendoPropuestas)
        {
            throw new DomainException(
                "El plazo de propuestas solo puede modificarse mientras se reciben propuestas.");
        }

        ValidarPlazos(
            fechaActual,
            nuevaFecha,
            FechaFinVotacion);

        ValidarLimiteOpcionesFecha(FechaFinVotacion);

        FechaFinPropuestas = nuevaFecha;
    }

    public void ActualizarFinVotacion(
        Guid usuarioSolicitanteId,
        DateTimeOffset nuevaFecha,
        DateTimeOffset fechaActual)
    {
        ValidarCreador(usuarioSolicitanteId);
        ValidarSalidaActiva();

        if (fechaActual >= FechaFinVotacion)
        {
            throw new DomainException(
                "El plazo de votación no puede modificarse después de su vencimiento.");
        }

        if (nuevaFecha <= fechaActual)
        {
            throw new DomainException(
                "El nuevo fin de votación debe ser posterior a la fecha actual.");
        }

        if (nuevaFecha <= FechaFinPropuestas)
        {
            throw new DomainException(
                "El fin de votación debe ser posterior al fin de propuestas.");
        }

        if (FechaEncuentro.HasValue && FechaEncuentro.Value <= nuevaFecha)
        {
            throw new DomainException(
                "La fecha del encuentro debe ser posterior al fin de votación.");
        }

        ValidarLimiteOpcionesFecha(nuevaFecha);

        FechaFinVotacion = nuevaFecha;
    }

    public void Cancelar(
        Guid usuarioSolicitanteId,
        DateTimeOffset fechaCancelacion)
    {
        ValidarCreador(usuarioSolicitanteId);

        if (FechaCancelacion.HasValue)
        {
            throw new DomainException("La salida ya se encuentra cancelada.");
        }

        FechaCancelacion = fechaCancelacion;
    }

    private void ValidarIngresoPermitido(DateTimeOffset fechaIngreso)
    {
        var estado = ObtenerEstado(fechaIngreso);

        if (estado is not EstadoSalida.RecibiendoPropuestas
            and not EstadoSalida.VotacionAbierta)
        {
            throw new DomainException(
                "La salida no permite el ingreso de nuevos participantes.");
        }
    }

    private void ValidarPropuestaPermitida(
        Guid participanteSalidaId,
        DateTimeOffset fechaCreacion)
    {
        if (ObtenerEstado(fechaCreacion) != EstadoSalida.RecibiendoPropuestas)
        {
            throw new DomainException(
                "Solo se pueden agregar propuestas durante el período habilitado.");
        }

        ValidarParticipante(participanteSalidaId);
    }

    private void ValidarParticipante(Guid participanteSalidaId)
    {
        if (_participantes.All(
                participante => participante.Id != participanteSalidaId))
        {
            throw new DomainException(
                "La persona no participa en esta salida.");
        }
    }

    private void ValidarLimitePropuestasParticipante(Guid participanteSalidaId)
    {
        if (_propuestas.Count(x =>
                x.ParticipanteSalidaId == participanteSalidaId) >= 3)
        {
            throw new DomainException(
                "Cada participante puede proponer como máximo tres lugares.");
        }
    }

    private void ValidarCreador(Guid usuarioSolicitanteId)
    {
        if (usuarioSolicitanteId != CreadorId)
        {
            throw new DomainException(
                "Solo el creador puede realizar esta operación.");
        }
    }

    private void ValidarSalidaActiva()
    {
        if (FechaCancelacion.HasValue)
        {
            throw new DomainException("La salida se encuentra cancelada.");
        }
    }

    private void ValidarLimiteOpcionesFecha(DateTimeOffset fechaFinVotacion)
    {
        if (_opcionesFecha.Any(x => x.FechaHora <= fechaFinVotacion))
        {
            throw new DomainException(
                "El fin de votación debe ser anterior a todas las opciones de fecha.");
        }
    }

    private static void ValidarPlazos(
        DateTimeOffset fechaReferencia,
        DateTimeOffset fechaFinPropuestas,
        DateTimeOffset fechaFinVotacion)
    {
        if (fechaFinPropuestas <= fechaReferencia)
        {
            throw new DomainException(
                "El fin de propuestas debe ser posterior a la fecha actual.");
        }

        if (fechaFinVotacion <= fechaFinPropuestas)
        {
            throw new DomainException(
                "El fin de votación debe ser posterior al fin de propuestas.");
        }

    }

    private static void ValidarFechaEncuentro(
        DateTimeOffset? fechaEncuentro,
        DateTimeOffset fechaFinVotacion)
    {
        if (!fechaEncuentro.HasValue)
        {
            throw new DomainException(
                "La fecha del encuentro es obligatoria para una salida con fecha fija.");
        }

        if (fechaEncuentro.Value <= fechaFinVotacion)
        {
            throw new DomainException(
                "La fecha del encuentro debe ser posterior al fin de votación.");
        }
    }

    private static string ValidarNombre(string nombre)
    {
        var nombreNormalizado = nombre?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(nombreNormalizado))
        {
            throw new DomainException("El nombre de la salida es obligatorio.");
        }

        if (nombreNormalizado.Length > 150)
        {
            throw new DomainException(
                "El nombre de la salida no puede superar los 150 caracteres.");
        }

        return nombreNormalizado;
    }

    private static string? ValidarDescripcion(string? descripcion)
    {
        var descripcionNormalizada = descripcion?.Trim();

        if (string.IsNullOrWhiteSpace(descripcionNormalizada))
        {
            return null;
        }

        if (descripcionNormalizada.Length > 1000)
        {
            throw new DomainException(
                "La descripción no puede superar los 1000 caracteres.");
        }

        return descripcionNormalizada;
    }

    private static string ValidarCodigo(string codigo)
    {
        var codigoNormalizado = codigo?.Trim().ToUpperInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(codigoNormalizado))
        {
            throw new DomainException("El código de acceso es obligatorio.");
        }

        if (codigoNormalizado.Length > 30)
        {
            throw new DomainException(
                "El código de acceso no puede superar los 30 caracteres.");
        }

        return codigoNormalizado;
    }
}
