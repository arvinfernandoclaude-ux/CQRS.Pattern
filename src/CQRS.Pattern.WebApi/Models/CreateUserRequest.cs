namespace CQRS.Pattern.WebApi.Models;

public sealed record CreateUserRequest(
    string UserName,
    string Email,
    string? PhoneNumber);
