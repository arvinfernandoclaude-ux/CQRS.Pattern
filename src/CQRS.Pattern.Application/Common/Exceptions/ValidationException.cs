using FluentValidation.Results;

namespace CQRS.Pattern.Application.Common.Exceptions;

public class ValidationException : Exception, IHasHttpStatus
{
    public IDictionary<string, string[]> Errors { get; }

    public int StatusCode => 400;
    public string Title => "Validation Failed";
    public string Type => "https://httpstatuses.com/400";
    IDictionary<string, object?>? IHasHttpStatus.Extensions =>
        new Dictionary<string, object?> { ["errors"] = Errors };

    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
