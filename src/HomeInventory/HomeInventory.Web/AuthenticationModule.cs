using Asp.Versioning.Conventions;
using HomeInventory.Application.Authentication.Commands.Register;
using HomeInventory.Application.Authentication.Queries.Authenticate;
using HomeInventory.Contracts;
using HomeInventory.Web.Authorization.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

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

    public static async Task<IResult> RegisterAsync([FromServices] HttpContext context, [FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(context, body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Problem(context, validationResult);
        }

        var mapper = GetMapper(context);
        var command = mapper.Map<RegisterCommand>(body);
        var result = await GetSender(context).Send(command, cancellationToken);
        return Match(context, result, x => Results.Ok(mapper.Map<RegisterResponse>(x)));
    }

    public static async Task<IResult> LoginAsync([FromServices] HttpContext context, [FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateAsync(context, body, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Problem(context, validationResult);
        }
        var mapper = GetMapper(context);
        var query = mapper.Map<AuthenticateQuery>(body);
        var result = await GetSender(context).Send(query, cancellationToken);
        return Match(context, result, x => Results.Ok(mapper.Map<LoginResponse>(x)));
    }
}
