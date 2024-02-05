namespace HomeInventory.Core;

public static class DecimalExtensions
{
    public static bool IsPow10(this decimal d) => d.IsPowOf(power: 10);

    public static bool IsPowOf(this decimal d, int power)
    {
        var abs = decimal.Abs(d);
        var value = GetBase(abs);
        return value.DividesBy(power);
    }

    private static Int128 GetBase(decimal abs)
    {
        Span<int> bits = stackalloc int[4];
        _ = decimal.GetBits(abs, bits);
        return (Int128)bits[0] + ((Int128)bits[1] << 32) + ((Int128)bits[2] << 64);
    }

    private static bool DividesBy(this Int128 value, Int128 power)
    {
        while (value > 0)
        {
            (value, var divides) = value.DividesByCore(power);
            if (divides.HasValue)
            {
                return divides.Value;
            }
        }

        return false;
    }

    private static (Int128 Quotient, Optional<bool> Divides) DividesByCore(this Int128 value, Int128 power)
    {
        var (quotient, reminder) = Int128.DivRem(value, power);

        if (reminder == 0)
        {
            return (quotient, quotient == 1 ? Optional.Some(true) : Optional<bool>.None);
        }

        return (quotient, false);
    }
}
