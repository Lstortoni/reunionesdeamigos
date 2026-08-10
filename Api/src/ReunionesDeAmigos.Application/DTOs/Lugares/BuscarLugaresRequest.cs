using ReunionesDeAmigos.Domain.Enums;

namespace ReunionesDeAmigos.Application.DTOs.Lugares;

public sealed record BuscarLugaresRequest(
    string? Texto,
    TipoLugar? Tipo,
    string? Barrio,
    Guid? CiudadId);
