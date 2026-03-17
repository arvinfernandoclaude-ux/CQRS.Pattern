using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CQRS.Pattern.Application.AspNetUsers.Services;
using CQRS.Pattern.WebApi.Models;

namespace CQRS.Pattern.WebApi.Controllers;

/// <summary>
/// Service Layer Pattern — uses IUserService directly, no MediatR.
/// Compare with <see cref="AspNetUsersCqrsController"/> which uses the CQRS pattern.
/// </summary>
[Authorize]
[ApiController]
[Route("api/aspnetusers")]
public sealed class AspNetUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AspNetUsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userService.GetAllAsync(page, pageSize, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var id = await _userService.CreateAsync(
            request.UserName, request.Email, request.PhoneNumber, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        await _userService.UpdateAsync(
            id, request.UserName, request.Email, request.PhoneNumber, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
