using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public interface IFeatureFlag
{
    string Name { get; }

    IFeatureFlag<TContext> WithContext<TContext>(TContext context);

    Task<bool> IsEnabledAsync(IFeatureManager manager);
}

public interface IFeatureFlag<out TContext> : IFeatureFlag
{
    TContext Context { get; }
}
