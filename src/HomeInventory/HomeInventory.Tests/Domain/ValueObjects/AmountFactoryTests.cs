using HomeInventory.Domain.Primitives.Ids;
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
        var unknownUnit = AmountUnit.Create(MeasurementType.Area, IdSuppliers.Ulid, Fixture.Create<string>());

        var result = sut.Create(value, unknownUnit);

        using var scope = new AssertionScope();
        result.IsSuccess.Should().BeTrue();
        var amount = (Amount)result;
        amount.Value.Should().Be(value);
        amount.Unit.Should().Be(unknownUnit);
    }

    [Fact]
    public void Create_Should_Return_Value_When_UnknownMeasurementType()
    {
        var sut = CreateSut();
        var value = 0m;
        var unknownUnit = AmountUnit.Create(MeasurementType.Create(IdSuppliers.Ulid, Fixture.Create<string>()), IdSuppliers.Ulid, Fixture.Create<string>());

        var result = sut.Create(value, unknownUnit);

        using var scope = new AssertionScope();
        result.IsSuccess.Should().BeTrue();
        var amount = (Amount)result;
        amount.Value.Should().Be(value);
        amount.Unit.Should().Be(unknownUnit);
    }

    [Fact]
    public void Create_Should_Return_Error_When_PieceIsFractional()
    {
        var sut = CreateSut();
        var value = 0.5m;

        var result = sut.Create(value, AmountUnit.Piece);

        result.IsFail.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Return_Error_When_ValueIsNegative()
    {
        var units = typeof(AmountUnit).GetFieldValuesOfType<AmountUnit>()
            .Except([AmountUnit.Celsius]);
        using var scope = new AssertionScope();
        var sut = CreateSut();
        var value = -1m;

        foreach (var unit in units)
        {
            var result = sut.Create(value, unit);

            result.IsFail.Should().BeTrue(unit.Name);
        }
    }

    [Fact]
    public void Create_Should_Return_Amount_When_ValueIsValid()
    {
        var sut = CreateSut();
        var value = 1m;

        var units = typeof(AmountUnit).GetFieldValuesOfType<AmountUnit>();
        using var scope = new AssertionScope();

        foreach (var unit in units)
        {
            var result = sut.Create(value, unit);

            result.IsSuccess.Should().BeTrue(unit.Name);
            var amount = (Amount)result;
            amount.Value.Should().Be(value, unit.Name);
            amount.Unit.Should().Be(unit);
        }
    }

    private static AmountFactory CreateSut() => new();
}
