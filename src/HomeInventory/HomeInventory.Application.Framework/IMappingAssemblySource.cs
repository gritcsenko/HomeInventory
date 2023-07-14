using System.Reflection;

namespace HomeInventory.Application;

public interface IMappingAssemblySource
{
    IEnumerable<Assembly> GetAssemblies();
}
