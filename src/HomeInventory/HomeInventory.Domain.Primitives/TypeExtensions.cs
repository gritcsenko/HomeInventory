﻿using System.Reflection;

namespace HomeInventory.Domain.Primitives;

public static class TypeExtensions
{
    private const BindingFlags _getFieldBindingAttr = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
    private const BindingFlags _createInstanceBindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance;

    internal static IEnumerable<TFieldType> GetFieldsOfType<TFieldType>(this Type type)
    {
        var fields = type.GetFields(_getFieldBindingAttr);
        var fieldType = typeof(TFieldType);
        foreach (var field in fields)
        {
            if (fieldType.IsAssignableFrom(field.FieldType))
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

    public static T? CreateInstance<T>(params object?[]? args)
        where T : class
        => (T?)Activator.CreateInstance(typeof(T), _createInstanceBindingAttr, null, args, null);

    private static string FormatGenericType(Type type)
    {
        var args = type.GenericTypeArguments;
        var name = type.Name;
        return $"{name.Replace("`" + args.Length, string.Empty)}<{string.Join(',', args.Select(GetFormattedName))}>";
    }
}
