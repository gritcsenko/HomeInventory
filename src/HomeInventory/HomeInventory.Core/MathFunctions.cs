using System.Numerics;

namespace HomeInventory.Core;

public static class MathFunctions
{
    public static T GreatestCommonDivisor<T>(T x, T y, T zero)
        where T : IComparisonOperators<T, T, bool>, IModulusOperators<T, T, T>, IEqualityOperators<T, T, bool>,
        IUnaryNegationOperators<T, T>
    {
        var a = x.Abs(zero);
        var b = y.Abs(zero);

        while (a != zero && b != zero)
        {
            if (a > b)
            {
                a %= b;
            }
            else
            {
                b %= a;
            }
        }

        return a == zero ? b : a;
    }

    private static T Abs<T>(this T value, T zero)
        where T : IComparisonOperators<T, T, bool>, IUnaryNegationOperators<T, T> =>
        value < zero ? -value : value;
}
