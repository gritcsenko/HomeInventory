using System.Reflection;

namespace HomeInventory.Tests.Framework;

internal static class ReflectionMethods
{
    private const BindingFlags _createInstanceBindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance;

    public static T? CreateInstance<T>(params object?[]? args)
        where T : class
        => Activator.CreateInstance(typeof(T), _createInstanceBindingAttr, null, args, null) as T;
}
