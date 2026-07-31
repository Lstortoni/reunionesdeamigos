using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Domain.Tests.Entities;

public sealed class SalidaTests
{
    [Fact]
    public void AgregarPropuestaDeCatalogo_DeberiaAgregarElLugarALaSalida()
    {
        // Arrange
        var fechaActual = new DateTimeOffset(
            2026,
            8,
            1,
            12,
            0,
            0,
            TimeSpan.Zero);
        var creador = Usuario.Crear(
            "Leo",
            "leo@email.com",
            fechaActual);
        var salida = Salida.Crear(
            "Cena del sábado",
            null,
            fechaActual.AddDays(10),
            fechaActual.AddDays(3),
            fechaActual.AddDays(6),
            "CENA-1234",
            creador,
            fechaActual);
        var lugar = Lugar.Crear(
            "La Trattoria",
            "Restaurante italiano",
            "Calle 12",
            "Centro",
            "La Plata",
            TipoLugar.Restaurante);
        var participanteCreador = Assert.Single(
            salida.Participantes);

        // Act
        var propuesta = salida.AgregarPropuestaDeCatalogo(
            participanteCreador.Id,
            lugar,
            fechaActual.AddDays(1));

        // Assert
        var propuestaGuardada = Assert.Single(
            salida.Propuestas);
        Assert.Equal(propuesta.Id, propuestaGuardada.Id);
        Assert.Equal(lugar.Id, propuestaGuardada.LugarId);
    }
}
