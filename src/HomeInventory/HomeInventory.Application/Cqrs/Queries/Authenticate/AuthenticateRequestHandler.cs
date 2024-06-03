using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal sealed class AuthenticateRequestHandler(IAuthenticationTokenGenerator tokenGenerator, IScopeAccessor scopeAccessor, IPasswordHasher hasher) : IRequestHandler<AuthenticateRequestMessage, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly IPasswordHasher _hasher = hasher;

    public async Task<OneOf<AuthenticateResult, IError>> HandleAsync(IMessageHub hub, AuthenticateRequestMessage request, CancellationToken cancellationToken = default)
    {
        var repository = _scopeAccessor.TryGet<IUserRepository>().OrThrow<InvalidOperationException>();

        var query =
            from user in repository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken)
            where _hasher.VarifyHashAsync(request.Password, user.Password, cancellationToken)
            let token = _tokenGenerator.GenerateTokenAsync(user, cancellationToken)
            select CreateResultAsync(user, token);

        return await query.OrInvoke(() => new InvalidCredentialsError());

        static async Task<OneOf<AuthenticateResult, IError>> CreateResultAsync(User user, Task<string> token) =>
            new AuthenticateResult(user.Id, await token);
    }
}
