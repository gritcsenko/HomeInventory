using System.Reflection;

namespace HomeInventory.Contracts.Validations;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
