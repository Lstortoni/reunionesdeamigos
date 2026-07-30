namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record ParticipanteSalidaDto(
    Guid Id,
    Guid? UsuarioId,
    string NombreVisible,
    DateTimeOffset FechaIngreso,
    bool EsInvitado);
