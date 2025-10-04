using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class AmountTests : BaseTest
{
    private static readonly EnumerationItemsCollection<AmountUnit> _items = EnumerationItemsCollection.CreateFor<AmountUnit>();

    [Fact]
    public void Equals_Should_ReturnTrueIfSameValueAndUnit()
    {
        var a = new Amount(Fixture.Create<decimal>(), (AmountUnit)new Random().Peek(_items));
        var b = new Amount(a.Value, a.Unit);

        var result = a.Equals(b);

        result.Should().BeTrue();
    }

    [Fact]
    public void OpEquality_Should_ReturnTrueIfSameValueAndUnit()
    {
        var a = new Amount(Fixture.Create<decimal>(), (AmountUnit)new Random().Peek(_items));
        var b = new Amount(a.Value, a.Unit);

        var result = a == b;

        result.Should().BeTrue();
    }

    [Fact]
    public void OpInequality_Should_ReturnFalseIfSameValueAndUnit()
    {
        var a = new Amount(Fixture.Create<decimal>(), (AmountUnit)new Random().Peek(_items));
        var b = new Amount(a.Value, a.Unit);

        var result = a != b;

        result.Should().BeFalse();
    }
}
