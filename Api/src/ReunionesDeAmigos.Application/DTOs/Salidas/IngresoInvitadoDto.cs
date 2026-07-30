namespace ReunionesDeAmigos.Application.DTOs.Salidas;

public sealed record IngresoInvitadoDto(
    ParticipanteSalidaDto Participante,
    string Credencial);
