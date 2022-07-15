using ErrorOr;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using MediatR;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
internal class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, ErrorOr<AuthenticateResult>>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;

    public AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<AuthenticateResult>> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.FindByEmailAsync(request.Email, cancellationToken);
        if (result.TryPickT1(out _, out var user))
        {
            return Domain.Errors.Authentication.InvalidCredentials;
        }

        if (user.Password != request.Password)
        {
            return Domain.Errors.Authentication.InvalidCredentials;
        }

        var token = await _tokenGenerator.GenerateTokenAsync(user, cancellationToken);
        return new AuthenticateResult(user.Id, token);
    }
}
