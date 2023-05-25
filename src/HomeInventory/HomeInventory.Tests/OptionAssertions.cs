using HomeInventory.Domain.Primitives;

namespace HomeInventory.Tests;

internal class OptionAssertions<T> : ReferenceTypeAssertions<Optional<T>, OptionAssertions<T>>
    where T : notnull
{
    public OptionAssertions(Optional<T> value)
        : base(value) =>
        Identifier = Subject.GetType().GetFormattedName();

    protected override string Identifier { get; }

    public AndConstraint<OptionAssertions<T>> HaveNoValue() => Match(o => !o.HasValue);

    public AndConstraint<OptionAssertions<T>> HaveSomeValue() => Match(o => o.HasValue);

    public AndConstraint<ObjectAssertions> HaveSameValueAs(T value) => HaveSomeValue().And.Subject.OrThrow(Fail).Should().BeSameAs(value);

    public AndConstraint<ObjectAssertions> HaveValue(T value) => HaveSomeValue().And.Subject.OrThrow(Fail).Should().Be(value);

    private Exception Fail() => throw new InvalidOperationException("This method should not be called");
}
