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
    public void AgregarPropuestaDeCatalogo_DeberiaAgregarElLugarALaSalida()
    {
        // Arrange
        var salida = CrearSalida();
        var lugar = CrearLugar();
        var participanteCreador = Assert.Single(
            salida.Participantes);

        // Act
        var propuesta = salida.AgregarPropuestaDeCatalogo(
            participanteCreador.Id,
            lugar,
            FechaActual.AddDays(1));

        // Assert
        var propuestaGuardada = Assert.Single(
            salida.Propuestas);
        Assert.Equal(propuesta.Id, propuestaGuardada.Id);
        Assert.Equal(lugar.Id, propuestaGuardada.LugarId);
    }

    [Fact]
    public void AgregarPropuestaDeCatalogo_DeberiaRechazarUnLugarDuplicado()
    {
        // Arrange
        var salida = CrearSalida();
        var lugar = CrearLugar();
        var participante = Assert.Single(salida.Participantes);
        salida.AgregarPropuestaDeCatalogo(
            participante.Id,
            lugar,
            FechaActual.AddDays(1));

        // Act
        var accion = () => salida.AgregarPropuestaDeCatalogo(
            participante.Id,
            lugar,
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

    private static Salida CrearSalida()
    {
        var creador = Usuario.Crear(
            "Leo",
            "leo@email.com",
            FechaActual);

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

    private static Lugar CrearLugar()
    {
        return Lugar.Crear(
            "La Trattoria",
            "Restaurante italiano",
            "Calle 12",
            "Centro",
            "La Plata",
            TipoLugar.Restaurante);
    }
}
