using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;

namespace HomeInventory.Application.UserManagement.Services;

internal sealed class AuthenticationService(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository, IPasswordHasher hasher) : IAuthenticationService
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher _hasher = hasher;

    public async Task<Validation<Error, AuthenticateResult>> AuthenticateAsync(AuthenticateQuery query, CancellationToken cancellationToken = default)
    {
        var result = await TryFindUserAsync(query.Email, cancellationToken)
            .IfAsync((user, t) => IsPasswordMatchAsync(user, query.Password, t), cancellationToken)
            .ConvertAsync(async (user, t) => (token: await _tokenGenerator.GenerateTokenAsync(user, t), id: user.Id), cancellationToken);

        return result.Map(t => new AuthenticateResult(t.id, t.token))
            .ErrorIfNone(() => new InvalidCredentialsError());
    }

    private async Task<Option<User>> TryFindUserAsync(Email email, CancellationToken cancellationToken) =>
        await _userRepository.FindFirstByEmailUserOptionalAsync(email, cancellationToken);

    private async Task<bool> IsPasswordMatchAsync(User user, string password, CancellationToken cancellationToken) =>
        await _hasher.VarifyHashAsync(password, user.Password, cancellationToken);
}
