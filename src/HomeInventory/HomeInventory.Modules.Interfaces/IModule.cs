namespace HomeInventory.Modules.Interfaces;

public interface IModule
{
    IReadOnlyCollection<Type> Dependencies { get; }

    IFeatureFlag Flag { get; }

    Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default);

    Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default);
}
