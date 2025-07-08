using CqrsService.Domain.Configuration;
using CqrsService.Domain.Configuration.Framework;
using CqrsService.Shared;

namespace CqrsService.Domain.ErrorResponses;

public sealed class SystemErrorResponse : ErrorResponse
{
    public SystemErrorResponse() { }

    public SystemErrorResponse(Exception exception)
    {
        ErrorCode = ExceptionHandlerHelpers.GetErrorCode(exception);
        ErrorReason = MessageContext.SystemError.ToString();
        ErrorMessage = MessageContext.SystemError.GetMessage();
    }
}