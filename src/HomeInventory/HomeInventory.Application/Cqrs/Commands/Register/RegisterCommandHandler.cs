using AutoMapper;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;
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

    protected override async Task<OneOf<RegistrationResult, IError>> InternalHandle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await IsUserHasEmailAsync(command, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        var result = await CreateUserAsync(command, cancellationToken);

        return result.Match<OneOf<RegistrationResult, IError>>(
            user => new RegistrationResult(user.Id),
            _ => new UserCreationError());
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
