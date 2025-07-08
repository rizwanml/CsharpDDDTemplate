namespace SmallService.Domain.Configuration.Framework;

/// <summary>
/// Validation error provides details related to an invalid domain entity property
/// </summary>
public sealed class ValidationError
{
    public string Message { get; private set; }
    public string PropertyName { get; private set; }

    internal ValidationError(string propertyName, string message)
    {
        PropertyName = propertyName;
        Message = message;
    }
}