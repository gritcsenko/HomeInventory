using AutoMapper;
using FluentResults;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using OneOf;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler : CommandHandler<RegisterCommand, RegistrationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    protected override async Task<Result<RegistrationResult>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await IsUserHasEmailAsync(command, cancellationToken))
        {
            return Result.Fail<RegistrationResult>(new DuplicateEmailError());
        }

        var result = await CreateUserAsync(command, cancellationToken);

        return result.Match(
            user => new RegistrationResult(user.Id),
            _ => Result.Fail<RegistrationResult>(new UserCreationError()));
    }

    private async Task<bool> IsUserHasEmailAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var specification = _mapper.Map<FilterSpecification<User>>(request);
        return await _userRepository.HasAsync(specification, cancellationToken);
    }

    private async Task<OneOf<User, OneOf.Types.None>> CreateUserAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var specification = _mapper.Map<CreateUserSpecification>(request);
        return await _userRepository.CreateAsync(specification, cancellationToken);
    }
}
