using ReunionesDeAmigos.Application.Services;
using ReunionesDeAmigos.Application.Tests.Fakes;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Tests.Services;

public sealed class PropuestaServiceTests
{
    [Fact]
    public async Task AgregarDeCatalogoAsync_DeberiaGuardarLaPropuestaEnLaSalida()
    {
        // Arrange
        var fechaActual = new DateTimeOffset(
            2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
        var almacenamiento = new AlmacenamientoEnMemoria();
        var creador = Usuario.Crear(
            "Leo",
            "leo@email.com",
            "hash-de-prueba",
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
        var ciudad = Ciudad.Crear(
            "La Plata",
            "Buenos Aires",
            "Argentina");
        var lugar = Lugar.Crear(
            "La Trattoria",
            null,
            "Calle 12",
            "Centro",
            ciudad,
            TipoLugar.Restaurante);
        almacenamiento.Salidas.Add(salida);
        almacenamiento.Lugares.Add(lugar);

        var unitOfWork = new UnitOfWorkEnMemoria();
        var servicio = new PropuestaService(
            new SalidaRepositoryEnMemoria(almacenamiento),
            new LugarRepositoryEnMemoria(almacenamiento),
            unitOfWork,
            new ClockFijo(fechaActual.AddDays(1)));
        var participante = Assert.Single(salida.Participantes);

        // Act
        var resultado = await servicio.AgregarDeCatalogoAsync(
            salida.Id,
            participante.Id,
            lugar.Id,
            CancellationToken.None);

        // Assert
        var propuestaGuardada = Assert.Single(salida.Propuestas);
        Assert.Equal(lugar.Id, resultado.LugarId);
        Assert.Equal(resultado.Id, propuestaGuardada.Id);
        Assert.Equal(1, unitOfWork.CantidadGuardados);
    }
}
