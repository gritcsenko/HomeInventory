namespace HomeInventory.Domain.ValueObjects;

public class ValueValidator<TValue> : IValueValidator<TValue>
{
    protected ValueValidator()
    {
    }

    public static IValueValidator<TValue> None { get; } = new ValueValidator<TValue>();

    public virtual bool IsValid(TValue value) => true;
}
