using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

internal class OptionAssertions<T> : ReferenceTypeAssertions<Option<T>, OptionAssertions<T>>
    where T : notnull
{
    public OptionAssertions(Option<T> value)
        : base(value) =>
        Identifier = Subject.GetType().GetFormattedName();

    protected override string Identifier { get; }

    public AndConstraint<OptionAssertions<T>> HaveNoValue() => BeSameAs(Option.None<T>());

    public AndConstraint<OptionAssertions<T>> HaveSomeValue() => Match(o => o.IsSome());

    public AndConstraint<ObjectAssertions> HaveSameValueAs(T value) => HaveSomeValue().And.Subject.Reduce(OrFail).Should().BeSameAs(value);

    private T OrFail() => throw new InvalidOperationException("This method should not be called");
}
