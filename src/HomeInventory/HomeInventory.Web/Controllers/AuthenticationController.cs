using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeInventory.Web.Controllers;

[Route("api/[controller]")]
[AllowAnonymous]
public class AuthenticationController : ApiControllerBase
{
    private readonly ISender _mediator;
    private readonly IMapper _mapper;

    public AuthenticationController(ISender mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var command = _mapper.Map<RegisterCommand>(body);
        var result = await _mediator.Send(command, cancellationToken);
        return Match(result, x => Ok(_mapper.Map<RegisterResponse>(x)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var query = _mapper.Map<AuthenticateQuery>(body);
        var result = await _mediator.Send(query, cancellationToken);
        return Match(result, x => Ok(_mapper.Map<LoginResponse>(x)));
    }
}
