using System.Reflection;

namespace HomeInventory.Domain.Primitives;

public static class ReflectionMethods
{
    private const BindingFlags _createInstanceBindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance;

    public static T? CreateInstance<T>(params object?[]? args)
        where T : class
        => (T?)Activator.CreateInstance(typeof(T), _createInstanceBindingAttr, null, args, null);
}
