using ErrorOr;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using MapsterMapper;
using MediatR;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
internal class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, ErrorOr<AuthenticateResult>>
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public AuthenticateQueryHandler(IAuthenticationTokenGenerator tokenGenerator, IUserRepository userRepository, IMapper mapper)
    {
        _tokenGenerator = tokenGenerator;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<AuthenticateResult>> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var result = await TryFindUserAsync(request, cancellationToken);
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

    private async Task<OneOf<User, NotFound>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var specification = _mapper.Map<UserHasEmailSpecification>(request);
        return await _userRepository.FindFirstOrNotFoundAsync(specification, cancellationToken);
    }
}
