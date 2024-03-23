using System.Reflection;

namespace HomeInventory.Application;

internal class MappingAssemblySource(IReadOnlyCollection<Assembly> assemblies) : IMappingAssemblySource
{
    private readonly IReadOnlyCollection<Assembly> _assemblies = assemblies;

    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
}
