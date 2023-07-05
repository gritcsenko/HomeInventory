using System.Reflection;

namespace HomeInventory.Domain.Primitives;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
