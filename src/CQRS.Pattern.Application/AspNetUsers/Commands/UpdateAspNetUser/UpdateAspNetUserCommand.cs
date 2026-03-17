using FluentValidation;
using MediatR;
using CQRS.Pattern.Application.Common.Exceptions;
using CQRS.Pattern.Application.Common.Interfaces;

namespace CQRS.Pattern.Application.AspNetUsers.Commands.UpdateAspNetUser;

public sealed record UpdateAspNetUserCommand(
    Guid Id,
    string UserName,
    string Email,
    string? PhoneNumber) : IRequest;

internal sealed class UpdateAspNetUserCommandHandler
    : IRequestHandler<UpdateAspNetUserCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateAspNetUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        UpdateAspNetUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.AspNetUsers.FindAsync([request.Id], cancellationToken);

        if (user is null)
            throw new NotFoundException("AspNetUser", request.Id);

        user.SetUserName(request.UserName);
        user.SetEmail(request.Email);
        user.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class UpdateAspNetUserCommandValidator : AbstractValidator<UpdateAspNetUserCommand>
{
    public UpdateAspNetUserCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();

        RuleFor(c => c.UserName)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);
    }
}
