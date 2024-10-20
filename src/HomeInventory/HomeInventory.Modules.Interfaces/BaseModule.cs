namespace HomeInventory.Modules.Interfaces;

public abstract class BaseModule : IModule
{
    private readonly List<Type> _dependencies = [];

    public IReadOnlyCollection<Type> Dependencies => _dependencies;

    public virtual Task AddServicesAsync(ModuleServicesContext context)
    {
        return Task.CompletedTask;
    }

    public virtual Task BuildAppAsync(ModuleBuildContext context)
    {
        return Task.CompletedTask;
    }

    protected void DependsOn<TModule>()
        where TModule : class, IModule =>
        _dependencies.Add(typeof(TModule));
}
