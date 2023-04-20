using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class AmountTests : BaseTest
{
    [Fact]
    public void Equals_Should_ReturnTrueIfSameValueAndUnit()
    {
        var a = new Amount(Fixture.Create<decimal>(), AmountUnit.Items.FirstRandom());
        var b = new Amount(a.Value, a.Unit);

        var result = a.Equals(b);

        result.Should().BeTrue();
    }

    [Fact]
    public void OpEquality_Should_ReturnTrueIfSameValueAndUnit()
    {
        var a = new Amount(Fixture.Create<decimal>(), AmountUnit.Items.FirstRandom());
        var b = new Amount(a.Value, a.Unit);

        var result = a == b;

        result.Should().BeTrue();
    }

    [Fact]
    public void OpInequality_Should_ReturnFalseIfSameValueAndUnit()
    {
        var a = new Amount(Fixture.Create<decimal>(), AmountUnit.Items.FirstRandom());
        var b = new Amount(a.Value, a.Unit);

        var result = a != b;

        result.Should().BeFalse();
    }

    public static TheoryData<AmountUnit> UnitsData() =>
        new(){
            AmountUnit.Piece,
            AmountUnit.Gallon,
            AmountUnit.CubicMeter,
        };
}
