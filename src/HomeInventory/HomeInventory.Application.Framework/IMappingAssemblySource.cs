using System.Reflection;

namespace HomeInventory.Application.Framework;

public interface IMappingAssemblySource
{
    IEnumerable<Assembly> GetAssemblies();
}
