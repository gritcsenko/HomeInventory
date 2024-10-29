using HomeInventory.Domain.Primitives.Errors;
using LanguageExt.SomeHelp;

namespace HomeInventory.Domain.Primitives;

public abstract class ValueObjectBuilder<TSelf, TObject, TValue> : IValueObjectBuilder<TSelf, TObject, TValue>
    where TSelf : ValueObjectBuilder<TSelf, TObject, TValue>
    where TObject : IValueObject<TObject>
    where TValue : notnull
{
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
            .Match(static x => x, static () => Statics.ValueNotSpecified);

    public void Reset() =>
        _value = OptionNone.Default;

    protected abstract TObject ToObject(TValue value);

    protected virtual Validation<Error, TValue> Validate(TValue? value) =>
        value is null ? Statics.ValueNotSpecified : value;
}

file static class Statics
{
    public static Seq<Error> ValueNotSpecified { get; } = Seq.create(new ValueNotSpecifiedError()).Cast<Error>();
}
