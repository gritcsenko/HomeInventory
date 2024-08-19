using HomeInventory.Domain.Primitives.Errors;
using LanguageExt.SomeHelp;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectBuilder<TSelf, TObject, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
    where TSelf : ValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : notnull, IValueObject<TObject>
    where TValue : notnull
{
    private static readonly Seq<Error> _valueNotSpecified = Seq.create(new ValueNotSpecifiedError()).Cast<Error>();

    private Option<TValue> _value = OptionNone.Default;

    public TSelf WithValue(TValue value)
    {
        _value = value.ToSome();
        return (TSelf)this;
    }

    public Validation<Error, TObject> Build() =>
        _value
            .Map(Validate)
            .Map(validation => validation.Map(ToObject))
            .Match(static x => x, static () => _valueNotSpecified);

    public void Reset() =>
        _value = OptionNone.Default;

    protected abstract TObject ToObject(TValue value);

    protected virtual Validation<Error, TValue> Validate(TValue value) =>
        value is null ? _valueNotSpecified : value;
}
