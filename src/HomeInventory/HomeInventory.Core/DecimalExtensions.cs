namespace HomeInventory.Core;

public static class DecimalExtensions
{
    public static bool IsPow10(this decimal d)
    {
        Span<int> bits = stackalloc int[4];
        _ = decimal.GetBits(d, bits);
        var value = (Int128)bits[0] + ((Int128)bits[1] << 32) + ((Int128)bits[2] << 64);
        if (value == 1)
        {
            return true;
        }

        while (value > 1)
        {
            var (div, rem) = Int128.DivRem(value, 10);
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
