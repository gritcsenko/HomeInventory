using System.Reflection;

namespace HomeInventory.Application;

public class MappingAssemblySource : IMappingAssemblySource
{
    private readonly Assembly _assembly;

    public MappingAssemblySource(Assembly assembly)
    {
        _assembly = assembly;
    }

    public Assembly GetAssembly() => _assembly;
}
