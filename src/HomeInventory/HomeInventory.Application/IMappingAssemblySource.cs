using System.Reflection;

namespace HomeInventory.Application;

public interface IMappingAssemblySource
{
    Assembly GetAssembly();
}
