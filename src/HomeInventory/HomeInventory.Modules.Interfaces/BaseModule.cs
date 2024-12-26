using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public abstract class BaseModule : IModule
{
    private readonly List<Type> _dependencies = [];

    public IReadOnlyCollection<Type> Dependencies => _dependencies;

    public virtual IFeatureFlag Flag { get; } = new AlwaysEnabledFlag("<Always Enabled>");

    public virtual Task AddServicesAsync(IModuleServicesContext context) => Task.CompletedTask;

    public virtual Task BuildAppAsync(IModuleBuildContext context) => Task.CompletedTask;

    protected void DependsOn<TModule>()
        where TModule : class, IModule =>
        _dependencies.Add(typeof(TModule));

    private class AlwaysEnabledFlag(string name) : IFeatureFlag
    {
        private readonly Task<bool> _enabled = Task.FromResult(true);

        public string Name { get; } = name;

        public Task<bool> IsEnabledAsync(IFeatureManager manager) => _enabled;

        public IFeatureFlag<TContext> WithContext<TContext>(TContext context) => new AlwaysEnabledFlag<TContext>(Name, context);
    }

    private sealed class AlwaysEnabledFlag<TContext>(string name, TContext context) : AlwaysEnabledFlag(name), IFeatureFlag<TContext>
    {
        public TContext Context { get; } = context;
    }
}
