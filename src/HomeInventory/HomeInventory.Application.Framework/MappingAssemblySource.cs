using System.Reflection;

namespace HomeInventory.Application;

internal class MappingAssemblySource : IMappingAssemblySource
{
    private readonly IReadOnlyCollection<Assembly> _assemblies;

    public MappingAssemblySource(IReadOnlyCollection<Assembly> assemblies) => _assemblies = assemblies;

    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
}
