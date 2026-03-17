using FluentValidation;
using MediatR;
using CQRS.Pattern.Domain.Entities;
using CQRS.Pattern.Application.Common.Interfaces;

namespace CQRS.Pattern.Application.AspNetUsers.Commands.CreateAspNetUser;

public sealed record CreateAspNetUserCommand(
    string UserName,
    string Email,
    string? PhoneNumber) : IRequest<Guid>;

internal sealed class CreateAspNetUserCommandHandler
    : IRequestHandler<CreateAspNetUserCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateAspNetUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(
        CreateAspNetUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = new AspNetUser
        {
            Id = Guid.NewGuid(),
            PhoneNumber = request.PhoneNumber,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString()
        };
        user.SetUserName(request.UserName);
        user.SetEmail(request.Email);

        _context.AspNetUsers.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}

internal sealed class CreateAspNetUserCommandValidator : AbstractValidator<CreateAspNetUserCommand>
{
    public CreateAspNetUserCommandValidator()
    {
        RuleFor(c => c.UserName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
