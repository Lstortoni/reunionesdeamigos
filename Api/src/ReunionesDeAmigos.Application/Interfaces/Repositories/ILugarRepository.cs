using ReunionesDeAmigos.Domain.Entities;
using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.Interfaces.Repositories;

public interface ILugarRepository
{
    Task<Lugar?> ObtenerPorIdAsync(
        Guid lugarId,
        CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Lugar>> BuscarAsync(
        string? texto,
        TipoLugar? tipo,
        string? barrio,
        Guid? ciudadId,
        CancellationToken cancellationToken);

    Task AgregarAsync(
        Lugar lugar,
        CancellationToken cancellationToken);
}
