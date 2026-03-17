using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using CQRS.Pattern.Application.AspNetUsers.DTOs;
using CQRS.Pattern.Application.Common.Exceptions;
using CQRS.Pattern.Application.Common.Interfaces;

namespace CQRS.Pattern.Application.AspNetUsers.Queries.GetAspNetUserById;

public sealed record GetAspNetUserByIdQuery(Guid Id) : IRequest<AspNetUserDto>;

internal sealed class GetAspNetUserByIdQueryHandler
    : IRequestHandler<GetAspNetUserByIdQuery, AspNetUserDto>
{
    private readonly IApplicationDbContext _context;

    public GetAspNetUserByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AspNetUserDto> Handle(
        GetAspNetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _context.AspNetUsers
            .AsNoTracking()
            .Where(u => u.Id == request.Id)
            .Select(AspNetUserDto.Projection)
            .FirstOrDefaultAsync(cancellationToken);

        if (user is null)
            throw new NotFoundException("AspNetUser", request.Id);

        return user;
    }
}

internal sealed class GetAspNetUserByIdQueryValidator : AbstractValidator<GetAspNetUserByIdQuery>
{
    public GetAspNetUserByIdQueryValidator()
    {
        RuleFor(q => q.Id).NotEmpty();
    }
}
