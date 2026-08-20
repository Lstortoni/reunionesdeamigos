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
            unitOfWork,
            new ClockFijo(fechaActual),
            new CodigoAccesoGeneratorFijo("CENA-1234"),
            new EnlaceInvitacionGeneratorFijo("https://app.test"));
        var request = new CrearSalidaRequest(
            "Cena del sábado",
            null,
            ModalidadFecha.Fija,
            fechaEncuentroLocal,
            [],
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
        Assert.Equal(
            "https://app.test/unirse/CENA-1234",
            resultado.EnlaceInvitacion);
        Assert.NotNull(resultado.FechaEncuentro);
        Assert.Equal(TimeSpan.Zero, resultado.FechaEncuentro.Value.Offset);
        Assert.Equal(
            fechaEncuentroLocal.UtcDateTime,
            resultado.FechaEncuentro.Value.UtcDateTime);
        Assert.Equal(creador.Id, participante.UsuarioId);
        Assert.Equal(3, salidaGuardada.Propuestas.Count);
        Assert.Equal(3, resultado.Propuestas.Count);
        var resumen = Assert.Single(misSalidas);
        Assert.Equal(resultado.Id, resumen.Id);
        Assert.True(resumen.EsCreador);
        Assert.Equal(1, resumen.CantidadParticipantes);
        Assert.Equal(1, unitOfWork.CantidadGuardados);
    }

    [Fact]
    public async Task CrearAsync_ConFechaADefinir_DeberiaGuardarLasOpcionesIniciales()
    {
        var fechaActual = new DateTimeOffset(
            2026, 8, 1, 12, 0, 0, TimeSpan.Zero);
        var almacenamiento = new AlmacenamientoEnMemoria();
        var creador = Usuario.Crear(
            "Leo",
            "leo@email.com",
            "hash-de-prueba",
            fechaActual);
        almacenamiento.Usuarios.Add(creador);
        var opcionesLocales = new[]
        {
            new DateTimeOffset(2026, 8, 15, 21, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 8, 16, 13, 0, 0, TimeSpan.FromHours(-3)),
            new DateTimeOffset(2026, 8, 22, 21, 0, 0, TimeSpan.FromHours(-3))
        };
        var unitOfWork = new UnitOfWorkEnMemoria();
        var servicio = new SalidaService(
            new SalidaRepositoryEnMemoria(almacenamiento),
            new UsuarioRepositoryEnMemoria(almacenamiento),
            unitOfWork,
            new ClockFijo(fechaActual),
            new CodigoAccesoGeneratorFijo("FECHAS-1234"),
            new EnlaceInvitacionGeneratorFijo("https://app.test"));
        var request = new CrearSalidaRequest(
            "Cena a coordinar",
            null,
            ModalidadFecha.ADefinir,
            null,
            opcionesLocales,
            3,
            2,
            [
                CrearPropuestaManual("Pizzería"),
                CrearPropuestaManual("Parrilla"),
                CrearPropuestaManual("Casa de Fede")
            ]);

        var resultado = await servicio.CrearAsync(
            request,
            creador.Id,
            CancellationToken.None);

        var salidaGuardada = Assert.Single(almacenamiento.Salidas);
        Assert.Equal(ModalidadFecha.ADefinir, salidaGuardada.Modalidad);
        Assert.Null(salidaGuardada.FechaEncuentro);
        Assert.Equal(3, salidaGuardada.OpcionesFecha.Count);
        Assert.Equal(3, resultado.OpcionesFecha.Count);
        Assert.All(
            resultado.OpcionesFecha,
            opcion => Assert.Equal(TimeSpan.Zero, opcion.FechaHora.Offset));
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
