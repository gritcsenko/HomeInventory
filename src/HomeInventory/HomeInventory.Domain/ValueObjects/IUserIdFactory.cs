using FluentResults;

namespace HomeInventory.Domain.ValueObjects;

public interface IUserIdFactory
{
    UserId CreateNew();
    Result<UserId> Create(Guid id);
}
