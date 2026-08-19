using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;
using ReunionesDeAmigos.Domain.Exceptions;

namespace ReunionesDeAmigos.Domain.Tests.Entities;

public sealed class SalidaTests
{
    private static readonly DateTimeOffset FechaActual = new(
        2026,
        8,
        1,
        12,
        0,
        0,
        TimeSpan.Zero);

    [Fact]
    public void AgregarPropuestaExterna_DeberiaAgregarElLugarALaSalida()
    {
        // Arrange
        var salida = CrearSalida();
        const string googlePlaceId = "ChIJ-lugar-prueba";
        var participanteCreador = Assert.Single(
            salida.Participantes);

        // Act
        var propuesta = salida.AgregarPropuestaExterna(
            participanteCreador.Id,
            googlePlaceId,
            FechaActual.AddDays(1));

        // Assert
        var propuestaGuardada = Assert.Single(
            salida.Propuestas);
        Assert.Equal(propuesta.Id, propuestaGuardada.Id);
        Assert.Equal(googlePlaceId, propuestaGuardada.GooglePlaceId);
    }

    [Fact]
    public void AgregarPropuestaExterna_DeberiaRechazarUnLugarDuplicado()
    {
        // Arrange
        var salida = CrearSalida();
        const string googlePlaceId = "ChIJ-lugar-prueba";
        var participante = Assert.Single(salida.Participantes);
        salida.AgregarPropuestaExterna(
            participante.Id,
            googlePlaceId,
            FechaActual.AddDays(1));

        // Act
        var accion = () => salida.AgregarPropuestaExterna(
            participante.Id,
            googlePlaceId,
            FechaActual.AddDays(1));

        // Assert
        Assert.Throws<DomainException>(accion);
        Assert.Single(salida.Propuestas);
    }

    [Fact]
    public void AgregarPropuestaManual_DeberiaRechazarUnNombreEquivalente()
    {
        // Arrange
        var salida = CrearSalida();
        var participante = Assert.Single(salida.Participantes);
        salida.AgregarPropuestaManual(
            participante.Id,
            "Casa de Fede",
            null,
            null,
            FechaActual.AddDays(1));

        // Act
        var accion = () => salida.AgregarPropuestaManual(
            participante.Id,
            "  casa   DE fede  ",
            null,
            null,
            FechaActual.AddDays(1));

        // Assert
        Assert.Throws<DomainException>(accion);
        Assert.Single(salida.Propuestas);
    }

    [Fact]
    public void RegistrarVoto_DeberiaCambiarElVotoSinCrearOtro()
    {
        // Arrange
        var salida = CrearSalida();
        var participante = Assert.Single(salida.Participantes);
        var primeraPropuesta = salida.AgregarPropuestaManual(
            participante.Id,
            "Casa de Fede",
            null,
            null,
            FechaActual.AddDays(1));
        var segundaPropuesta = salida.AgregarPropuestaManual(
            participante.Id,
            "Pedir pizzas",
            null,
            null,
            FechaActual.AddDays(1));
        var primerVoto = salida.RegistrarVoto(
            participante.Id,
            primeraPropuesta.Id,
            FechaActual.AddDays(4));

        // Act
        var votoModificado = salida.RegistrarVoto(
            participante.Id,
            segundaPropuesta.Id,
            FechaActual.AddDays(5));

        // Assert
        var votoGuardado = Assert.Single(salida.Votos);
        Assert.Equal(primerVoto.Id, votoModificado.Id);
        Assert.Equal(segundaPropuesta.Id, votoGuardado.PropuestaId);
    }

    [Fact]
    public void CrearConFechaADefinir_DeberiaGuardarOpcionesYDejarFechaVacia()
    {
        var creador = CrearUsuario();
        var opciones = new[]
        {
            FechaActual.AddDays(10),
            FechaActual.AddDays(11),
            FechaActual.AddDays(12)
        };

        var salida = Salida.CrearConFechaADefinir(
            "Cena a coordinar",
            null,
            opciones,
            FechaActual.AddDays(3),
            FechaActual.AddDays(6),
            "CENA-FECHAS",
            creador,
            FechaActual);

        Assert.Equal(ModalidadFecha.ADefinir, salida.ModalidadFecha);
        Assert.Null(salida.FechaEncuentro);
        Assert.Equal(3, salida.OpcionesFecha.Count);
    }

    [Fact]
    public void CrearConFechaADefinir_DeberiaRechazarMenosDeDosOpciones()
    {
        var creador = CrearUsuario();

        var accion = () => Salida.CrearConFechaADefinir(
            "Cena a coordinar",
            null,
            new[] { FechaActual.AddDays(10) },
            FechaActual.AddDays(3),
            FechaActual.AddDays(6),
            "CENA-FECHAS",
            creador,
            FechaActual);

        Assert.Throws<DomainException>(accion);
    }

    [Fact]
    public void AgregarOpcionFecha_DeberiaRechazarUnaFechaDuplicada()
    {
        var salida = CrearSalidaConFechaADefinir();
        var participante = Assert.Single(salida.Participantes);
        var fechaExistente = salida.OpcionesFecha.First().FechaHora;

        var accion = () => salida.AgregarOpcionFecha(
            participante.Id,
            fechaExistente,
            FechaActual.AddDays(1));

        Assert.Throws<DomainException>(accion);
        Assert.Equal(2, salida.OpcionesFecha.Count);
    }

    [Fact]
    public void RegistrarDisponibilidadFecha_DeberiaActualizarLaRespuestaExistente()
    {
        var salida = CrearSalidaConFechaADefinir();
        var participante = Assert.Single(salida.Participantes);
        var opcion = salida.OpcionesFecha.First();

        var primeraRespuesta = salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opcion.Id,
            true,
            FechaActual.AddDays(4));
        var respuestaActualizada = salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opcion.Id,
            false,
            FechaActual.AddDays(5));

        var respuestaGuardada = Assert.Single(opcion.Disponibilidades);
        Assert.Equal(primeraRespuesta.Id, respuestaActualizada.Id);
        Assert.False(respuestaGuardada.Disponible);
    }

    [Fact]
    public void FinalizadaLaVotacion_DeberiaQuedarConfirmadaSinFechaUnica()
    {
        var salida = CrearSalidaConFechaADefinir();

        var estado = salida.ObtenerEstado(FechaActual.AddDays(7));

        Assert.Equal(EstadoSalida.Confirmada, estado);
        Assert.Null(salida.FechaEncuentro);
    }

    [Fact]
    public void ObtenerResultadoDisponibilidad_DeberiaDistinguirLasRespuestas()
    {
        var salida = CrearSalidaConFechaADefinir();
        var participante = Assert.Single(salida.Participantes);
        var opciones = salida.OpcionesFecha.ToArray();
        salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opciones[0].Id,
            true,
            FechaActual.AddDays(4));
        salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opciones[1].Id,
            false,
            FechaActual.AddDays(4));

        var resultado = salida.ObtenerResultadoDisponibilidad(
            FechaActual.AddDays(7));

        Assert.Equal(2, resultado.Opciones.Count);
        Assert.Equal(2, resultado.OpcionesDestacadas.Count);
        Assert.Single(resultado.Opciones.First().ParticipantesDisponibles);
        Assert.Single(resultado.Opciones.Last().ParticipantesNoDisponibles);
        Assert.All(
            resultado.Opciones,
            opcion => Assert.Empty(opcion.ParticipantesSinResponder));
    }

    [Fact]
    public void ObtenerResultadoDisponibilidad_DeberiaDestacarTresYConservarTodas()
    {
        var salida = Salida.CrearConFechaADefinir(
            "Cena a coordinar",
            null,
            new[]
            {
                FechaActual.AddDays(10),
                FechaActual.AddDays(11),
                FechaActual.AddDays(12),
                FechaActual.AddDays(13)
            },
            FechaActual.AddDays(3),
            FechaActual.AddDays(6),
            "CENA-CUATRO-FECHAS",
            CrearUsuario(),
            FechaActual);
        var participante = Assert.Single(salida.Participantes);
        var opciones = salida.OpcionesFecha.OrderBy(x => x.FechaHora).ToArray();

        salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opciones[0].Id,
            true,
            FechaActual.AddDays(4));
        salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opciones[1].Id,
            true,
            FechaActual.AddDays(4));
        salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opciones[2].Id,
            true,
            FechaActual.AddDays(4));
        salida.RegistrarDisponibilidadFecha(
            participante.Id,
            opciones[3].Id,
            false,
            FechaActual.AddDays(4));

        var resultado = salida.ObtenerResultadoDisponibilidad(
            FechaActual.AddDays(7));

        Assert.Equal(4, resultado.Opciones.Count);
        Assert.Equal(3, resultado.OpcionesDestacadas.Count);
        Assert.Contains(
            resultado.Opciones,
            x => x.OpcionFechaId == opciones[3].Id);
        Assert.DoesNotContain(
            resultado.OpcionesDestacadas,
            x => x.OpcionFechaId == opciones[3].Id);
        Assert.Null(salida.FechaEncuentro);
    }

    private static Salida CrearSalida()
    {
        var creador = CrearUsuario();

        return Salida.Crear(
            "Cena del sábado",
            null,
            FechaActual.AddDays(10),
            FechaActual.AddDays(3),
            FechaActual.AddDays(6),
            "CENA-1234",
            creador,
            FechaActual);
    }

    private static Salida CrearSalidaConFechaADefinir()
    {
        return Salida.CrearConFechaADefinir(
            "Cena a coordinar",
            null,
            new[]
            {
                FechaActual.AddDays(10),
                FechaActual.AddDays(11)
            },
            FechaActual.AddDays(3),
            FechaActual.AddDays(6),
            "CENA-FECHAS",
            CrearUsuario(),
            FechaActual);
    }

    private static Usuario CrearUsuario()
    {
        return Usuario.Crear(
            "Leo",
            "leo@email.com",
            "hash-de-prueba",
            FechaActual);
    }

}
