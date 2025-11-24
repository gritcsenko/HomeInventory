using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace HomeInventory.Core;

[SuppressMessage("Minor Code Smell", "S2325:Methods and properties that don't access instance data should be static")]
public static class TypeExtensions
{
    private const BindingFlags _getFieldBindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;

    extension(Type type)
    {
        public IEnumerable<TFieldType> GetFieldValuesOfType<TFieldType>()
        {
            var fieldType = typeof(TFieldType);
            return type.GetFields(_getFieldBindingAttr)
                .Where(field => fieldType.IsAssignableFrom(field.FieldType))
                .Select(field => field.GetValue(null))
                .Cast<TFieldType>();
        }

        public IEnumerable<(FieldInfo Field, TFieldType? Value)> GetFieldsOfType<TFieldType>()
        {
            var fieldType = typeof(TFieldType);
            return type.GetFields(_getFieldBindingAttr)
                .Where(field => fieldType.IsAssignableFrom(field.FieldType))
                .Select(field => (field, (TFieldType?)field.GetValue(null)));
        }

        public string GetFormattedName() =>
            type switch
            {
                { IsGenericType: true } => type.FormatGenericType(),
                _ => type.Name,
            };

        private string FormatGenericType()
        {
            var args = type.GenericTypeArguments;
            var name = type.Name;
            return $"{name.Replace("`" + args.Length, string.Empty, StringComparison.Ordinal)}<{string.Join(',', args.Select(GetFormattedName))}>";
        }
    }
}
