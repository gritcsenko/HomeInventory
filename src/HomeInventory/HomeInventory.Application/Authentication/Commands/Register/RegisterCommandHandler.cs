using ErrorOr;
using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain;
using HomeInventory.Domain.Entities;
using MapsterMapper;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Authentication.Commands.Register;
internal class RegisterCommandHandler : ICommandHandler<RegisterCommand, RegistrationResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public RegisterCommandHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ErrorOr<RegistrationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await IsUserHasEmailAsync(request, cancellationToken))
        {
            return Errors.User.DuplicateEmail;
        }

        var result = await CreateUserAsync(request, cancellationToken);

        return result.Match<ErrorOr<RegistrationResult>>(
            user => new RegistrationResult(user.Id),
            _ => Errors.User.UserCreation);
    }

    private async Task<bool> IsUserHasEmailAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var specification = _mapper.Map<FilterSpecification<User>>(request);
        return await _userRepository.HasAsync(specification, cancellationToken);
    }

    private async Task<OneOf<User, None>> CreateUserAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var specification = _mapper.Map<CreateUserSpecification>(request);
        return await _userRepository.CreateAsync(specification, cancellationToken);
    }
}
