using Asp.Versioning.Conventions;
using AutoMapper;
using FluentValidation;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ISender = MediatR.ISender;

namespace HomeInventory.Web;

internal class AuthenticationModule : ApiModule
{
    public AuthenticationModule()
    {
    }

    public override void AddRoutes(IEndpointRouteBuilder app)
    {
        var versionSet = GetVersionSet(app);

        var group = app.MapGroup("/api/authentication")
            .WithOpenApi()
            .RequireDynamicAuthorization(Permission.AccessPermissions)
            .WithApiVersionSet(versionSet)
        .MapToApiVersion(1);

        group.MapPost("register", RegisterAsync);

        group.MapPost("login", LoginAsync);
    }

    public static async Task<IResult> RegisterAsync([FromServices] HttpContext context, [FromServices] ISender mediator, [FromServices] IMapper mapper, [FromServices] IValidator<RegisterRequest> registerValidator, [FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var validationResult = await registerValidator.ValidateAsync(body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Problem(context, validationResult);
        }

        var command = mapper.Map<RegisterCommand>(body);
        var result = await mediator.Send(command, cancellationToken);
        return Match(context, result, x => Results.Ok(mapper.Map<RegisterResponse>(x)));
    }

    public static async Task<IResult> LoginAsync([FromServices] HttpContext context, [FromServices] ISender mediator, [FromServices] IMapper mapper, [FromServices] IValidator<LoginRequest> loginValidator, [FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var validationResult = await loginValidator.ValidateAsync(body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Problem(context, validationResult);
        }
        var query = mapper.Map<AuthenticateQuery>(body);
        var result = await mediator.Send(query, cancellationToken);
        return Match(context, result, x => Results.Ok(mapper.Map<LoginResponse>(x)));
    }
}
