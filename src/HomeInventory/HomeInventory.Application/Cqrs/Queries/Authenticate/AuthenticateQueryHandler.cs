using DotNext;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal class AuthenticateQueryHandler : QueryHandler<AuthenticateQuery, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    protected override async Task<OneOf<AuthenticateResult, IError>> InternalHandle(AuthenticateQuery query, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(query, cancellationToken);
        if (!result.HasValue)
        {
            return new InvalidCredentialsError();
        }

        return await HandleAsync(result.Value, query.Password, cancellationToken);
    }

    private ValueTask<Optional<User>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken) =>
        _userRepository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);

    private async ValueTask<OneOf<AuthenticateResult, IError>> HandleAsync(User user, string password, CancellationToken cancellationToken)
    {
        if (IsPasswordMatch(user, password))
        {
            return new InvalidCredentialsError();
        }

        var token = await _tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticateResult(user.Id, token);
    }

    private static bool IsPasswordMatch(User user, string password) =>
        user.Password != password;
}
