using System.Reflection;

namespace HomeInventory.Infrastructure.Framework;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
