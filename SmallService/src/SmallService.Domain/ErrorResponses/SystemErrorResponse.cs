using SmallService.Domain.Configuration;
using SmallService.Domain.Configuration.Framework;
using SmallService.Shared;

namespace SmallService.Domain.ErrorResponses;

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