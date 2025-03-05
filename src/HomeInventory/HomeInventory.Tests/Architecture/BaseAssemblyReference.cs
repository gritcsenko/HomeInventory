using System.Reflection;

namespace HomeInventory.Tests.Architecture;

public class BaseAssemblyReference : IAssemblyReference
{
    private readonly Type _currentType;

    public BaseAssemblyReference(Type type) => _currentType = type;

    protected BaseAssemblyReference() => _currentType = GetType();

    public Assembly Assembly => _currentType.Assembly;

    public string Namespace => _currentType.Namespace ?? throw new InvalidOperationException("No Namespace");
}
