namespace CQRS.Pattern.Application.Common.Exceptions;

public interface IHasHttpStatus
{
    int StatusCode { get; }
    string Title { get; }
    string Type { get; }
    IDictionary<string, object?>? Extensions => null;
}
