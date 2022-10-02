using AutoFixture;
using FluentAssertions;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Tests.Helpers;

namespace HomeInventory.Tests.Domain.ValueObjects;

[Trait("Category", "Unit")]
public class AmountFactoryTests : BaseTest
{
    [Fact]
    public void Create_Should_Return_ValidatorNotFound_When_UnknownUnit()
    {
        var sut = CreateSut();
        var unknownUnit = new AmountUnit(Fixture.Create<string>(), MeasurementType.Area);

        var result = sut.Create(0m, unknownUnit);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Should().BeAssignableTo<ValidatorNotFoundError>();
    }

    [Fact]
    public void Create_Should_Return_Error_When_PieceIsFractional()
    {
        var sut = CreateSut();
        var value = 0.5m;

        var result = sut.Create(value, AmountUnit.Piece);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(NonNegativeUnitsData))]
    public void Create_Should_Return_Error_When_ValueIsNegative(AmountUnit unit)
    {
        var sut = CreateSut();
        var value = -1m;

        var result = sut.Create(value, unit);

        result.IsFailed.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(UnitsData))]
    public void Create_Should_Return_Amount_When_ValueIsValid(AmountUnit unit)
    {
        var sut = CreateSut();
        var value = 1m;

        var result = sut.Create(value, unit);

        result.IsFailed.Should().BeFalse();
        var amount = result.Value;
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

