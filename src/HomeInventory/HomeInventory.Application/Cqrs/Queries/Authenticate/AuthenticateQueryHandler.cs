using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Persistence;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal sealed class AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository, IPasswordHasher hasher) : QueryHandler<AuthenticateQuery, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _hasher = hasher;

    protected override async Task<Validation<Error, AuthenticateResult>> InternalHandle(AuthenticateQuery query, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(query, cancellationToken)
            .IfAsync((user, t) => IsPasswordMatchAsync(user, query.Password, t), cancellationToken)
            .ConvertAsync(async (user, t) => (token: await _tokenGenerator.GenerateTokenAsync(user, t), id: user.Id), cancellationToken);

        if (!result.IsSome)
        {
            return new InvalidCredentialsError();
        }

        return result.Map(t => new AuthenticateResult(t.id, t.token)).ErrorIfNone(() => new InvalidCredentialsError());
    }

    private async Task<Option<User>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken) =>
        await _userRepository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);

    private async Task<bool> IsPasswordMatchAsync(User user, string password, CancellationToken cancellationToken) =>
        await _hasher.VarifyHashAsync(password, user.Password, cancellationToken);
}
