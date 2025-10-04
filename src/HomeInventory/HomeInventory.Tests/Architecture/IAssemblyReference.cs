using System.Reflection;

namespace HomeInventory.Tests.Architecture;

public interface IAssemblyReference
{
    Assembly Assembly { get; }

    string Namespace { get; }
}
