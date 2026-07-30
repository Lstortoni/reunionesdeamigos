namespace ReunionesDeAmigos.Application.DTOs.Propuestas;

public sealed record AgregarPropuestaManualRequest(
    string Nombre,
    string? Descripcion,
    string? Direccion);
