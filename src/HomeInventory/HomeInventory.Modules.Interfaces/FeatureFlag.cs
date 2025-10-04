using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

internal class FeatureFlag : IFeatureFlag
{
    internal FeatureFlag(string name) => Name = name;

    public string Name { get; }

    public virtual async Task<bool> IsEnabledAsync(IFeatureManager manager) => await manager.IsEnabledAsync(Name);

    public virtual IFeatureFlag<TContext> WithContext<TContext>(TContext context) => Create(Name, context);

    public static IFeatureFlag Create(string name) => new FeatureFlag(name);

    public static IFeatureFlag<TContext> Create<TContext>(string name, TContext context) => new ContextFeatureFlag<TContext>(name, context);

    private sealed class ContextFeatureFlag<TContext>(string name, TContext context) : FeatureFlag(name), IFeatureFlag<TContext>
    {
        public TContext Context { get; } = context;

        public override async Task<bool> IsEnabledAsync(IFeatureManager manager) => await manager.IsEnabledAsync(Name, Context);
    }
}
