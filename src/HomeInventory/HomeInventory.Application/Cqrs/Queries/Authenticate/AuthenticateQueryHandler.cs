using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal class AuthenticateQueryHandler : QueryHandler<AuthenticateQuery, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _hasher;

    public AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository, IPasswordHasher hasher)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _hasher = hasher;
    }

    protected override async Task<OneOf<AuthenticateResult, IError>> InternalHandle(AuthenticateQuery query, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(query, cancellationToken);
        if (!result.HasValue)
        {
            return new InvalidCredentialsError();
        }

        var user = result.Value;
        if (!await IsPasswordMatchAsync(user, query.Password, cancellationToken))
        {
            return new InvalidCredentialsError();
        }

        var token = await _tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticateResult(user.Id, token);
    }

    private ValueTask<Optional<User>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken) =>
        _userRepository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);

    private async ValueTask<bool> IsPasswordMatchAsync(User user, string password, CancellationToken cancellationToken) =>
        await _hasher.VarifyHashAsync(password, user.Password, cancellationToken);
}
