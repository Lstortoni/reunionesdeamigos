using ReunionesDeAmigos.Application.Services;
using ReunionesDeAmigos.Application.DTOs.Propuestas;
using ReunionesDeAmigos.Application.Tests.Fakes;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Tests.Services;

public sealed class PropuestaServiceTests
{
    [Fact]
    public async Task AgregarExternaAsync_DeberiaGuardarLaPropuestaEnLaSalida()
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
        almacenamiento.Salidas.Add(salida);
        const string googlePlaceId = "ChIJ-lugar-prueba";

        var unitOfWork = new UnitOfWorkEnMemoria();
        var servicio = new PropuestaService(
            new SalidaRepositoryEnMemoria(almacenamiento),
            unitOfWork,
            new ClockFijo(fechaActual.AddDays(1)));
        var participante = Assert.Single(salida.Participantes);

        // Act
        var resultado = await servicio.AgregarExternaAsync(
            salida.Id,
            participante.Id,
            new AgregarPropuestaExternaRequest(googlePlaceId),
            CancellationToken.None);

        // Assert
        var propuestaGuardada = Assert.Single(salida.Propuestas);
        Assert.Equal(googlePlaceId, resultado.GooglePlaceId);
        Assert.Equal(resultado.Id, propuestaGuardada.Id);
        Assert.Equal(1, unitOfWork.CantidadGuardados);
    }
}
