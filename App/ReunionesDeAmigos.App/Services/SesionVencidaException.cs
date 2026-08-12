namespace ReunionesDeAmigos.App.Services;

public sealed class SesionVencidaException()
    : Exception("La sesión venció. Iniciá sesión nuevamente.");
