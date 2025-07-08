namespace CqrsService.Domain.Exceptions;

public sealed class ExternalSystemException : Exception
{
    public ExternalSystemException() { }

    public ExternalSystemException(string message)
        : base(message) { }

    public ExternalSystemException(string message, Exception inner)
        : base(message, inner) { }
}