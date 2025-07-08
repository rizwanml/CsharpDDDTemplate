namespace SmallService.Domain.Configuration.Framework;

/// <summary>
/// Using the Result Pattern, the response object is the expected result of a domain service process.
/// The response can contain content or an error response object in the event of any failures.
/// </summary>
public sealed class Response<T> : Response
{
    public T Content { get; }

    private Response(T content, Status status, ErrorResponse? errorResponse)
        : base(status, errorResponse)
    {
        Content = content;
    }

    public static Response<T> Success(T content)
    {
        return new Response<T>(content, Status.Success, null);
    }

    public static Response<T> Failure(T content)
    {
        return new Response<T>(content, Status.Fail, null);
    }

    public static Response<T> Failure(ErrorResponse errorResponse)
    {
        return new Response<T>(default(T)!, Status.Fail, errorResponse);
    }
}

public class Response
{
    public Status Status { get; }
    public ErrorResponse? ErrorResponse { get; }

    protected Response(Status status, ErrorResponse? errorResponse)
    {
        Status = status;
        ErrorResponse = errorResponse;
    }
}

public enum Status
{
    Success,
    Fail
}