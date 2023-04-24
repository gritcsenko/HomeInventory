using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class AmountFactoryTests : BaseTest
{
    [Fact]
    public void Create_Should_Return_Value_When_UnknownUnit()
    {
        var sut = CreateSut();
        var value = 0m;
        var unknownUnit = new AmountUnit(Fixture.Create<string>(), MeasurementType.Area);

        var result = sut.Create(value, unknownUnit);

        result.IsT0.Should().BeTrue();
        var amount = result.AsT0;
        amount.Value.Should().Be(value);
        amount.Unit.Should().Be(unknownUnit);
    }

    [Fact]
    public void Create_Should_Return_Error_When_PieceIsFractional()
    {
        var sut = CreateSut();
        var value = 0.5m;

        var result = sut.Create(value, AmountUnit.Piece);

        result.IsT1.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(NonNegativeUnitsData))]
    public void Create_Should_Return_Error_When_ValueIsNegative(AmountUnit unit)
    {
        var sut = CreateSut();
        var value = -1m;

        var result = sut.Create(value, unit);

        result.IsT1.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(UnitsData))]
    public void Create_Should_Return_Amount_When_ValueIsValid(AmountUnit unit)
    {
        var sut = CreateSut();
        var value = 1m;

        var result = sut.Create(value, unit);

        result.IsT0.Should().BeTrue();
        var amount = result.AsT0;
        amount.Value.Should().Be(value);
        amount.Unit.Should().Be(unit);
    }

    private static AmountFactory CreateSut() => new();

    public static TheoryData<AmountUnit> NonNegativeUnitsData() =>
        new(){
            AmountUnit.Piece,
            AmountUnit.Gallon,
            AmountUnit.CubicMeter,
        };

    public static TheoryData<AmountUnit> UnitsData() =>
        new(){
            AmountUnit.Piece,
            AmountUnit.Gallon,
            AmountUnit.CubicMeter,
        };
}
