using FluentResults;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using OneOf;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
internal class AuthenticateQueryHandler : IQueryHandler<AuthenticateQuery, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<Result<AuthenticateResult>> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(request, cancellationToken);
        if (result.TryPickT1(out _, out var user))
        {
            return Result.Fail<AuthenticateResult>(new InvalidCredentialsError());
        }

        if (user.Password != request.Password)
        {
            return Result.Fail<AuthenticateResult>(new InvalidCredentialsError());
        }

        var token = await _tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticateResult(user.Id, token);
    }

    private async Task<OneOf<User, OneOf.Types.NotFound>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken) =>
        await _userRepository.FindFirstByEmailOrNotFoundUserAsync(request.Email, cancellationToken);
}
