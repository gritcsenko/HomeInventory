namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    IReadOnlyCollection<Type> Dependencies { get; }

    Task AddServicesAsync(ModuleServicesContext context);

    Task BuildAppAsync(ModuleBuildContext context);
}
