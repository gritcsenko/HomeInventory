using System.Reflection;

namespace HomeInventory.Application;

internal class MappingAssemblySource : IMappingAssemblySource
{
    private readonly Assembly[] _assemblies;

    public MappingAssemblySource(params Assembly[] assemblies) => _assemblies = assemblies;

    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
}
