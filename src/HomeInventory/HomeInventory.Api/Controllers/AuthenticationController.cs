using HomeInventory.Application.Services.Authentication;
using HomeInventory.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Api.Controllers;

[Route("api/[controller]")]
public class AuthenticationController : ApiController
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var result = await _authenticationService.RegisterAsync(body.FirstName, body.LastName, body.Email, body.Password, cancellationToken);
        return Match(result, x => Ok(new RegisterResponse(x.Id)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var result = await _authenticationService.AuthenticateAsync(body.Email, body.Password, cancellationToken);
        return Match(result, x => Ok(new LoginResponse(x.Id, x.Token)));
    }
}
