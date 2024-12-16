namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    IReadOnlyCollection<Type> Dependencies { get; }

    IFeatureFlag Flag { get; }

    Task AddServicesAsync(IModuleServicesContext context);

    Task BuildAppAsync(IModuleBuildContext context);
}
