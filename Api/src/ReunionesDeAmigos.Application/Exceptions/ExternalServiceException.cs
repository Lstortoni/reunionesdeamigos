namespace ReunionesDeAmigos.Application.Exceptions;

public sealed class ExternalServiceException : Exception
{
    public ExternalServiceException(string message)
        : base(message)
    {
    }
}
