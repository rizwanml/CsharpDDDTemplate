using CqrsService.Domain.Configuration;
using CqrsService.Domain.Configuration.Framework;

namespace CqrsService.Domain.ErrorResponses;

/// <summary>
/// Domain validation error response is a response object that provides details related to an invalid domain entity in the domain layer
/// </summary>
public sealed class DomainValidationErrorResponse : ErrorResponse
{
    public object Content { get; set; }
    public List<ValidationError> ValidationErrors { get; set; }

    public DomainValidationErrorResponse() { }

    internal DomainValidationErrorResponse(object content, List<ValidationError> validationErrors)
    {
        ErrorCode = Guid.NewGuid().ToString();
        Content = content;
        ValidationErrors = validationErrors;
        ErrorReason = MessageContext.ValidationError.ToString();
        ErrorMessage = MessageContext.ValidationError.GetMessage();
    }

    internal DomainValidationErrorResponse(object content, string propertyName, MessageContext messageContext)
    {
        ErrorCode = Guid.NewGuid().ToString();
        Content = content;
        ValidationErrors = new List<ValidationError>
        {
            new ValidationError(propertyName, messageContext.GetMessage())
        };
        ErrorReason = MessageContext.ValidationError.ToString();
        ErrorMessage = MessageContext.ValidationError.GetMessage();
    }
}