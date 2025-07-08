using Serilog;

namespace CqrsService.Shared;

public static class ExceptionHandlerHelpers
{
    public static void HandleException(Exception exception, string className, string methodName, string operationDetail = default)
    {
        var errorCode = GenerateErrorCode(exception);

        Log.ForContext("ErrorCode", errorCode)
            .ForContext("ClassType", className)
            .ForContext("OperationType", methodName)
            .ForContext("OperationDetail", operationDetail)
            .ForContext("Diagnostics", new { exception, exception.InnerException }, destructureObjects: true)
            .Error("Exception occurred while processing operation");
    }

    public static string GetErrorCode(Exception ex)
    {
        //get the error code from the exception
        string errorCode = string.Empty;

        if (ex.Data.Contains("ErrorCode"))
        {
            errorCode = ex.Data["ErrorCode"].ToString();
        };

        return errorCode;
    }

    private static string GenerateErrorCode(Exception ex)
    {
        //check if the rethrown exception already has a error code
        //ensure's we log the same error code for exception rethrows and not a new one
        if (ex.Data.Contains("ErrorCode"))
        {
            return ex.Data["ErrorCode"].ToString();
        };

        var errorCode = Guid.NewGuid().ToString();
        ex.Data.Add("ErrorCode", errorCode);

        return errorCode;
    }
}