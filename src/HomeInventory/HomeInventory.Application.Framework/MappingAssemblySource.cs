using System.Reflection;

namespace HomeInventory.Application.Framework;

internal sealed class MappingAssemblySource(IReadOnlyCollection<Assembly> assemblies) : IMappingAssemblySource
{
    private readonly IReadOnlyCollection<Assembly> _assemblies = assemblies;

    public IEnumerable<Assembly> GetAssemblies() => _assemblies;
}
