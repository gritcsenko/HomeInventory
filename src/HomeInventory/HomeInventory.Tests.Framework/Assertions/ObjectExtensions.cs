using System.Globalization;

namespace HomeInventory.Tests.Framework.Assertions;

internal static class ObjectExtensions
{
    public static Func<T?, T?, bool> GetComparer<T>()
    {
        if (typeof(T) == typeof(object))
        {
            return static (actual, expected) =>
            {
                if (actual is null && expected is null)
                {
                    return true;
                }

                if (actual is null || expected is null)
                {
                    return false;
                }

                if (EqualityComparer<T>.Default.Equals(actual, expected))
                {
                    return true;
                }

                // CompareNumerics is only relevant for numerics boxed in an object.
                return CompareNumerics(actual, expected, CultureInfo.InvariantCulture);
            };
        }

        // Avoid causing any boxing for value types
        return EqualityComparer<T>.Default.Equals;
    }

    private static bool CompareNumerics(object actual, object expected, IFormatProvider formatProvider)
    {
        var expectedType = expected.GetType();
        var actualType = actual.GetType();

        return actualType != expectedType
            && actual.IsNumericType()
            && expected.IsNumericType()
            && CanConvert(actual, expected, formatProvider)
            && CanConvert(expected, actual, formatProvider);
    }

    private static bool CanConvert(object source, object target, IFormatProvider formatProvider) =>
        Execute.AndCatch(() =>
            {
                var convertedToTarget = source.ConvertTo(target.GetType(), formatProvider);
                var convertedToSource = convertedToTarget.ConvertTo(source.GetType(), formatProvider);
                return source.Equals(convertedToSource) && convertedToTarget.Equals(target);
            },
            (Exception _) => false);

    private static object ConvertTo(this object source, Type targetType, IFormatProvider formatProvider) =>
        Convert.ChangeType(source, targetType, formatProvider);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S4201:Null checks should not be used with \"is\"", Justification = "Due to https://github.com/dotnet/runtime/issues/47920#issuecomment-774481505")]
    private static bool IsNumericType(this object obj) =>
        obj is not null and (
            int or
            long or
            float or
            double or
            decimal or
            sbyte or
            byte or
            short or
            ushort or
            uint or
            ulong);
}
