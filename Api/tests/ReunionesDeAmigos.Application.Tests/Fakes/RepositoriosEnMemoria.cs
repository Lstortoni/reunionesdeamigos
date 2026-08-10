using ReunionesDeAmigos.Application.Interfaces.Repositories;
using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Tests.Fakes;

internal sealed class SalidaRepositoryEnMemoria(
    AlmacenamientoEnMemoria almacenamiento) : ISalidaRepository
{
    public Task<Salida?> ObtenerPorIdAsync(
        Guid salidaId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            almacenamiento.Salidas.FirstOrDefault(salida => salida.Id == salidaId));
    }

    public Task<Salida?> ObtenerPorCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            almacenamiento.Salidas.FirstOrDefault(
                salida => salida.CodigoAcceso == codigoAcceso));
    }

    public Task<IReadOnlyCollection<Salida>> ObtenerPorUsuarioAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        var salidas = almacenamiento.Salidas
            .Where(salida => salida.TieneParticipanteRegistrado(usuarioId))
            .OrderBy(salida => salida.FechaEncuentro)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<Salida>>(salidas);
    }

    public Task<bool> ExisteCodigoAsync(
        string codigoAcceso,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            almacenamiento.Salidas.Any(
                salida => salida.CodigoAcceso == codigoAcceso));
    }

    public Task AgregarAsync(
        Salida salida,
        CancellationToken cancellationToken)
    {
        almacenamiento.Salidas.Add(salida);
        return Task.CompletedTask;
    }
}

internal sealed class UsuarioRepositoryEnMemoria(
    AlmacenamientoEnMemoria almacenamiento) : IUsuarioRepository
{
    public Task<Usuario?> ObtenerPorIdAsync(
        Guid usuarioId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            almacenamiento.Usuarios.FirstOrDefault(usuario => usuario.Id == usuarioId));
    }

    public Task<Usuario?> ObtenerPorEmailAsync(
        string email,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            almacenamiento.Usuarios.FirstOrDefault(
                usuario => usuario.Email == email));
    }

    public Task AgregarAsync(
        Usuario usuario,
        CancellationToken cancellationToken)
    {
        almacenamiento.Usuarios.Add(usuario);
        return Task.CompletedTask;
    }
}

internal sealed class LugarRepositoryEnMemoria(
    AlmacenamientoEnMemoria almacenamiento) : ILugarRepository
{
    public Task<Lugar?> ObtenerPorIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(
            almacenamiento.Lugares.FirstOrDefault(lugar => lugar.Id == lugarId));
    }

    public Task<IReadOnlyCollection<Lugar>> BuscarAsync(
        string? texto,
        TipoLugar? tipo,
        string? barrio,
        Guid? ciudadId,
        CancellationToken cancellationToken)
    {
        var lugares = almacenamiento.Lugares
            .Where(lugar => texto is null
                || lugar.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase))
            .Where(lugar => !tipo.HasValue || lugar.Tipo == tipo.Value)
            .Where(lugar => barrio is null
                || string.Equals(lugar.Barrio, barrio, StringComparison.OrdinalIgnoreCase))
            .Where(lugar => !ciudadId.HasValue
                || lugar.CiudadId == ciudadId.Value)
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<Lugar>>(lugares);
    }

    public Task AgregarAsync(
        Lugar lugar,
        CancellationToken cancellationToken)
    {
        almacenamiento.Lugares.Add(lugar);
        return Task.CompletedTask;
    }
}

internal sealed class UnitOfWorkEnMemoria : IUnitOfWork
{
    public int CantidadGuardados { get; private set; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        CantidadGuardados++;
        return Task.FromResult(0);
    }
}
