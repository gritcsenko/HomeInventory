using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;

namespace HomeInventory.Domain.Primitives.Units;

public sealed class Ratio : ISignedNumber<Ratio>, INumber<Ratio>, IConvertible,
    IMultiplyOperators<Ratio, long, Ratio>, IDivisionOperators<Ratio, long, Ratio>,
    IComparisonOperators<Ratio, long, bool>,
    IMultiplyOperators<Ratio, int, Ratio>, IDivisionOperators<Ratio, int, Ratio>,
    IComparisonOperators<Ratio, int, bool>,
    IMultiplyOperators<Ratio, double, double>, IDivisionOperators<Ratio, double, double>,
    IComparisonOperators<Ratio, double, bool>,
    IMultiplyOperators<Ratio, decimal, decimal>, IDivisionOperators<Ratio, decimal, decimal>,
    IComparisonOperators<Ratio, decimal, bool>
{
    private readonly Lazy<long> _gcd;

    public Ratio(long numerator, long denominator = 1L)
    {
        ArgumentOutOfRangeException.ThrowIfZero(denominator);
        (Numerator, Denominator) = denominator switch
        {
            < 0 => (-numerator, -denominator),
            _ => (numerator, denominator),
        };
        _gcd = new(() => MathFunctions.GreatestCommonDivisor(Numerator, Denominator, 0L));
    }

    public long Numerator { get; }

    public long Denominator { get; }

    public long Gcd => _gcd.Value;

    public static bool operator ==(Ratio? left, decimal right) => left?.ToDecimal(null) == right;

    public static bool operator !=(Ratio? left, decimal right) => !(left == right);

    public static bool operator >(Ratio left, decimal right) => left.ToDecimal(null) > right;

    public static bool operator >=(Ratio left, decimal right) => left.ToDecimal(null) >= right;

    public static bool operator <=(Ratio left, decimal right) => left.ToDecimal(null) <= right;

    public static bool operator <(Ratio left, decimal right) => left.ToDecimal(null) < right;

    public static bool operator ==(Ratio? left, double right) =>
        Math.Abs(left?.ToDouble(null) ?? double.NaN - right) < double.Epsilon;

    public static bool operator !=(Ratio? left, double right) => !(left == right);

    public static bool operator >(Ratio left, double right) => left.ToDouble(null) > right;

    public static bool operator >=(Ratio left, double right) => left.ToDouble(null) >= right;

    public static bool operator <=(Ratio left, double right) => left.ToDouble(null) <= right;

    public static bool operator <(Ratio left, double right) => left.ToDouble(null) < right;

    public static bool operator ==(Ratio? left, int right) => left?.Equals(new(right)) ?? false;

    public static bool operator !=(Ratio? left, int right) => !(left == right);

    public static bool operator >(Ratio left, int right) => left.CompareTo(new(right)) > 0;

    public static bool operator >=(Ratio left, int right) => left.CompareTo(new(right)) >= 0;

    public static bool operator <=(Ratio left, int right) => left.CompareTo(new(right)) <= 0;

    public static bool operator <(Ratio left, int right) => left.CompareTo(new(right)) < 0;

    public static bool operator ==(Ratio? left, long right) => left?.Equals(new(right)) ?? false;

    public static bool operator !=(Ratio? left, long right) => !(left == right);

    public static bool operator >(Ratio left, long right) => left.CompareTo(new(right)) > 0;

    public static bool operator >=(Ratio left, long right) => left.CompareTo(new(right)) >= 0;

    public static bool operator <(Ratio left, long right) => left.CompareTo(new(right)) < 0;

    public static bool operator <=(Ratio left, long right) => left.CompareTo(new(right)) <= 0;

    public TypeCode GetTypeCode() => TypeCode.Object;

    public bool ToBoolean(IFormatProvider? provider) => Numerator != 0L;

    public byte ToByte(IFormatProvider? provider) => checked((byte)(Numerator / Denominator));

    public char ToChar(IFormatProvider? provider) => throw new InvalidCastException("Cannot convert Ratio to Char");

    public DateTime ToDateTime(IFormatProvider? provider) =>
        throw new InvalidCastException("Cannot convert Ratio to DateTime");

    public decimal ToDecimal(IFormatProvider? provider) => (decimal)Numerator / Denominator;

    public double ToDouble(IFormatProvider? provider) => (double)Numerator / Denominator;

    public short ToInt16(IFormatProvider? provider) => checked((short)(Numerator / Denominator));

    public int ToInt32(IFormatProvider? provider) => checked((int)(Numerator / Denominator));

    public long ToInt64(IFormatProvider? provider) => Numerator / Denominator;

    public sbyte ToSByte(IFormatProvider? provider) => checked((sbyte)(Numerator / Denominator));

    public float ToSingle(IFormatProvider? provider) => (float)Numerator / Denominator;

    public object ToType(Type conversionType, IFormatProvider? provider) =>
        Convert.ChangeType(conversionType.IsAssignableFrom(typeof(Ratio)) ? this : ToDouble(provider), conversionType,
            provider);

    public ushort ToUInt16(IFormatProvider? provider) => checked((ushort)(Numerator / Denominator));

    public uint ToUInt32(IFormatProvider? provider) => checked((uint)(Numerator / Denominator));

    public ulong ToUInt64(IFormatProvider? provider) => checked((ulong)(Numerator / Denominator));

    public static decimal operator /(Ratio left, decimal right) => Divide(left, right);

    public static double operator /(Ratio left, double right) => Divide(left, right);

    public static Ratio operator /(Ratio left, int right) => Divide(left, right);

    public static Ratio operator /(Ratio left, long right) => Divide(left, right);

    public static decimal operator *(Ratio left, decimal right) => Multiply(left, right);

    public static double operator *(Ratio left, double right) => Multiply(left, right);

    public static Ratio operator *(Ratio left, int right) => Multiply(left, right);

    public static Ratio operator *(Ratio left, long right) => Multiply(left, right);

    public static Ratio operator %(Ratio left, Ratio right) => Mod(left, right);

    public int CompareTo(object? obj) =>
        obj switch
        {
            null => 1,
            Ratio other => CompareTo(other),
            _ => throw new ArgumentException("Object must be of type Ratio", nameof(obj)),
        };

    public int CompareTo(Ratio? other)
    {
        if (other is null)
        {
            return 1;
        }

        // Cross-multiply to avoid division: a/b compared to c/d => a*d compared to c*b
        var left = Numerator * other.Denominator;
        var right = other.Numerator * Denominator;

        return left.CompareTo(right);
    }

    public static bool operator >(Ratio left, Ratio right) => left.CompareTo(right) > 0;

    public static bool operator >=(Ratio left, Ratio right) => left.CompareTo(right) >= 0;

    public static bool operator <(Ratio left, Ratio right) => left.CompareTo(right) < 0;

    public static bool operator <=(Ratio left, Ratio right) => left.CompareTo(right) <= 0;

    public static Ratio One { get; } = new(1L);

    public static int Radix { get; } = 2;

    public static Ratio Zero { get; } = new(0L);

    public static Ratio AdditiveIdentity => Zero;

    public static Ratio MultiplicativeIdentity => One;

    public static Ratio NegativeOne { get; } = new(-1L);

    public static Ratio Abs(Ratio value) => new(Math.Abs(value.Numerator), Math.Abs(value.Denominator));

    public static bool IsCanonical(Ratio value) =>
        value.Numerator == 0L
            ? value.Denominator == 1L
            : value.Gcd == 1L;

    public static bool IsComplexNumber(Ratio value) => false;

    public static bool IsEvenInteger(Ratio value)
    {
        var normal = value.Normalize();
        return normal.Denominator == 1L && long.IsEvenInteger(normal.Numerator);
    }

    public static bool IsFinite(Ratio value) => value.Denominator != 0L;

    public static bool IsImaginaryNumber(Ratio value) => false;

    public static bool IsInfinity(Ratio value) => value.Denominator == 0L;

    public static bool IsInteger(Ratio value) => value.Numerator % value.Denominator == 0L;

    public static bool IsNaN(Ratio value) => false;

    public static bool IsNegative(Ratio value) => value.Numerator < 0L;

    public static bool IsNegativeInfinity(Ratio value) => value.Denominator == 0L && Math.Sign(value.Numerator) == -1;

    public static bool IsNormal(Ratio value) => value.Gcd == 1L;

    public static bool IsOddInteger(Ratio value)
    {
        var normal = value.Normalize();
        return normal.Denominator == 1L && long.IsOddInteger(normal.Numerator);
    }

    public static bool IsPositive(Ratio value) => Math.Sign(value.Numerator) == Math.Sign(value.Denominator);

    public static bool IsPositiveInfinity(Ratio value) => value.Denominator == 0L && Math.Sign(value.Numerator) == 1;

    public static bool IsRealNumber(Ratio value) => true;

    public static bool IsSubnormal(Ratio value) => value.Gcd != 1L;

    public static bool IsZero(Ratio value) => value.Numerator == 0L;

    public static Ratio MaxMagnitude(Ratio x, Ratio y) => CompareMagnitudes(x, y).Bigger;

    public static Ratio MaxMagnitudeNumber(Ratio x, Ratio y) => MaxMagnitude(x, y);

    public static Ratio MinMagnitude(Ratio x, Ratio y) => CompareMagnitudes(x, y).Smaller;

    public static Ratio MinMagnitudeNumber(Ratio x, Ratio y) => MinMagnitude(x, y);

    public static Ratio Parse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider)
        => TryParse(s, style, provider, out var result)
            ? result
            : throw new FormatException($"Unable to parse '{s.ToString()}' as a Ratio.");

    public static Ratio Parse(string s, NumberStyles style, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), style, provider);
    }

    public static Ratio Parse(ReadOnlySpan<char> s, IFormatProvider? provider) =>
        Parse(s, NumberStyles.Integer, provider);

    public static Ratio Parse(string s, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(s);
        return Parse(s.AsSpan(), provider);
    }

    public static bool TryParse(ReadOnlySpan<char> s, NumberStyles style, IFormatProvider? provider,
        [MaybeNullWhen(false)] out Ratio result)
    {
        result = null;

        if (s.IsEmpty || s.IsWhiteSpace())
        {
            return false;
        }

        s = s.Trim();

        var slashIndex = s.IndexOf('/');
        if (slashIndex == -1)
        {
            if (!long.TryParse(s, style, provider, out var numerator))
            {
                return false;
            }

            result = new(numerator);
            return true;
        }

        var numeratorSpan = s[..slashIndex];
        var denominatorSpan = s[(slashIndex + 1)..];

        if (!long.TryParse(numeratorSpan, style, provider, out var num) ||
            !long.TryParse(denominatorSpan, style, provider, out var den))
        {
            return false;
        }

        try
        {
            result = new(num, den);
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            return false;
        }
    }

    public static bool TryParse([NotNullWhen(true)] string? s, NumberStyles style, IFormatProvider? provider,
        [MaybeNullWhen(false)] out Ratio result)
    {
        if (s is not null)
        {
            return TryParse(s.AsSpan(), style, provider, out result);
        }

        result = null;
        return false;
    }

    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider,
        [MaybeNullWhen(false)] out Ratio result) => TryParse(s, NumberStyles.Integer, provider, out result);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
        [MaybeNullWhen(false)] out Ratio result)
    {
        if (s is not null)
        {
            return TryParse(s.AsSpan(), provider, out result);
        }

        result = null;
        return false;
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

        return Gcd != 1 && other.Gcd != 1 && DivideBothBy(Gcd).Equals(other.DivideBothBy(Gcd));
    }

    public override string ToString() => ToString(null);

    public string ToString(IFormatProvider? provider) => ToString(null, provider);

    public string ToString(string? format, IFormatProvider? formatProvider) =>
        Denominator == 1L
            ? Numerator.ToString(format, formatProvider)
            : string.Format(formatProvider, "{0}/{1}", Numerator, Denominator);

    public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format,
        IFormatProvider? provider)
    {
        var str = ToString(format.ToString(), provider);
        if (str.Length > destination.Length)
        {
            charsWritten = 0;
            return false;
        }

        str.AsSpan().CopyTo(destination);
        charsWritten = str.Length;
        return true;
    }

    static bool INumberBase<Ratio>.TryConvertFromChecked<TOther>(TOther value, out Ratio result)
    {
        switch (typeof(TOther))
        {
            case var t when t == typeof(byte):
                result = new((byte)(object)value);
                return true;
            case var t when t == typeof(sbyte):
                result = new((sbyte)(object)value);
                return true;
            case var t when t == typeof(short):
                result = new((short)(object)value);
                return true;
            case var t when t == typeof(ushort):
                result = new((ushort)(object)value);
                return true;
            case var t when t == typeof(int):
                result = new((int)(object)value);
                return true;
            case var t when t == typeof(uint):
                result = new((uint)(object)value);
                return true;
            case var t when t == typeof(long):
                result = new((long)(object)value);
                return true;
            case var t when t == typeof(ulong):
                result = new(checked((long)(ulong)(object)value));
                return true;
            case var t when t == typeof(nint):
                result = new((nint)(object)value);
                return true;
            case var t when t == typeof(nuint):
                result = new(checked((long)(nuint)(object)value));
                return true;
            case var t when t == typeof(Ratio):
                result = (Ratio)(object)value;
                return true;
            default:
                result = null!;
                return false;
        }
    }

    static bool INumberBase<Ratio>.TryConvertFromSaturating<TOther>(TOther value, out Ratio result)
    {
        switch (typeof(TOther))
        {
            case var t when t == typeof(ulong):
                var ulongValue = (ulong)(object)value;
                result = new(ulongValue > long.MaxValue ? long.MaxValue : (long)ulongValue);
                return true;
            case var t when t == typeof(nuint):
                var nuintValue = (nuint)(object)value;
                result = new(nuintValue > (nuint)long.MaxValue ? long.MaxValue : (long)nuintValue);
                return true;
            case var t when t == typeof(byte):
                result = new((byte)(object)value);
                return true;
            case var t when t == typeof(sbyte):
                result = new((sbyte)(object)value);
                return true;
            case var t when t == typeof(short):
                result = new((short)(object)value);
                return true;
            case var t when t == typeof(ushort):
                result = new((ushort)(object)value);
                return true;
            case var t when t == typeof(int):
                result = new((int)(object)value);
                return true;
            case var t when t == typeof(uint):
                result = new((uint)(object)value);
                return true;
            case var t when t == typeof(long):
                result = new((long)(object)value);
                return true;
            case var t when t == typeof(nint):
                result = new((nint)(object)value);
                return true;
            case var t when t == typeof(Ratio):
                result = (Ratio)(object)value;
                return true;
            default:
                result = null!;
                return false;
        }
    }

    static bool INumberBase<Ratio>.TryConvertFromTruncating<TOther>(TOther value, out Ratio result)
    {
        switch (typeof(TOther))
        {
            case var t when t == typeof(ulong):
                result = new(unchecked((long)(ulong)(object)value));
                return true;
            case var t when t == typeof(nuint):
                result = new(unchecked((long)(nuint)(object)value));
                return true;
            case var t when t == typeof(double):
                var dblValue = (double)(object)value;
                if (double.IsNaN(dblValue) || double.IsInfinity(dblValue))
                {
                    result = null!;
                    return false;
                }

                result = new(unchecked((long)dblValue));
                return true;
            case var t when t == typeof(float):
                var fltValue = (float)(object)value;
                if (float.IsNaN(fltValue) || float.IsInfinity(fltValue))
                {
                    result = null!;
                    return false;
                }

                result = new(unchecked((long)fltValue));
                return true;
            case var t when t == typeof(decimal):
                result = new((long)(decimal)(object)value);
                return true;
            case var t when t == typeof(byte):
                result = new((byte)(object)value);
                return true;
            case var t when t == typeof(sbyte):
                result = new((sbyte)(object)value);
                return true;
            case var t when t == typeof(short):
                result = new((short)(object)value);
                return true;
            case var t when t == typeof(ushort):
                result = new((ushort)(object)value);
                return true;
            case var t when t == typeof(int):
                result = new((int)(object)value);
                return true;
            case var t when t == typeof(uint):
                result = new((uint)(object)value);
                return true;
            case var t when t == typeof(long):
                result = new((long)(object)value);
                return true;
            case var t when t == typeof(nint):
                result = new((nint)(object)value);
                return true;
            case var t when t == typeof(Ratio):
                result = (Ratio)(object)value;
                return true;
            default:
                result = null!;
                return false;
        }
    }

    static bool INumberBase<Ratio>.TryConvertToChecked<TOther>(Ratio value, out TOther result)
    {
        switch (typeof(TOther))
        {
            case var t when t == typeof(byte):
                result = (TOther)(object)checked((byte)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(sbyte):
                result = (TOther)(object)checked((sbyte)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(short):
                result = (TOther)(object)checked((short)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(ushort):
                result = (TOther)(object)checked((ushort)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(int):
                result = (TOther)(object)checked((int)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(uint):
                result = (TOther)(object)checked((uint)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(long):
                result = (TOther)(object)(value.Numerator / value.Denominator);
                return true;
            case var t when t == typeof(ulong):
                result = (TOther)(object)checked((ulong)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(nint):
                result = (TOther)(object)checked((nint)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(nuint):
                result = (TOther)(object)checked((nuint)(value.Numerator / value.Denominator));
                return true;
            case var t when t == typeof(float):
                result = (TOther)(object)(float)value.ToDouble(null);
                return true;
            case var t when t == typeof(double):
                result = (TOther)(object)value.ToDouble(null);
                return true;
            case var t when t == typeof(decimal):
                result = (TOther)(object)value.ToDecimal(null);
                return true;
            case var t when t == typeof(Ratio):
                result = (TOther)(object)value;
                return true;
            default:
                result = default!;
                return false;
        }
    }

    static bool INumberBase<Ratio>.TryConvertToSaturating<TOther>(Ratio value, out TOther result)
    {
        var quotient = value.Numerator / value.Denominator;

        switch (typeof(TOther))
        {
            case var t when t == typeof(byte):
                var byteClamped = quotient switch
                {
                    < 0 => (byte)0,
                    > byte.MaxValue => byte.MaxValue,
                    _ => (byte)quotient,
                };

                result = (TOther)(object)byteClamped;
                return true;
            case var t when t == typeof(sbyte):
                var sbyteClamped = quotient switch
                {
                    < sbyte.MinValue => sbyte.MinValue,
                    > sbyte.MaxValue => sbyte.MaxValue,
                    _ => (sbyte)quotient,
                };

                result = (TOther)(object)sbyteClamped;
                return true;
            case var t when t == typeof(short):
                var shortClamped = quotient switch
                {
                    < short.MinValue => short.MinValue,
                    > short.MaxValue => short.MaxValue,
                    _ => (short)quotient,
                };

                result = (TOther)(object)shortClamped;
                return true;
            case var t when t == typeof(ushort):
                var ushortClamped = quotient switch
                {
                    < 0 => (ushort)0,
                    > ushort.MaxValue => ushort.MaxValue,
                    _ => (ushort)quotient,
                };

                result = (TOther)(object)ushortClamped;
                return true;
            case var t when t == typeof(int):
                var intClamped = quotient switch
                {
                    < int.MinValue => int.MinValue,
                    > int.MaxValue => int.MaxValue,
                    _ => (int)quotient,
                };

                result = (TOther)(object)intClamped;
                return true;
            case var t when t == typeof(uint):
                var uintClamped = quotient switch
                {
                    < 0 => 0U,
                    > uint.MaxValue => uint.MaxValue,
                    _ => (uint)quotient,
                };

                result = (TOther)(object)uintClamped;
                return true;
            case var t when t == typeof(long):
                result = (TOther)(object)quotient;
                return true;
            case var t when t == typeof(ulong):
                result = (TOther)(object)(ulong)(quotient < 0 ? 0 : quotient);
                return true;
            case var t when t == typeof(nint):
                result = (TOther)(object)(nint)quotient;
                return true;
            case var t when t == typeof(nuint):
                result = (TOther)(object)(nuint)(quotient < 0 ? 0 : quotient);
                return true;
            case var t when t == typeof(float):
                result = (TOther)(object)(float)value.ToDouble(null);
                return true;
            case var t when t == typeof(double):
                result = (TOther)(object)value.ToDouble(null);
                return true;
            case var t when t == typeof(decimal):
                result = (TOther)(object)value.ToDecimal(null);
                return true;
            case var t when t == typeof(Ratio):
                result = (TOther)(object)value;
                return true;
            default:
                result = default!;
                return false;
        }
    }

    static bool INumberBase<Ratio>.TryConvertToTruncating<TOther>(Ratio value, out TOther result)
    {
        var quotient = value.Numerator / value.Denominator;

        switch (typeof(TOther))
        {
            case var t when t == typeof(byte):
                result = (TOther)(object)unchecked((byte)quotient);
                return true;
            case var t when t == typeof(sbyte):
                result = (TOther)(object)unchecked((sbyte)quotient);
                return true;
            case var t when t == typeof(short):
                result = (TOther)(object)unchecked((short)quotient);
                return true;
            case var t when t == typeof(ushort):
                result = (TOther)(object)unchecked((ushort)quotient);
                return true;
            case var t when t == typeof(int):
                result = (TOther)(object)unchecked((int)quotient);
                return true;
            case var t when t == typeof(uint):
                result = (TOther)(object)unchecked((uint)quotient);
                return true;
            case var t when t == typeof(long):
                result = (TOther)(object)quotient;
                return true;
            case var t when t == typeof(ulong):
                result = (TOther)(object)unchecked((ulong)quotient);
                return true;
            case var t when t == typeof(nint):
                result = (TOther)(object)unchecked((nint)quotient);
                return true;
            case var t when t == typeof(nuint):
                result = (TOther)(object)unchecked((nuint)quotient);
                return true;
            case var t when t == typeof(float):
                result = (TOther)(object)(float)value.ToDouble(null);
                return true;
            case var t when t == typeof(double):
                result = (TOther)(object)value.ToDouble(null);
                return true;
            case var t when t == typeof(decimal):
                result = (TOther)(object)value.ToDecimal(null);
                return true;
            case var t when t == typeof(Ratio):
                result = (TOther)(object)value;
                return true;
            default:
                result = default!;
                return false;
        }
    }

    public static Ratio operator +(Ratio value) => Plus(value);

    public static Ratio operator +(Ratio left, Ratio right) => Add(left, right);

    public static Ratio operator -(Ratio value) => Negate(value);

    public static Ratio operator -(Ratio left, Ratio right) => Subtract(left, right);

    public static Ratio operator ++(Ratio value) => Increment(value);

    public static Ratio operator --(Ratio value) => Decrement(value);

    public static Ratio operator *(Ratio left, Ratio right) => Multiply(left, right);

    public static Ratio operator /(Ratio left, Ratio right) => Divide(left, right);

    public static bool operator ==(Ratio? left, Ratio? right) => right?.Equals(left) ?? left is null;

    public static bool operator !=(Ratio? left, Ratio? right) => !(left == right);

    public static Ratio Plus(Ratio value) => value;

    public static Ratio Add(Ratio left, Ratio right) =>
        left.Denominator == right.Denominator
            ? new(left.Numerator + right.Numerator, left.Denominator)
            : new((left.Numerator * right.Denominator) + (right.Numerator * left.Denominator), left.Denominator * right.Denominator);

    public static Ratio Negate(Ratio item) => new(-item.Numerator, item.Denominator);

    public static Ratio Subtract(Ratio left, Ratio right) =>
        left.Denominator == right.Denominator
            ? new(left.Numerator - right.Numerator, left.Denominator)
            : new((left.Numerator * right.Denominator) - (right.Numerator * left.Denominator), left.Denominator * right.Denominator);

    public static Ratio Increment(Ratio item) => Add(item, One);

    public static Ratio Decrement(Ratio item) => Subtract(item, One);

    public static Ratio Multiply(Ratio left, Ratio right) =>
        new(left.Numerator * right.Numerator, left.Denominator * right.Denominator);

    public static Ratio Multiply(Ratio left, long right) => new(left.Numerator * right, left.Denominator);

    public static Ratio Multiply(Ratio left, int right) => new(left.Numerator * right, left.Denominator);

    public static double Multiply(Ratio left, double right) => left.ToDouble(null) * right;

    public static decimal Multiply(Ratio left, decimal right) => left.ToDecimal(null) * right;

    public static Ratio Divide(Ratio left, Ratio right) => Multiply(left, right.OneOver());

    public static Ratio Divide(Ratio left, long right) => new(left.Numerator, left.Denominator * right);

    public static Ratio Divide(Ratio left, int right) => new(left.Numerator, left.Denominator * right);

    public static double Divide(Ratio left, double right) => left.ToDouble(null) / right;

    public static decimal Divide(Ratio left, decimal right) => left.ToDecimal(null) / right;

    public static Ratio Mod(Ratio left, Ratio right) =>
        throw new NotSupportedException("Modulo operator is not supported for Ratio.");

    public Ratio Normalize() =>
        Numerator == 0L
            ? new(0L)
            : DivideBothBy(Gcd);

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

    public override int GetHashCode()
    {
        var normalized = Normalize();
        HashCode hash = new();
        hash.Add(normalized.Numerator);
        hash.Add(normalized.Denominator);
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
            return (x, y);
        }

        var absXNum = Math.Abs(x.Numerator);
        var absYNum = Math.Abs(y.Numerator);

        if (x.Denominator == y.Denominator)
        {
            return absXNum > absYNum ? (y, x) : (x, y);
        }

        if (absXNum == absYNum)
        {
            return x.Denominator < y.Denominator ? (y, x) : (x, y);
        }

        var leftProduct = absXNum * y.Denominator;
        var rightProduct = absYNum * x.Denominator;
        return leftProduct > rightProduct ? (y, x) : (x, y);
    }
}
