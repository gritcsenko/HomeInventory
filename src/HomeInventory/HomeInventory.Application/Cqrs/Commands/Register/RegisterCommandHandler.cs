using HomeInventory.Application.Interfaces.Messaging;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Cqrs.Commands.Register;

internal class RegisterCommandHandler : CommandHandler<RegisterCommand>
{
    private readonly IUserRepository _repository;
    private readonly IIdFactory<UserId> _userIdFactory;

    public RegisterCommandHandler(IUserRepository userRepository, IIdFactory<UserId> userIdFactory)
    {
        _repository = userRepository;
        _userIdFactory = userIdFactory;
    }

    protected override async Task<OneOf<Success, IError>> InternalHandle(RegisterCommand query, CancellationToken cancellationToken)
    {
        await using var unit = await _repository.WithUnitOfWorkAsync(cancellationToken);
        if (await IsUserHasEmailAsync(query, cancellationToken))
        {
            return new DuplicateEmailError();
        }

        await CreateUserAsync(query, cancellationToken);
        await unit.SaveChangesAsync(cancellationToken);

        return new Success();
    }

    private async Task<bool> IsUserHasEmailAsync(RegisterCommand request, CancellationToken cancellationToken) =>
        await _repository.IsUserHasEmailAsync(request.Email, cancellationToken);

    private async Task CreateUserAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var userId = _userIdFactory.CreateNew();
        var user = new User(userId)
        {
            Email = request.Email,
            Password = request.Password,
        };
        await _repository.AddAsync(user, cancellationToken);
    }
}
