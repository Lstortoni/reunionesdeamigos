namespace ReunionesDeAmigos.App.Services;

public sealed class ApiException(string message) : Exception(message);
