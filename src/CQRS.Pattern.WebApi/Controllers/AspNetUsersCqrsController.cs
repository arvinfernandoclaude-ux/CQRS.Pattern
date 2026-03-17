using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CQRS.Pattern.Application.AspNetUsers.Commands.CreateAspNetUser;
using CQRS.Pattern.Application.AspNetUsers.Commands.DeleteAspNetUser;
using CQRS.Pattern.Application.AspNetUsers.Commands.UpdateAspNetUser;
using CQRS.Pattern.Application.AspNetUsers.Queries.GetAllAspNetUsers;
using CQRS.Pattern.Application.AspNetUsers.Queries.GetAspNetUserById;
using CQRS.Pattern.WebApi.Models;

namespace CQRS.Pattern.WebApi.Controllers;

/// <summary>
/// CQRS Pattern — uses MediatR commands/queries with pipeline behaviours.
/// Compare with <see cref="AspNetUsersController"/> which uses the Service Layer pattern.
/// </summary>
[Authorize]
[ApiController]
[Route("api/cqrs/aspnetusers")]
public sealed class AspNetUsersCqrsController : ControllerBase
{
    private readonly ISender _sender;

    public AspNetUsersCqrsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(new GetAllAspNetUsersQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAspNetUserByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateAspNetUserCommand(request.UserName, request.Email, request.PhoneNumber);
        var id = await _sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateAspNetUserCommand(id, request.UserName, request.Email, request.PhoneNumber);
        await _sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteAspNetUserCommand(id), cancellationToken);
        return NoContent();
    }
}
