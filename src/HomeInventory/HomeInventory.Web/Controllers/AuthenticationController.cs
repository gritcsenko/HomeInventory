using AutoMapper;
using FluentValidation;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
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
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthenticationController(ISender mediator, IMapper mapper, IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator)
    {
        _mediator = mediator;
        _mapper = mapper;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }


    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var validationResult = await _registerValidator.ValidateAsync(body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Problem(validationResult);
        }

        var command = _mapper.Map<RegisterCommand>(body);
        var result = await _mediator.Send(command, cancellationToken);
        return Match(result, x => Ok(_mapper.Map<RegisterResponse>(x)));
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loginValidator.ValidateAsync(body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Problem(validationResult);
        }

        var query = _mapper.Map<AuthenticateQuery>(body);
        var result = await _mediator.Send(query, cancellationToken);
        return Match(result, x => Ok(_mapper.Map<LoginResponse>(x)));
    }
}
