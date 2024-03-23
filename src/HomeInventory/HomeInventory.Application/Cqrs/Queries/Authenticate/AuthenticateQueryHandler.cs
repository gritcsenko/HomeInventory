using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal sealed class AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository, IPasswordHasher hasher) : QueryHandler<AuthenticateQuery, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _hasher = hasher;

    protected override async Task<OneOf<AuthenticateResult, IError>> InternalHandle(AuthenticateQuery query, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(query, cancellationToken)
            .IfAsync((user, t) => IsPasswordMatchAsync(user, query.Password, t), cancellationToken)
            .ConvertAsync(async (user, t) => (token: await _tokenGenerator.GenerateTokenAsync(user, t), id: user.Id), cancellationToken);

        if (!result.HasValue)
        {
            return new InvalidCredentialsError();
        }

        return new AuthenticateResult(result.Value.id, result.Value.token);
    }

    private ValueTask<Optional<User>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken) =>
        _userRepository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);

    private async ValueTask<bool> IsPasswordMatchAsync(User user, string password, CancellationToken cancellationToken) =>
        await _hasher.VarifyHashAsync(password, user.Password, cancellationToken);
}
