using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Api.Controllers;

[Route("api/[controller]")]
public class AuthenticationController : ApiController
{
    private readonly ISender _mediator;

    public AuthenticationController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var command = new RegisterCommand(body.FirstName, body.LastName, body.Email, body.Password);
        var result = await _mediator.Send(command, cancellationToken);
        return Match(result, x => Ok(new RegisterResponse(x.Id)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var query = new AuthenticateQuery(body.Email, body.Password);
        var result = await _mediator.Send(query, cancellationToken);
        return Match(result, x => Ok(new LoginResponse(x.Id, x.Token)));
    }
}
