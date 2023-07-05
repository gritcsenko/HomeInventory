using System.Reflection;

namespace HomeInventory.Contracts.UserManagement.Validators;

public static class AssemblyReference
{
    public static readonly Assembly Assembly = typeof(AssemblyReference).Assembly;
}
