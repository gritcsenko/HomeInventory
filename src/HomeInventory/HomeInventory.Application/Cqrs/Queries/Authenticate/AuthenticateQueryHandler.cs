using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Application.Cqrs.Queries.Authenticate;

internal class AuthenticateQueryHandler : QueryHandler<AuthenticateQuery, AuthenticateResult>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeService _dateTimeService;
    public AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository, IDateTimeService dateTimeService)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _dateTimeService = dateTimeService;
    }

    protected override async Task<OneOf<AuthenticateResult, IError>> InternalHandle(AuthenticateQuery query, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(query, cancellationToken);
        if (result.TryPickT1(out _, out var user))
        {
            return new InvalidCredentialsError();
        }

        if (user.Password != query.Password)
        {
            return new InvalidCredentialsError();
        }

        var token = await _tokenGenerator.GenerateTokenAsync(user, _dateTimeService, cancellationToken);
        return new AuthenticateResult(user.Id, token);
    }

    private async Task<OneOf<User, OneOf.Types.NotFound>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken) =>
        await _userRepository.FindFirstByEmailOrNotFoundUserAsync(request.Email, cancellationToken);
}
