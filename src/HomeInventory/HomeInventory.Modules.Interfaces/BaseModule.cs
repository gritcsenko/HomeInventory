using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public abstract class BaseModule : IModule
{
    private readonly List<Type> _dependencies = [];

    public IReadOnlyCollection<Type> Dependencies => _dependencies;

    public virtual IFeatureFlag Flag { get; } = new AlwaysEnabledFlag("<Always Enabled>");

    public virtual Task AddServicesAsync(IModuleServicesContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public virtual Task BuildAppAsync(IModuleBuildContext context, CancellationToken cancellationToken = default) => Task.CompletedTask;

    protected void DependsOn<TModule>()
        where TModule : class, IModule =>
        _dependencies.Add(typeof(TModule));

    private class AlwaysEnabledFlag(string name) : FeatureFlag(name)
    {
        private readonly Task<bool> _enabled = Task.FromResult(true);

        public override Task<bool> IsEnabledAsync(IFeatureManager manager) => _enabled;

        public override IFeatureFlag<TContext> WithContext<TContext>(TContext context) => new AlwaysEnabledFlag<TContext>(Name, context);
    }

    private sealed class AlwaysEnabledFlag<TContext>(string name, TContext context) : AlwaysEnabledFlag(name), IFeatureFlag<TContext>
    {
        public TContext Context { get; } = context;
    }
}
