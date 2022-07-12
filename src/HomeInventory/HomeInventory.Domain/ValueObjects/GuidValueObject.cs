namespace HomeInventory.Domain.ValueObjects;

public class GuidValueObject<TObject> : ValueObject<TObject, Guid>
    where TObject : notnull, GuidValueObject<TObject>
{
    protected GuidValueObject(Guid value, IEqualityComparer<Guid> equalityComparer)
        : base(value, equalityComparer)
    {
    }
}

public class GuidValueValidator : ValueValidator<Guid>
{
    public static IValueValidator<Guid> Default { get; } = new GuidValueValidator();
}

public abstract class GuidValueObjectFactory<TObject> : ValueObjectFactory<TObject, Guid>
    where TObject : notnull, GuidValueObject<TObject>
{
    protected GuidValueObjectFactory(IValueValidator<Guid>? validator = null)
        : base(validator ?? GuidValueValidator.Default, EqualityComparer<Guid>.Default)
    {
    }
}
