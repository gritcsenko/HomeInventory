using System.Reflection;

namespace HomeInventory.Core;

public static class ReflectionMethods
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "Designed to call internal constructors")]
    private const BindingFlags _createInstanceBindingAttr = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance;

    public static T? CreateInstance<T>(params object?[]? args)
        where T : class
        => Activator.CreateInstance(typeof(T), _createInstanceBindingAttr, null, args, null) as T;
}
