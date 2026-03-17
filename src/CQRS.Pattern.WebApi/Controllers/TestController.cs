using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CQRS.Pattern.WebApi.Controllers;

[ApiController]
[Route("api/test")]
public sealed class TestController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok("OK");
    }

    [HttpGet("private")]
    public IActionResult Private()
    {
        return Ok("OK");
    }
}
