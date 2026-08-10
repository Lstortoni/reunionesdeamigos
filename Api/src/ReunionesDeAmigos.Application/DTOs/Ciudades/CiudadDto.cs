namespace ReunionesDeAmigos.Application.DTOs.Ciudades;

public sealed record CiudadDto(
    Guid Id,
    string Nombre,
    string Provincia,
    string Pais);
