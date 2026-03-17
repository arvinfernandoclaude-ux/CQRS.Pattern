namespace CQRS.Pattern.Application.Common.Exceptions;

public class NotFoundException : Exception, IHasHttpStatus
{
    public int StatusCode => 404;
    public string Title => "Not Found";
    public string Type => "https://httpstatuses.com/404";

    public NotFoundException()
        : base()
    {
    }

    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) was not found.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
