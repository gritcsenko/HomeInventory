using HomeInventory.Domain.Primitives.Units;
using System.Globalization;

namespace HomeInventory.Tests.Domain.ValueObjects;

[UnitTest]
public class RatioTests : BaseTest
{
    public RatioTests() =>
        Fixture.Customizations.Add(new RandomNumericSequenceGenerator(1, 100));

    [Fact]
    public void Constructor_Should_CreateRatioWithDefaultDenominator()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = 1;

        // When
        var actual = new Ratio(expectedNumerator);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Constructor_Should_CreateRatioWithBothParameters()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();

        // When
        var actual = new Ratio(expectedNumerator, expectedDenominator);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Constructor_Should_CreateRatioWithNegativeDenominator()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = -Fixture.Create<int>();
        var expectedNumerator = -numerator;
        var expectedDenominator = -denominator;

        // When
        var actual = new Ratio(numerator, denominator);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Constructor_Should_ThrowIfDenominatorIsZero()
    {
        // Given
        var numerator = Fixture.Create<int>();

        // When
        var action = () => new Ratio(numerator, 0);

        // Then
        action.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Equals_Should_ReturnTrueForSameValues()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator, denominator);
        var ratio2 = new Ratio(numerator, denominator);

        // When
        var actual = ratio1.Equals(ratio2);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnTrueForEquivalentFractions()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio1 = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);
        var ratio2 = new Ratio(baseNumerator, baseDenominator);

        // When
        var actual = ratio1.Equals(ratio2);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalseForDifferentValues()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = numerator1 + 1;
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);

        // When
        var actual = ratio1.Equals(ratio2);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnFalseForNull()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
#pragma warning disable CA1508 // Avoid dead conditional code
        var actual = ratio.Equals((object?)null);
#pragma warning restore CA1508

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equals_Should_ReturnTrueForSameReference()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = ratio.Equals(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnTrueForZeroNumerators()
    {
        // Given
        var denominator1 = Fixture.Create<int>();
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(0, denominator1);
        var ratio2 = new Ratio(0, denominator2);

        // When
        var actual = ratio1.Equals(ratio2);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void CompareTo_Should_ReturnZeroForEqualRatios()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio1 = new Ratio(baseNumerator, baseDenominator);
        var ratio2 = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);
        var expected = 0;

        // When
        var actual = ratio1.CompareTo(ratio2);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void CompareTo_Should_ReturnPositiveWhenGreater()
    {
        // Given
        Fixture.Customizations.Add(new LargerMagnitudeFirstSpecimenBuilder());
        var (larger, smaller) = Fixture.Create<(int First, int Second)>();
        var ratio1 = new Ratio(larger, Fixture.Create<int>());
        var ratio2 = new Ratio(smaller, Fixture.Create<int>());

        // When
        var actual = ratio1.CompareTo(ratio2);

        // Then
        actual.Should().BePositive();
    }

    [Fact]
    public void CompareTo_Should_ReturnNegativeWhenLess()
    {
        // Given
        Fixture.Customizations.Add(new LargerMagnitudeFirstSpecimenBuilder());
        var (larger, smaller) = Fixture.Create<(int First, int Second)>();
        var ratio1 = new Ratio(smaller, Fixture.Create<int>());
        var ratio2 = new Ratio(larger, Fixture.Create<int>());

        // When
        var actual = ratio1.CompareTo(ratio2);

        // Then
        actual.Should().BeNegative();
    }

    [Fact]
    public void CompareTo_Should_ReturnPositiveForNull()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = ratio.CompareTo(null);

        // Then
        actual.Should().BePositive();
    }

    [Fact]
    public void CompareTo_Should_ThrowForInvalidType()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var action = () => ratio.CompareTo("not a ratio");

        // Then
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void One_Should_ReturnRatioOfOne()
    {
        // Given
        var expectedNumerator = 1;
        var expectedDenominator = 1;

        // When
        var actual = Ratio.One;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Zero_Should_ReturnRatioOfZero()
    {
        // Given
        var expectedNumerator = 0;
        var expectedDenominator = 1;

        // When
        var actual = Ratio.Zero;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void NegativeOne_Should_ReturnRatioOfNegativeOne()
    {
        // Given
        var expectedNumerator = -1;
        var expectedDenominator = 1;

        // When
        var actual = Ratio.NegativeOne;

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void AdditiveIdentity_Should_BeZero()
    {
        // Given
        var expected = Ratio.Zero;

        // When
        var actual = Ratio.AdditiveIdentity;

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void MultiplicativeIdentity_Should_BeOne()
    {
        // Given
        var expected = Ratio.One;

        // When
        var actual = Ratio.MultiplicativeIdentity;

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Radix_Should_BeTwo()
    {
        // Given
        var expected = 2;

        // When
        var actual = Ratio.Radix;

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Abs_Should_ReturnAbsoluteValue()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var numerator = -expectedNumerator;
        var expectedDenominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, expectedDenominator);

        // When
        var actual = Ratio.Abs(ratio);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void IsCanonical_Should_ReturnTrueForNormalizedRatio()
    {
        // Given
        Fixture.Customizations.Add(new CoprimeNumbersSpecimenBuilder());
        var (numerator, denominator) = Fixture.Create<(int Numerator, int Denominator)>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsCanonical(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsCanonical_Should_ReturnFalseForNonNormalizedRatio()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);

        // When
        var actual = Ratio.IsCanonical(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsCanonical_Should_ReturnTrueForZero()
    {
        // Given
        var ratio = new Ratio(0);

        // When
        var actual = Ratio.IsCanonical(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsComplexNumber_Should_AlwaysReturnFalse()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsComplexNumber(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsEvenInteger_Should_ReturnTrueForEvenInteger()
    {
        // Given
        var evenValue = Fixture.Create<int>() * 2;
        var ratio = new Ratio(evenValue);

        // When
        var actual = Ratio.IsEvenInteger(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsEvenInteger_Should_ReturnFalseForOddInteger()
    {
        // Given
        var oddNumerator = Fixture.Create<int>() * 2 + 1;
        var ratio = new Ratio(oddNumerator);

        // When
        var actual = Ratio.IsEvenInteger(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsEvenInteger_Should_ReturnFalseForNonInteger()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>() + 1;
        var ratio = new Ratio(numerator, numerator + denominator);

        // When
        var actual = Ratio.IsEvenInteger(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsFinite_Should_ReturnTrueForNormalRatio()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsFinite(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsImaginaryNumber_Should_AlwaysReturnFalse()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsImaginaryNumber(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsInteger_Should_ReturnTrueForInteger()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator * denominator, denominator);

        // When
        var actual = Ratio.IsInteger(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsInteger_Should_ReturnFalseForNonInteger()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>() + 1;
        var ratio = new Ratio(numerator, numerator + denominator);

        // When
        var actual = Ratio.IsInteger(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsNaN_Should_AlwaysReturnFalse()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsNaN(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsNegative_Should_ReturnTrueForNegative()
    {
        // Given
        var numerator = -Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsNegative(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsNegative_Should_ReturnFalseForPositive()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsNegative(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsNormal_Should_ReturnTrueWhenGcdIsOne()
    {
        // Given
        Fixture.Customizations.Add(new CoprimeNumbersSpecimenBuilder());
        var (numerator, denominator) = Fixture.Create<(int Numerator, int Denominator)>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsNormal(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsOddInteger_Should_ReturnTrueForOddInteger()
    {
        // Given
        var oddNumerator = Fixture.Create<int>() * 2 + 1;
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(oddNumerator * denominator, denominator);

        // When
        var actual = Ratio.IsOddInteger(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsOddInteger_Should_ReturnFalseForEvenInteger()
    {
        // Given
        var evenNumerator = Fixture.Create<int>() * 2;
        var ratio = new Ratio(evenNumerator);

        // When
        var actual = Ratio.IsOddInteger(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsPositive_Should_ReturnTrueForPositive()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsPositive(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsPositive_Should_ReturnFalseForNegative()
    {
        // Given
        var numerator = -Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsPositive(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void IsRealNumber_Should_AlwaysReturnTrue()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = Ratio.IsRealNumber(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsSubnormal_Should_ReturnTrueWhenNotNormalized()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);

        // When
        var actual = Ratio.IsSubnormal(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsZero_Should_ReturnTrueForZero()
    {
        // Given
        var ratio = new Ratio(0);

        // When
        var actual = Ratio.IsZero(ratio);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsZero_Should_ReturnFalseForNonZero()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var ratio = new Ratio(numerator);

        // When
        var actual = Ratio.IsZero(ratio);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void MaxMagnitude_Should_ReturnLargerMagnitude()
    {
        // Given
        Fixture.Customizations.Add(new LargerMagnitudeFirstSpecimenBuilder());
        var (larger, smaller) = Fixture.Create<(int First, int Second)>();
        var expected = new Ratio(larger);
        var decoy = new Ratio(smaller);

        // When
        var actual = Ratio.MaxMagnitude(expected, decoy);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void MinMagnitude_Should_ReturnSmallerMagnitude()
    {
        // Given
        Fixture.Customizations.Add(new LargerMagnitudeFirstSpecimenBuilder());
        var (larger, smaller) = Fixture.Create<(int First, int Second)>();
        var decoy = new Ratio(larger);
        var expected = new Ratio(smaller);

        // When
        var actual = Ratio.MinMagnitude(decoy, expected);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Parse_Should_ParseSimpleInteger()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = 1;

        // When
        var actual = Ratio.Parse(expectedNumerator.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Parse_Should_ParseFraction()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var text = $"{expectedNumerator}/{expectedDenominator}";

        // When
        var actual = Ratio.Parse(text, CultureInfo.InvariantCulture);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Parse_Should_ParseNegativeFraction()
    {
        // Given
        var expectedNumerator = -Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var text = $"{expectedNumerator}/{expectedDenominator}";

        // When
        var actual = Ratio.Parse(text, CultureInfo.InvariantCulture);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Parse_Should_ThrowForInvalidFormat()
    {
        // Given
        var text = "invalid";

        // When
        var action = () => Ratio.Parse(text, CultureInfo.InvariantCulture);

        // Then
        action.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_Should_ThrowForNullString()
    {
        // Given
        string text = null!;

        // When
        var action = () => Ratio.Parse(text, CultureInfo.InvariantCulture);

        // Then
        action.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void TryParse_Should_ReturnTrueForValidInput()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();

        // When
        var actualSuccess = Ratio.TryParse($"{expectedNumerator}/{expectedDenominator}", CultureInfo.InvariantCulture, out var actual);

        // Then
        using var _ = new AssertionScope();
        actualSuccess.Should().BeTrue();
#pragma warning disable CS8602 // Dereference of a possibly null reference
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
#pragma warning restore CS8602
    }

    [Fact]
    public void TryParse_Should_ReturnFalseForInvalidInput()
    {
        // Given
        var text = "invalid";

        // When
        var actualSuccess = Ratio.TryParse(text, CultureInfo.InvariantCulture, out var actual);

        // Then
        using var _ = new AssertionScope();
        actualSuccess.Should().BeFalse();
        actual.Should().BeNull();
    }

    [Fact]
    public void TryParse_Should_ReturnFalseForEmptyString()
    {
        // Given
        var text = "";

        // When
        var actual = Ratio.TryParse(text, CultureInfo.InvariantCulture, out _);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void TryParse_Should_ReturnFalseForWhitespace()
    {
        // Given
        var text = "   ";

        // When
        var actual = Ratio.TryParse(text, CultureInfo.InvariantCulture, out _);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void TryParse_Should_ReturnFalseForZeroDenominator()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var text = $"{numerator}/0";

        // When
        var actual = Ratio.TryParse(text, CultureInfo.InvariantCulture, out _);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void TryParse_Should_HandleNullString()
    {
        // Given
        string? text = null;

        // When
        var actual = Ratio.TryParse(text, CultureInfo.InvariantCulture, out _);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void TryParse_Span_Should_ParseValidInput()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var span = $"{expectedNumerator}/{expectedDenominator}".AsSpan();

        // When
        var actualSuccess = Ratio.TryParse(span, CultureInfo.InvariantCulture, out var actual);

        // Then
        using var _ = new AssertionScope();
        actualSuccess.Should().BeTrue();
#pragma warning disable CS8602 // Dereference of a possibly null reference
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
#pragma warning restore CS8602
    }

    [Fact]
    public void ToString_Should_FormatAsInteger()
    {
        // Given
        var value = Fixture.Create<int>();
        var ratio = new Ratio(value);
        var expected = value.ToString(CultureInfo.InvariantCulture);

        // When
        var actual = ratio.ToString(CultureInfo.InvariantCulture);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToString_Should_FormatAsFraction()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = $"{numerator}/{denominator}";

        // When
        var actual = ratio.ToString(CultureInfo.InvariantCulture);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToString_Should_FormatNegativeFraction()
    {
        // Given
        var numerator = -Fixture.Create<int>();
        var denominator = Fixture.Create<int>() + 1;
        var ratio = new Ratio(numerator, denominator);
        var expected = $"{numerator}/{denominator}";

        // When
        var actual = ratio.ToString(CultureInfo.InvariantCulture);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void TryFormat_Should_FormatToSpan()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        Span<char> destination = stackalloc char[10];
        var expected = $"{numerator}/{denominator}";
        var expectedCharsWritten = expected.Length;

        // When
        var actualSuccess = ratio.TryFormat(destination, out var actualCharsWritten, [], CultureInfo.InvariantCulture);

        // Then
        using var _ = new AssertionScope();
        actualSuccess.Should().BeTrue();
        actualCharsWritten.Should().Be(expectedCharsWritten);
        destination[..actualCharsWritten].ToString().Should().Be(expected);
    }

    [Fact]
    public void TryFormat_Should_ReturnFalseWhenDestinationTooSmall()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        Span<char> destination = stackalloc char[2];
        var expectedCharsWritten = 0;

        // When
        var actualSuccess = ratio.TryFormat(destination, out var actualCharsWritten, [], CultureInfo.InvariantCulture);

        // Then
        using var _ = new AssertionScope();
        actualSuccess.Should().BeFalse();
        actualCharsWritten.Should().Be(expectedCharsWritten);
    }

    [Fact]
    public void GetTypeCode_Should_ReturnObject()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = TypeCode.Object;

        // When
        var actual = ratio.GetTypeCode();

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToBoolean_Should_ReturnTrueForNonZero()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var actual = ratio.ToBoolean(null);

        // Then
        actual.Should().BeTrue();
    }

    [Fact]
    public void ToBoolean_Should_ReturnFalseForZero()
    {
        // Given
        var ratio = new Ratio(0);

        // When
        var actual = ratio.ToBoolean(null);

        // Then
        actual.Should().BeFalse();
    }

    [Fact]
    public void ToByte_Should_ConvertToInteger()
    {
        // Given
        var numerator = Fixture.Create<byte>();
        var denominator = Fixture.Create<byte>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (byte)(numerator / denominator);

        // When
        var actual = ratio.ToByte(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToChar_Should_ThrowInvalidCastException()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var action = () => ratio.ToChar(null);

        // Then
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void ToDateTime_Should_ThrowInvalidCastException()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var action = () => ratio.ToDateTime(null);

        // Then
        action.Should().Throw<InvalidCastException>();
    }

    [Fact]
    public void ToDecimal_Should_ConvertToDecimal()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (decimal)numerator / denominator;

        // When
        var actual = ratio.ToDecimal(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToDouble_Should_ConvertToDouble()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (double)numerator / denominator;

        // When
        var actual = ratio.ToDouble(null);

        // Then
        actual.Should().BeApproximately(expected, 0.0001);
    }

    [Fact]
    public void ToInt16_Should_ConvertToShort()
    {
        // Given
        var numerator = Fixture.Create<short>();
        var denominator = Fixture.Create<short>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (short)(numerator / denominator);

        // When
        var actual = ratio.ToInt16(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToInt32_Should_ConvertToInt()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = numerator / denominator;

        // When
        var actual = ratio.ToInt32(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToInt64_Should_ConvertToLong()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = numerator / denominator;

        // When
        var actual = ratio.ToInt64(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToSByte_Should_ConvertToSByte()
    {
        // Given
        var numerator = Math.Abs(Fixture.Create<sbyte>());
        var denominator = Math.Abs(Fixture.Create<sbyte>());
        var ratio = new Ratio(numerator, denominator);
        var expected = (sbyte)(numerator / denominator);

        // When
        var actual = ratio.ToSByte(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToSingle_Should_ConvertToFloat()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (float)numerator / denominator;

        // When
        var actual = ratio.ToSingle(null);

        // Then
        actual.Should().BeApproximately(expected, 0.0001f);
    }

    [Fact]
    public void ToUInt16_Should_ConvertToUShort()
    {
        // Given
        var numerator = Fixture.Create<ushort>();
        var denominator = Fixture.Create<ushort>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (ushort)(numerator / denominator);

        // When
        var actual = ratio.ToUInt16(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToUInt32_Should_ConvertToUInt()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (uint)(numerator / denominator);

        // When
        var actual = ratio.ToUInt32(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void ToUInt64_Should_ConvertToULong()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expected = (ulong)(numerator / denominator);

        // When
        var actual = ratio.ToUInt64(null);

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Normalize_Should_ReduceFractionToLowestTerms()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);
        var expectedGreatestCommonDivisor = MathFunctions.GreatestCommonDivisor(baseNumerator, baseDenominator, 0);
        var expectedNumerator = baseNumerator / expectedGreatestCommonDivisor;
        var expectedDenominator = baseDenominator / expectedGreatestCommonDivisor;

        // When
        var actual = ratio.Normalize();

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void Normalize_Should_ReturnZeroOverOneForZero()
    {
        // Given
        var denominator = Fixture.Create<int>();
        var expectedNumerator = 0;
        var ratio = new Ratio(expectedNumerator, denominator);
        var expectedDenominator = 1;

        // When
        var actual = ratio.Normalize();

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void MultiplyBothBy_Should_MultiplyNumeratorAndDenominator()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expectedNumerator = numerator * multiplier;
        var expectedDenominator = denominator * multiplier;

        // When
        var actual = ratio.MultiplyBothBy(multiplier);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void MultiplyBothBy_Should_ReturnZeroWhenMultipliedByZero()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);
        var expectedNumerator = 0;
        var expectedDenominator = 1;

        // When
        var actual = ratio.MultiplyBothBy(0);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void DivideBothBy_Should_DivideNumeratorAndDenominator()
    {
        // Given
        var expectedNumerator = Fixture.Create<int>();
        var expectedDenominator = Fixture.Create<int>();
        var divisor = Fixture.Create<int>();
        var ratio = new Ratio(expectedNumerator * divisor, expectedDenominator * divisor);

        // When
        var actual = ratio.DivideBothBy(divisor);

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void DivideBothBy_Should_ThrowWhenDividingByZero()
    {
        // Given
        var numerator = Fixture.Create<int>();
        var denominator = Fixture.Create<int>();
        var ratio = new Ratio(numerator, denominator);

        // When
        var action = () => ratio.DivideBothBy(0);

        // Then
        action.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void OneOver_Should_ReturnReciprocal()
    {
        // Given
        var expectedDenominator = Fixture.Create<int>();
        var expectedNumerator = Fixture.Create<int>();
        var ratio = new Ratio(expectedDenominator, expectedNumerator);

        // When
        var actual = ratio.OneOver();

        // Then
        using var _ = new AssertionScope();
        actual.Numerator.Should().Be(expectedNumerator);
        actual.Denominator.Should().Be(expectedDenominator);
    }

    [Fact]
    public void OneOver_Should_ThrowForZeroNumerator()
    {
        // Given
        var ratio = new Ratio(0);

        // When
        var action = () => ratio.OneOver();

        // Then
        action.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void GetHashCode_Should_BeSameForEquivalentFractions()
    {
        // Given
        var baseNumerator = Fixture.Create<int>();
        var baseDenominator = Fixture.Create<int>();
        var multiplier = Fixture.Create<int>();
        var ratio1 = new Ratio(baseNumerator * multiplier, baseDenominator * multiplier);
        var ratio2 = new Ratio(baseNumerator, baseDenominator);
        var expected = ratio2.GetHashCode();

        // When
        var actual = ratio1.GetHashCode();

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void GetHashCode_Should_BeDifferentForDifferentValues()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var denominator1 = Fixture.Create<int>();
        var numerator2 = numerator1 + 1;
        var denominator2 = Fixture.Create<int>();
        var ratio1 = new Ratio(numerator1, denominator1);
        var ratio2 = new Ratio(numerator2, denominator2);
        var notExpected = ratio2.GetHashCode();

        // When
        var actual = ratio1.GetHashCode();

        // Then
        actual.Should().NotBe(notExpected);
    }

    [Fact]
    public void Gcd_Should_CalculateGreatestCommonDivisor()
    {
        // Given
        var numerator1 = Fixture.Create<int>();
        var numerator2 = Fixture.Create<int>();
        var greatestCommonDivisor = Fixture.Create<int>();
        var ratio = new Ratio(numerator1 * greatestCommonDivisor, numerator2 * greatestCommonDivisor);
        var expected = MathFunctions.GreatestCommonDivisor(numerator1 * greatestCommonDivisor, numerator2 * greatestCommonDivisor, 0L);

        // When
        var actual = ratio.Gcd;

        // Then
        actual.Should().Be(expected);
    }

    [Fact]
    public void Gcd_Should_BeOneForCoprimeNumbers()
    {
        // Given
        Fixture.Customizations.Add(new CoprimeNumbersSpecimenBuilder());
        var (numerator, denominator) = Fixture.Create<(int Numerator, int Denominator)>();
        var ratio = new Ratio(numerator, denominator);
        var expected = 1;

        // When
        var actual = ratio.Gcd;

        // Then
        actual.Should().Be(expected);
    }
}
