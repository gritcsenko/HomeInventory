using System.Reflection;

namespace HomeInventory.Domain.Extensions;

public static class TypeExtensions
{
    private const BindingFlags BindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    internal static IEnumerable<TFieldType> GetFieldsOfType<TFieldType>(this Type type)
    {
        var fields = type.GetFields(BindingAttr);
        foreach (var field in fields)
        {
            if (type.IsAssignableFrom(field.FieldType))
            {
                yield return (TFieldType)field.GetValue(null)!;
            }
        }
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
        return $"{name.Replace("`" + args.Length, string.Empty)}<{string.Join(',', args.Select(GetFormattedName))}>";
    }
}
