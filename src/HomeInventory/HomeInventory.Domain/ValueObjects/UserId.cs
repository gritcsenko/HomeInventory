using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;
public class UserId : GuidIdentifierObject<UserId>
{
    internal UserId(Guid value)
        : base(value)
    {
    }

    public static explicit operator Guid(UserId obj) => obj.Value;
}

public interface IUserIdFactory
{
    UserId CreateNew();
    ErrorOr<UserId> Create(Guid id);
}

internal class UserIdFactory : ValueObjectFactory<UserId>, IUserIdFactory
{
    public ErrorOr<UserId> Create(Guid id) => id == Guid.Empty ? GetValidationError() : CreateCore(id);

    public UserId CreateNew() => CreateCore(Guid.NewGuid());

    private UserId CreateCore(Guid id) => new(id);
}
