using HomeInventory.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Api.Controllers;

[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        return Ok(body);
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        return Ok(body);
    }
}
