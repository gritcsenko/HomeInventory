﻿using FluentResults;
using HomeInventory.Application.Interfaces.Authentication;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.Errors;
using MapsterMapper;
using OneOf;

namespace HomeInventory.Application.Authentication.Queries.Authenticate;
internal class AuthenticateQueryHandler : IQueryHandler<AuthenticateQuery, AuthenticateResult>
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

    private async Task<OneOf<User, OneOf.Types.NotFound>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var specification = _mapper.Map<FilterSpecification<User>>(request);
        return await _userRepository.FindFirstOrNotFoundAsync(specification, cancellationToken);
    }
}
