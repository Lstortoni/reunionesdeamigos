using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Application.Services;
using ReunionesDeAmigos.Application.Tests.Fakes;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Tests.Services;

public sealed class SalidaServiceTests
{
    [Fact]
    public async Task CrearAsync_DeberiaGuardarLaSalidaConElCreadorComoParticipante()
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
        almacenamiento.Usuarios.Add(creador);
        var fechaEncuentroLocal = new DateTimeOffset(
            2026, 8, 11, 9, 0, 0, TimeSpan.FromHours(-3));

        var unitOfWork = new UnitOfWorkEnMemoria();
        var servicio = new SalidaService(
            new SalidaRepositoryEnMemoria(almacenamiento),
            new UsuarioRepositoryEnMemoria(almacenamiento),
            new LugarRepositoryEnMemoria(almacenamiento),
            unitOfWork,
            new ClockFijo(fechaActual),
            new CodigoAccesoGeneratorFijo("CENA-1234"));
        var request = new CrearSalidaRequest(
            "Cena del sábado",
            null,
            fechaEncuentroLocal,
            3,
            2,
            [
                CrearPropuestaManual("Pizzería"),
                CrearPropuestaManual("Parrilla"),
                CrearPropuestaManual("Casa de Fede")
            ]);

        // Act
        var resultado = await servicio.CrearAsync(
            request,
            creador.Id,
            CancellationToken.None);
        var misSalidas = await servicio.ObtenerMiasAsync(
            creador.Id,
            CancellationToken.None);

        // Assert
        var salidaGuardada = Assert.Single(almacenamiento.Salidas);
        var participante = Assert.Single(salidaGuardada.Participantes);
        Assert.Equal("CENA-1234", resultado.CodigoAcceso);
        Assert.Equal(TimeSpan.Zero, resultado.FechaEncuentro.Offset);
        Assert.Equal(
            fechaEncuentroLocal.UtcDateTime,
            resultado.FechaEncuentro.UtcDateTime);
        Assert.Equal(creador.Id, participante.UsuarioId);
        Assert.Equal(3, salidaGuardada.Propuestas.Count);
        Assert.Equal(3, resultado.Propuestas.Count);
        var resumen = Assert.Single(misSalidas);
        Assert.Equal(resultado.Id, resumen.Id);
        Assert.True(resumen.EsCreador);
        Assert.Equal(1, resumen.CantidadParticipantes);
        Assert.Equal(1, unitOfWork.CantidadGuardados);
    }

    private static CrearPropuestaInicialRequest CrearPropuestaManual(
        string nombre) =>
        new(
            TipoPropuesta.Manual,
            null,
            nombre,
            null,
            null);
}
