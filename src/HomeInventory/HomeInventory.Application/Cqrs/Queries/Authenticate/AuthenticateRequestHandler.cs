using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal sealed class AuthenticateRequestHandler(IAuthenticationTokenGenerator tokenGenerator, IScopeAccessor scopeAccessor, IPasswordHasher hasher) : QueryHandler<AuthenticateRequestMessage, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IPasswordHasher _hasher = hasher;

    protected override async Task<Validation<Error, AuthenticateResult>> InternalHandle(IRequestContext<AuthenticateRequestMessage> context)
    {
        var result = await TryFindUserAsync(context.Request, context.RequestAborted)
        .IfAsync((user, t) => IsPasswordMatchAsync(user, context.Request.Password, t), context.RequestAborted)
            .ConvertAsync(async (user, t) => (token: await _tokenGenerator.GenerateTokenAsync(user, t), id: user.Id), context.RequestAborted);

        if (!result.IsSome)
        {
            return new InvalidCredentialsError();
        }

        return result.Map(t => new AuthenticateResult(t.id, t.token)).ErrorIfNone(() => new InvalidCredentialsError());
    }

    private async Task<Option<User>> TryFindUserAsync(AuthenticateRequestMessage request, CancellationToken cancellationToken)
    {
        var repository = _scopeAccessor.GetRequiredContext<IUserRepository>();
        return await repository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);
    }

    private async Task<bool> IsPasswordMatchAsync(User user, string password, CancellationToken cancellationToken) =>
        await _hasher.VarifyHashAsync(password, user.Password, cancellationToken);
}
