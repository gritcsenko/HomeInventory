namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    IReadOnlyCollection<Type> Dependencies { get; }

    IFeatureFlag Flag { get; }

    Task AddServicesAsync(ModuleServicesContext context);

    Task BuildAppAsync(ModuleBuildContext context);
}
