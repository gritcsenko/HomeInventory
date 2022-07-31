using System.Reflection;

namespace HomeInventory.Domain;

internal static class TypeExtensions
{
    private const BindingFlags BindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    public static IEnumerable<TFieldType> GetFieldsOfType<TFieldType>(this Type type)
    {
        foreach (var p in type.GetFields(BindingAttr))
        {
            if (type.IsAssignableFrom(p.FieldType))
            {
                yield return (TFieldType)p.GetValue(null)!;
            }
        }
    }
}
