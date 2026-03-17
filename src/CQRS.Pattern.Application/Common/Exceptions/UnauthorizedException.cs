namespace CQRS.Pattern.Application.Common.Exceptions;

public class UnauthorizedException : Exception, IHasHttpStatus
{
    public int StatusCode => 401;
    public string Title => "Unauthorized";
    public string Type => "https://httpstatuses.com/401";

    public UnauthorizedException()
        : base("Authentication is required.")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
