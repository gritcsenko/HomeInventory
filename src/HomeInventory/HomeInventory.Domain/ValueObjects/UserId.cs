using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public class UserId : GuidValueObject<UserId>, IIdentityValue
{
    internal UserId(Guid value, IEqualityComparer<Guid> equalityComparer)
        : base(value, equalityComparer)
    {
    }
}

public interface IUserIdFactory : IValueObjectFactory<UserId, Guid>
{
    ErrorOr<UserId> CreateNew();
}

public class UserIdFactory : GuidValueObjectFactory<UserId>, IUserIdFactory
{
    public UserIdFactory(IUserIdValidator validator)
        : base(validator)
    {
    }

    public ErrorOr<UserId> CreateNew() => Create(Guid.NewGuid());

    protected override UserId CreateObject(Guid value) => new(value, EqualityComparer);
}

public interface IUserIdValidator : IValueValidator<Guid>
{
}

public class UserIdValidator : GuidValueValidator, IUserIdValidator
{
    public override bool IsValid(Guid value) => base.IsValid(value) && !value.Equals(Guid.Empty);
}
