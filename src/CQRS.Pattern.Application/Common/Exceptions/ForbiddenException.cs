namespace CQRS.Pattern.Application.Common.Exceptions;

public class ForbiddenException : Exception, IHasHttpStatus
{
    public int StatusCode => 403;
    public string Title => "Forbidden";
    public string Type => "https://httpstatuses.com/403";

    public ForbiddenException()
        : base("You do not have permission to access this resource.")
    {
    }

    public ForbiddenException(string message)
        : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
