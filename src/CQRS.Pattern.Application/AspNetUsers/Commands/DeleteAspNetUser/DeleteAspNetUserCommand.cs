using FluentValidation;
using MediatR;
using CQRS.Pattern.Application.Common.Exceptions;
using CQRS.Pattern.Application.Common.Interfaces;

namespace CQRS.Pattern.Application.AspNetUsers.Commands.DeleteAspNetUser;

public sealed record DeleteAspNetUserCommand(Guid Id) : IRequest;

internal sealed class DeleteAspNetUserCommandHandler
    : IRequestHandler<DeleteAspNetUserCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteAspNetUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(
        DeleteAspNetUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _context.AspNetUsers.FindAsync([request.Id], cancellationToken);

        if (user is null)
            throw new NotFoundException("AspNetUser", request.Id);

        _context.AspNetUsers.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

internal sealed class DeleteAspNetUserCommandValidator : AbstractValidator<DeleteAspNetUserCommand>
{
    public DeleteAspNetUserCommandValidator()
    {
        RuleFor(c => c.Id).NotEmpty();
    }
}
