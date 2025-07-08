using SmallService.Domain.Configuration;
using SmallService.Domain.Configuration.Framework;
using SmallService.Domain.Exceptions;
using SmallService.Shared;

namespace SmallService.Domain.ErrorResponses;

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