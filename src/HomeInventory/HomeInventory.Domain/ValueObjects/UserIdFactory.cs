using FluentResults;
using HomeInventory.Domain.Primitives;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

internal sealed class UserIdFactory : ValueObjectFactory<UserId>, IUserIdFactory
{
    public OneOf<UserId, IError> Create(Guid id) => id == Guid.Empty ? GetValidationError(id) : CreateCore(id);

    public UserId CreateNew() => CreateCore(Guid.NewGuid());

    private UserId CreateCore(Guid id) => new(id);
}
