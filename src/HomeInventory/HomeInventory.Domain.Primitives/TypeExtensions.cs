using System.Reflection;

namespace HomeInventory.Domain.Primitives;

public static class TypeExtensions
{
    private const BindingFlags _getFieldBindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    internal static IEnumerable<TFieldType> GetFieldsOfType<TFieldType>(this Type type)
    {
        var fieldType = typeof(TFieldType);
        return type.GetFields(_getFieldBindingAttr)
            .Where(field => fieldType.IsAssignableFrom(field.FieldType))
            .Select(field => field.GetValue(null))
            .Cast<TFieldType>();
    }

    public static string GetFormattedName(this Type type) =>
        type switch
        {
            { IsGenericType: true } => FormatGenericType(type),
            _ => type.Name,
        };


    private static string FormatGenericType(Type type)
    {
        var args = type.GenericTypeArguments;
        var name = type.Name;
        return $"{name.Replace("`" + args.Length, string.Empty, StringComparison.Ordinal)}<{string.Join(',', args.Select(GetFormattedName))}>";
    }
}
