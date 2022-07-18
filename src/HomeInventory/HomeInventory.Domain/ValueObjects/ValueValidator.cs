namespace HomeInventory.Domain.ValueObjects;

public class ValueValidator<TValue> : IValueValidator<TValue>
{
    private readonly Func<TValue, bool> _validatorFunc;

    protected ValueValidator()
    {
        _validatorFunc = _ => true;
    }

    protected ValueValidator(Func<TValue, bool> validatorFunc)
    {
        _validatorFunc = validatorFunc;
    }

    public static IValueValidator<TValue> None { get; } = new ValueValidator<TValue>();

    public virtual bool IsValid(TValue value) => _validatorFunc(value);
}
