using HomeInventory.Domain.Primitives.Errors;
using OneOf;

namespace HomeInventory.Domain.ValueObjects;

public interface IUserIdFactory
{
    UserId CreateNew();

    OneOf<UserId, IError> Create(Guid id);
}
