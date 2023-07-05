using System.Reflection;

namespace HomeInventory.Core;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
