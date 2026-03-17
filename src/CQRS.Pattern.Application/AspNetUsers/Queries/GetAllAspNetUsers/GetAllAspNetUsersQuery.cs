using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CQRS.Pattern.Application.AspNetUsers.DTOs;
using CQRS.Pattern.Application.Common.Interfaces;
using CQRS.Pattern.Application.Common.Models;

namespace CQRS.Pattern.Application.AspNetUsers.Queries.GetAllAspNetUsers;

public sealed record GetAllAspNetUsersQuery(int Page = 1, int PageSize = 20)
    : IRequest<PaginatedList<AspNetUserDto>>;

internal sealed class GetAllAspNetUsersQueryHandler
    : IRequestHandler<GetAllAspNetUsersQuery, PaginatedList<AspNetUserDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllAspNetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<AspNetUserDto>> Handle(
        GetAllAspNetUsersQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.AspNetUsers
            .AsNoTracking()
            .OrderBy(u => u.UserName)
            .Select(AspNetUserDto.Projection);

        return await PaginatedList.CreateAsync(
            query, request.Page, request.PageSize, cancellationToken);
    }
}

internal sealed class GetAllAspNetUsersQueryValidator : AbstractValidator<GetAllAspNetUsersQuery>
{
    public GetAllAspNetUsersQueryValidator()
    {
        RuleFor(q => q.Page).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).InclusiveBetween(1, 100);
    }
}
