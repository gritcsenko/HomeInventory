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
}

internal class UserIdFactory : ValueObjectFactory<UserId>, IUserIdFactory
{
    public UserId CreateNew() => new(Guid.NewGuid());
}
