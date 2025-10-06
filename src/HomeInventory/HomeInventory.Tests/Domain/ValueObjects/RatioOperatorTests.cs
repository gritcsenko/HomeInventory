using HomeInventory.Domain.Primitives.Units;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class RatioOperatorTests : BaseTest
{
    public RatioOperatorTests() =>
        Fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));

    [Fact]
    public void OpEquality_Should_ReturnTrueForEqualRatios()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator, denominator);
        var ratio2 = new Ratio(numerator, denominator);

        // When
        var actual = ratio1 == ratio2;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpInequality_Should_ReturnTrueForDifferentRatios()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = numerator1 + 1;
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);

        // When
        var actual = ratio1 != ratio2;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpEquality_Should_HandleNullLeft()
    {
        // Given
        Ratio? ratio1 = null;
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio2 = new Ratio(numerator, denominator);

        // When
#pragma warning disable CA1508 // Avoid dead conditional code
        var actual1 = ratio1 == ratio2;
        var actual2 = ratio2 == ratio1;
#pragma warning restore CA1508

        // Then
        using var _ = new AssertionScope();
        actual1.Should().BeFalse();
        actual2.Should().BeFalse();
    }

    [Fact]
    public void OpEquality_Should_HandleNullRight()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator, denominator);
        Ratio? ratio2 = null;

        // When
#pragma warning disable CA1508 // Avoid dead conditional code
        var actual1 = ratio1 == ratio2;
        var actual2 = ratio2 == ratio1;
#pragma warning restore CA1508

        // Then
        using var _ = new AssertionScope();
        actual1.Should().BeFalse();
        actual2.Should().BeFalse();
    }

    [Fact]
    public void OpEquality_Should_HandleBothNull()
    {
        // Given
        Ratio? ratio1 = null;
        Ratio? ratio2 = null;

        // When
#pragma warning disable CA1508 // Avoid dead conditional code
        // ReSharper disable once EqualExpressionComparison
        var actual = ratio1 == ratio2;
#pragma warning restore CA1508

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpGreaterThan_Should_ReturnTrueWhenGreater()
    {
        // Given
        Fixture.Customizations.Add(new LargerMagnitudeFirstSpecimenBuilder());
        var (larger, smaller) = Fixture.Create<(int First, int Second)>();
        var denominator = Fixture.Create<int>();
        var ratio1 = new Ratio(larger, denominator);
        var ratio2 = new Ratio(smaller, denominator);

        // When
        var actual = ratio1 > ratio2;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpLessThan_Should_ReturnTrueWhenLess()
    {
        // Given
        Fixture.Customizations.Add(new LargerMagnitudeFirstSpecimenBuilder());
        var (larger, smaller) = Fixture.Create<(int First, int Second)>();
        var denominator = Fixture.Create<int>();
        var ratio1 = new Ratio(smaller, denominator);
        var ratio2 = new Ratio(larger, denominator);

        // When
        var actual = ratio1 < ratio2;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpGreaterThanOrEqual_Should_ReturnTrueForEqual()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio1 = new Ratio(baseNumerator, baseDenominator);
        var ratio2 = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);

        // When
        var actual = ratio1 >= ratio2;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpLessThanOrEqual_Should_ReturnTrueForEqual()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio1 = new Ratio(baseNumerator, baseDenominator);
        var ratio2 = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);

        // When
        var actual = ratio1 <= ratio2;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpEquality_WithInt_Should_ReturnTrueWhenEqual()
    {
        // Given
        var value = Fixture.Create<int>();
        var ratio = new Ratio(value);

        // When
        var actual = ratio == value;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpGreaterThan_WithInt_Should_ReturnTrueWhenGreater()
    {
        // Given
        var value = Fixture.Create<int>();
        var offset = Fixture.Create<int>();
        var ratio = new Ratio(value + offset);

        // When
        var actual = ratio > value;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpLessThan_WithLong_Should_ReturnTrueWhenLess()
    {
        // Given
        var value = Fixture.Create<int>();
        var offset = Fixture.Create<int>();
        var ratio = new Ratio(value - offset);

        // When
        var actual = ratio < value;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpEquality_WithDouble_Should_ReturnTrueWhenEqual()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var value = (double)numerator / denominator;

        // When
        var actual = ratio == value;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpGreaterThan_WithDouble_Should_ReturnTrueWhenGreater()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var value = (double)numerator / denominator - 1.0;

        // When
        var actual = ratio > value;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpEquality_WithDecimal_Should_ReturnTrueWhenEqual()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var decimalValue = (decimal)numerator / denominator;

        // When
        var actual = ratio == decimalValue;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void OpLessThan_WithDecimal_Should_ReturnTrueWhenLess()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var decimalValue = (decimal)numerator / denominator + 1.0m;

        // When
        var actual = ratio < decimalValue;

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void Add_Should_AddTwoRatiosWithSameDenominator()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, expectedDenominator);
        var ratio2 = new Ratio(numerator2, expectedDenominator);
        var expectedNumerator = numerator1 + numerator2;

        // When
        var actual = ratio1 + ratio2;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Add_Should_AddTwoRatiosWithDifferentDenominators()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);
        var expectedNumerator = numerator1 * denominator2 + numerator2 * denominator1;
        var expectedDenominator = denominator1 * denominator2;

        // When
        var actual = ratio1 + ratio2;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Subtract_Should_SubtractTwoRatios()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, expectedDenominator);
        var ratio2 = new Ratio(numerator2, expectedDenominator);
        var expectedNumerator = numerator1 - numerator2;

        // When
        var actual = ratio1 - ratio2;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Subtract_Should_SubtractTwoRatiosWithDifferentDenominators()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);
        var expectedNumerator = numerator1 * denominator2 - numerator2 * denominator1;
        var expectedDenominator = denominator1 * denominator2;

        // When
        var actual = ratio1 - ratio2;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Multiply_Should_MultiplyTwoRatios()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);
        var expectedNumerator = numerator1 * numerator2;
        var expectedDenominator = denominator1 * denominator2;

        // When
        var actual = ratio1 * ratio2;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Multiply_WithInt_Should_MultiplyByInteger()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio = new Ratio(numerator, expectedDenominator);
        var expectedNumerator = numerator * multiplier;

        // When
        var actual = ratio * multiplier;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Multiply_WithLong_Should_MultiplyByLong()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<long>();
        var ratio = new Ratio(numerator, expectedDenominator);
        var expectedNumerator = numerator * multiplier;

        // When
        var actual = ratio * multiplier;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Multiply_WithDouble_Should_ReturnDouble()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var randomValue = Fixture.Create<double>();
        var multiplier = Math.Abs(randomValue % 10) + 1;
        var ratio = new Ratio(numerator, denominator);
        var expected = ((double)numerator / denominator) * multiplier;

        // When
        var actual = ratio * multiplier;

        // Then
        actual.Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void Multiply_WithDecimal_Should_ReturnDecimal()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var randomValue = Fixture.Create<double>();
        var multiplier = (decimal)(Math.Abs(randomValue % 10) + 1);
        var ratio = new Ratio(numerator, denominator);
        var expected = ((decimal)numerator / denominator) * multiplier;

        // When
        var actual = ratio * multiplier;

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Negate_Should_NegateTheRatio()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, expectedDenominator);
        var expectedNumerator = -numerator;

        // When
        var actual = -ratio;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Plus_Should_ReturnSameRatio()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var expected = new Ratio(numerator, denominator);

        // When
        var actual = +expected;

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Increment_Should_AddOne()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var original = new Ratio(numerator, expectedDenominator);
        var expectedNumerator = numerator + expectedDenominator;

        // When
        var actual = original + Ratio.One;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Decrement_Should_SubtractOne()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var original = new Ratio(numerator, expectedDenominator);
        var expectedNumerator = numerator - expectedDenominator;

        // When
        var actual = original - Ratio.One;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Mod_Should_ThrowNotSupportedException()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);

        // When
        var action = () => ratio1 % ratio2;

        // Then
        action.Should().Throw<NotSupportedException>();
    }
}
