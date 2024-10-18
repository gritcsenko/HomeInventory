using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public class FeatureFlag : IFeatureFlag
{
    internal FeatureFlag(string name) => Name = name;

    public string Name { get; }

    public virtual async Task<bool> IsEnabledAsync(IFeatureManager manager) => await manager.IsEnabledAsync(Name);

    public IFeatureFlag<TContext> WithContext<TContext>(TContext context) => Create(Name, context);

    public static IFeatureFlag Create(string name) => new FeatureFlag(name);

    public static IFeatureFlag<TContext> Create<TContext>(string name, TContext context) => new FeatureFlag<TContext>(name, context);
}

internal sealed class FeatureFlag<TContext>(string name, TContext context) : FeatureFlag(name), IFeatureFlag<TContext>
{
    public TContext Context { get; } = context;

    public override async Task<bool> IsEnabledAsync(IFeatureManager manager) => await manager.IsEnabledAsync(Name, Context);
}
