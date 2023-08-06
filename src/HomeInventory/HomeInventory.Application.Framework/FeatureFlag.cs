using Microsoft.FeatureManagement;

namespace HomeInventory.Application.Framework;

public sealed class FeatureFlag : IFeatureFlag
{
    internal FeatureFlag(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public async Task<bool> IsEnabledAsync(IFeatureManager manager, CancellationToken cancellationToken = default)
    {
        return await manager.IsEnabledAsync(Name, cancellationToken);
    }

    public static IFeatureFlag Create(string name) => new FeatureFlag(name);

    public static IFeatureFlag Create<TContext>(string name, TContext context) => new FeatureFlag<TContext>(name, context);
}

public sealed class FeatureFlag<TContext> : IFeatureFlag
{
    internal FeatureFlag(string name, TContext context)
    {
        Name = name;
        Context = context;
    }

    public string Name { get; }

    public TContext Context { get; }

    public async Task<bool> IsEnabledAsync(IFeatureManager manager, CancellationToken cancellationToken = default)
    {
        return await manager.IsEnabledAsync(Name, Context, cancellationToken);
    }
}
