using CqrsService.Domain.Configuration;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.Exceptions;
using CqrsService.Shared;

namespace CqrsService.Domain.ErrorResponses;

public sealed class ExternalSystemErrorResponse : ErrorResponse
{
    public ExternalSystemErrorResponse() { }

    public ExternalSystemErrorResponse(ExternalSystemException externalSystemException)
    {
        ErrorCode = ExceptionHandlerHelpers.GetErrorCode(externalSystemException);
        ErrorReason = MessageContext.ExternalSystemError.ToString();
        ErrorMessage = MessageContext.ExternalSystemError.GetMessage();
    }
}