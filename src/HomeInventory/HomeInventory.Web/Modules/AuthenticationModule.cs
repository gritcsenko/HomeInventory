namespace HomeInventory.Web.Modules;

internal class AuthenticationModule : ApiModule
{
    public AuthenticationModule()
        : base("/api/authentication", Permission.AccessPermissions)
    {
    }

    protected override void AddRoutes(RouteGroupBuilder group)
    {
        group.MapPost("register", RegisterAsync)
            .AllowAnonymous()
            .WithValidationOf<RegisterRequest>();

        group.MapPost("login", LoginAsync)
            .AllowAnonymous()
            .WithValidationOf<LoginRequest>();
    }

    public static async Task<IResult> RegisterAsync(HttpContext context, [FromBody] RegisterRequest body, CancellationToken cancellationToken = default)
    {
        var mapper = context.GetMapper();

        var command = mapper.Map<RegisterCommand>(body);
        var result = await context.GetSender().Send(command, cancellationToken);
        return context.MatchToOk(result, mapper.Map<RegisterResponse>);
    }

    public static async Task<IResult> LoginAsync(HttpContext context, [FromBody] LoginRequest body, CancellationToken cancellationToken = default)
    {
        var mapper = context.GetMapper();

        var query = mapper.Map<AuthenticateQuery>(body);
        var result = await context.GetSender().Send(query, cancellationToken);
        return context.MatchToOk(result, mapper.Map<LoginResponse>);
    }
}
