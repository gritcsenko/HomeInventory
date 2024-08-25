using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace HomeInventory.Domain.Primitives.Units;

public sealed class Ratio : ISignedNumber<Ratio>, INumber<Ratio>, IConvertible
{
    public Ratio(long numerator, long denominator = 1L)
    {
        ArgumentOutOfRangeException.ThrowIfZero(denominator);
        (Numerator, Denominator) = denominator switch
        {
            < 0 => (-numerator, -denominator),
            _ => (numerator, denominator),
        };
    }

    public static Ratio One { get; } = new(1L);

    public static int Radix { get; } = 2;

    public static Ratio Zero { get; } = new(0L);

    public static Ratio AdditiveIdentity => Zero;

    public static Ratio MultiplicativeIdentity => One;

    public static Ratio NegativeOne { get; } = new(-1L);

    public long Numerator { get; }

    public long Denominator { get; }

    public static Ratio Abs(Ratio value) => new(Math.Abs(value.Numerator), Math.Abs(value.Denominator));

    public static bool IsCanonical(Ratio value)
    {
        throw new NotImplementedException();
    }

    public static bool IsComplexNumber(Ratio value) => false;

    public static bool IsEvenInteger(Ratio value)
    {
        var normal = value.Normalize();
        return normal.Denominator == 1L && long.IsEvenInteger(normal.Numerator);
    }

    public static bool IsFinite(Ratio value) => value.Denominator != 0L;

    public static bool IsImaginaryNumber(Ratio value) => false;

    public static bool IsInfinity(Ratio value) => value.Denominator == 0L;

    public static bool IsInteger(Ratio value) => value.Normalize().Denominator == 1L;

    public static bool IsNaN(Ratio value) => false;

    public static bool IsNegative(Ratio value) => Math.Sign(value.Numerator) != Math.Sign(value.Denominator);

    public static bool IsNegativeInfinity(Ratio value) => value.Denominator == 0L && Math.Sign(value.Numerator) == -1;

    public static bool IsNormal(Ratio value) => value.Gcd() == 1L;

    public static bool IsOddInteger(Ratio value)
    {
        var normal = value.Normalize();
        return normal.Denominator == 1L && long.IsOddInteger(normal.Numerator);
    }

    public static bool IsPositive(Ratio value) => Math.Sign(value.Numerator) == Math.Sign(value.Denominator);

    public static bool IsPositiveInfinity(Ratio value) => value.Denominator == 0L && Math.Sign(value.Numerator) == 1;

    public static bool IsRealNumber(Ratio value) => true;

    public static bool IsSubnormal(Ratio value) => value.Gcd() != 1L;

    public static bool IsZero(Ratio value) => value.Numerator == 0L;

    public static Ratio MaxMagnitude(Ratio x, Ratio y) => CompareMagnitudes(x, y).Bigger;

    public static Ratio MaxMagnitudeNumber(Ratio x, Ratio y) => MaxMagnitude(x, y);

    public static Ratio MinMagnitude(Ratio x, Ratio y) => CompareMagnitudes(x, y).Smaller;

    public static Ratio MinMagnitudeNumber(Ratio x, Ratio y) => MinMagnitude(x, y);

    public static Ratio Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Ratio Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Ratio Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static Ratio Parse(string s, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Ratio result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider, [MaybeNullWhen(false)] out Ratio result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, [MaybeNullWhen(false)] out Ratio result)
    {
        throw new NotImplementedException();
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Ratio result)
    {
        throw new NotImplementedException();
    }

    public static Ratio Plus(Ratio value) => value;

    public static Ratio Add(Ratio left, Ratio right) =>
        left.Denominator == right.Denominator
            ? new(left.Numerator + right.Numerator, left.Denominator)
            : new(left.Numerator * right.Denominator + right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    public static Ratio Negate(Ratio item)
    {
        throw new NotImplementedException();
    }

    public static Ratio Subtract(Ratio left, Ratio right)
    {
        throw new NotImplementedException();
    }

    public static Ratio Increment(Ratio item)
    {
        throw new NotImplementedException();
    }

    public static Ratio Decrement(Ratio item)
    {
        throw new NotImplementedException();
    }

    public static Ratio Multiply(Ratio left, Ratio right)
    {
        throw new NotImplementedException();
    }

    public static Ratio Divide(Ratio left, Ratio right)
    {
        throw new NotImplementedException();
    }

    public override bool Equals(object? obj) => obj is Ratio rational && Equals(rational);

    public bool Equals(Ratio? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        if (Numerator == other.Numerator)
        { 
            return Numerator == 0L || Denominator == other.Denominator;
        }

        var gcd = Gcd();
        if (gcd == 1)
        {
            return false;
        }

        var otherGcd = other.Gcd();
        if (otherGcd == 1)
        {
            return false;
        }

        return DivideBothBy(gcd).Equals(other.DivideBothBy(gcd));
    }

    public override string ToString() => ToString(null);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        Denominator == 1L
            ? Numerator.ToString(format, formatProvider)
            : string.Format(formatProvider, "{0}/{1}", Numerator, Denominator);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(object? obj)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(Ratio? other)
    {
        throw new NotImplementedException();
    }

    public TypeCode GetTypeCode() => TypeCode.Object;

    public bool ToBoolean(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public byte ToByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public char ToChar(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public DateTime ToDateTime(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public decimal ToDecimal(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public double ToDouble(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public short ToInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public int ToInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public long ToInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public sbyte ToSByte(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public float ToSingle(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public object ToType(Type conversionType, IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ushort ToUInt16(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public uint ToUInt32(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public ulong ToUInt64(IFormatProvider? provider)
    {
        throw new NotImplementedException();
    }

    public Ratio Normalize() =>
        Numerator == 0L
            ? new(0L)
            : DivideBothBy(Gcd());

    public Ratio MultiplyBothBy(long value) =>
        value == 0L
            ? new(0L)
            : new(Numerator * value, Denominator * value);

    public Ratio DivideBothBy(long value) =>
        value == 0L
            ? throw new DivideByZeroException()
            : new(Numerator / value, Denominator / value);

    public Ratio OneOver() =>
        Numerator == 0L
            ? throw new DivideByZeroException()
            : new(Denominator, Numerator);

    static bool INumberBase<Ratio>.TryConvertFromChecked<TOther>(TOther value, out Ratio result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<Ratio>.TryConvertFromSaturating<TOther>(TOther value, out Ratio result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<Ratio>.TryConvertFromTruncating<TOther>(TOther value, out Ratio result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<Ratio>.TryConvertToChecked<TOther>(Ratio value, out TOther result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<Ratio>.TryConvertToSaturating<TOther>(Ratio value, out TOther result)
    {
        throw new NotImplementedException();
    }

    static bool INumberBase<Ratio>.TryConvertToTruncating<TOther>(Ratio value, out TOther result)
    {
        throw new NotImplementedException();
    }

    public static Ratio operator +(Ratio value) => Plus(value);

    public static Ratio operator +(Ratio left, Ratio right) => Add(left, right);

    public static Ratio operator -(Ratio value) => new(-value.Numerator, value.Denominator);

    public static Ratio operator -(Ratio left, Ratio right) =>
        left.Denominator == right.Denominator
            ? new(left.Numerator - right.Numerator, left.Denominator)
            : new(left.Numerator * right.Denominator - right.Numerator * left.Denominator, left.Denominator * right.Denominator);

    public static Ratio operator ++(Ratio value) => value + One;

    public static Ratio operator --(Ratio value) => value - One;

    public static Ratio operator *(Ratio left, Ratio right) => new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    public static Ratio operator /(Ratio left, Ratio right) => left * right.OneOver();

    public static bool operator ==(Ratio? left, Ratio? right) => right?.Equals(left) ?? left is null;

    public static bool operator !=(Ratio? left, Ratio? right) => !(right == left);

    public static bool operator >(Ratio left, Ratio right) => CompareMagnitudes(left, right).Bigger.Equals(left);

    public static bool operator >=(Ratio left, Ratio right) => (left == right) || left > right;

    public static bool operator <(Ratio left, Ratio right) => CompareMagnitudes(left, right).Bigger.Equals(right);

    public static bool operator <=(Ratio left, Ratio right) => (left == right) || left < right;

    static Ratio IModulusOperators<Ratio, Ratio, Ratio>.operator %(Ratio left, Ratio right)
    {
        throw new InvalidOperationException();
    }

    public override int GetHashCode()
    {
        HashCode hash = new();
        hash.Add(Denominator);
        hash.Add(Numerator);
        return hash.ToHashCode();
    }

    private static (Ratio Smaller, Ratio Bigger) CompareMagnitudes(Ratio x, Ratio y)
    {
        if (x.Denominator == 0)
        {
            return (y, x);
        }

        if (y.Denominator == 0)
        {
            return (y, x);
        }

        if (x.Denominator == y.Denominator)
        {
            return x.Numerator > y.Numerator ? (y, x) : (x, y);
        }

        if (x.Numerator == y.Numerator)
        {
            return x.Denominator < y.Denominator ? (y, x) : (x, y);
        }

        var gcdX = x.Gcd();
        var gcdY = y.Gcd();

        if (gcdX != 1 || gcdY != 1)
        {
            var normalX = gcdX == 1 ? x : x.DivideBothBy(gcdX);
            var normalY = gcdY == 1 ? y : y.DivideBothBy(gcdY);

            if (normalX.Denominator == normalY.Denominator)
            {
                return normalX.Numerator > normalY.Numerator ? (y, x) : (x, y);
            }

            if (normalX.Numerator == normalY.Numerator)
            {
                return normalX.Denominator < normalY.Denominator ? (y, x) : (x, y);
            }
        }

        var ratioX = (decimal)x.Numerator / x.Denominator;
        var ratioY = (decimal)y.Numerator / y.Denominator;

        return ratioX > ratioY ? (y, x) : (x, y);
    }

    private long Gcd() => MathFunctions.GreatestCommonDivisor(Numerator, Denominator, 0L);
}

public static class MathFunctions
{
    public static T GreatestCommonDivisor<T>(T x, T y, T zero)
        where T : IComparisonOperators<T, T, bool>, IModulusOperators<T, T, T>, IEqualityOperators<T, T, bool>, IUnaryNegationOperators<T, T>
    {
        var a = x.Abs(zero);
        var b = y.Abs(zero);

        while (a != zero && b != zero)
        {
            if (a > b)
                a %= b;
            else
                b %= a;
        }

        return a == zero ? b : a;
    }

    private static T Abs<T>(this T value, T zero)
        where T : IComparisonOperators<T, T, bool>, IUnaryNegationOperators<T, T> =>
        value < zero ? -value : value;
}
