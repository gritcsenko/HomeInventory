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
            var (div, rem) = Int128.DivRem(value, power);
            if (rem != 0)
            {
                return false;
            }
            if (div == 1)
            {
                return true;
            }
            value = div;
        }

        return false;
    }
}
