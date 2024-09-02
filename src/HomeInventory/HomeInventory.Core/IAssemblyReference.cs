using System.Reflection;

namespace HomeInventory.Core;

public interface IAssemblyReference
{
    Assembly Assembly { get; }

    string Namespace { get; }
}
