using System.Linq.Expressions;
using CQRS.Pattern.Domain.Entities;

namespace CQRS.Pattern.Application.AspNetUsers.DTOs;

public sealed record AspNetUserDto(
    Guid Id,
    string? UserName,
    string? Email,
    bool EmailConfirmed,
    string? PhoneNumber,
    bool PhoneNumberConfirmed)
{
    public static Expression<Func<AspNetUser, AspNetUserDto>> Projection =>
        u => new AspNetUserDto(
            u.Id,
            u.UserName,
            u.Email,
            u.EmailConfirmed,
            u.PhoneNumber,
            u.PhoneNumberConfirmed);
}
