using ErrorOr;

namespace HomeInventory.Domain.ValueObjects;

public class UserId : GuidValueObject<UserId>, IIdentityValue
{
    internal UserId(Guid value, IEqualityComparer<Guid> equalityComparer)
        : base(value, equalityComparer)
    {
    }
}

public class UserIdFactory : GuidValueObjectFactory<UserId>
{
    public UserIdFactory()
        : base(UserIdValidator.Default)
    {
    }

    protected override UserId CreateObject(Guid value) => new(value, EqualityComparer);
}

public static class UserIdFactoryExtensions
{
    public static ErrorOr<UserId> CreateNew(this IValueObjectFactory<UserId, Guid> factory) => factory.Create(Guid.NewGuid());
}

public class UserIdValidator : GuidValueValidator
{
    public static new IValueValidator<Guid> Default { get; } = new UserIdValidator();

    public override bool IsValid(Guid value) => base.IsValid(value) && !value.Equals(Guid.Empty);
}
