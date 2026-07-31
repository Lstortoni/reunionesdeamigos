using ReunionesDeAmigos.Application.DTOs.Salidas;
using ReunionesDeAmigos.Application.Services;
using ReunionesDeAmigos.Application.Tests.Fakes;
using ReunionesDeAmigos.Domain.Entities;

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
            fechaActual);
        almacenamiento.Usuarios.Add(creador);

        var unitOfWork = new UnitOfWorkEnMemoria();
        var servicio = new SalidaService(
            new SalidaRepositoryEnMemoria(almacenamiento),
            new UsuarioRepositoryEnMemoria(almacenamiento),
            unitOfWork,
            new ClockFijo(fechaActual),
            new CodigoAccesoGeneratorFijo("CENA-1234"));
        var request = new CrearSalidaRequest(
            "Cena del sábado",
            null,
            fechaActual.AddDays(10),
            3,
            2);

        // Act
        var resultado = await servicio.CrearAsync(
            request,
            creador.Id,
            CancellationToken.None);

        // Assert
        var salidaGuardada = Assert.Single(almacenamiento.Salidas);
        var participante = Assert.Single(salidaGuardada.Participantes);
        Assert.Equal("CENA-1234", resultado.CodigoAcceso);
        Assert.Equal(creador.Id, participante.UsuarioId);
        Assert.Equal(1, unitOfWork.CantidadGuardados);
    }
}
