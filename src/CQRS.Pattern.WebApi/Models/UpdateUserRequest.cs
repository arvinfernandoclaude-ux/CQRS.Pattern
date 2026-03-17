namespace CQRS.Pattern.WebApi.Models;

public sealed record UpdateUserRequest(
    string UserName,
    string Email,
    string? PhoneNumber);
