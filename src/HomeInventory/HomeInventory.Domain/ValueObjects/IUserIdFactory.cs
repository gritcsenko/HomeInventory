using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public interface IUserIdFactory
{
    UserId CreateNew();
    ErrorOr<UserId> Create(Guid id);
}
