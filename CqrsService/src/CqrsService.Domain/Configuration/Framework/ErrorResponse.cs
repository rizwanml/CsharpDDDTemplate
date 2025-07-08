namespace CqrsService.Domain.Configuration.Framework;

/// <summary>
/// An error response object that can be extended to handle various error responses
/// </summary>
public abstract class ErrorResponse
{
    public string ErrorCode { get; set; }
    public string ErrorReason { get; set; }
    public string ErrorMessage { get; set; }
}