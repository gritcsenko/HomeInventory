using System.Reflection;

namespace HomeInventory.Domain;

internal static class TypeExtensions
{
    private const BindingFlags BindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    public static IEnumerable<TFieldType> GetFieldsOfType<TFieldType>(this Type type)
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
}
