using Microsoft.FeatureManagement;

namespace HomeInventory.Application.Framework;

public static class FeatureManagerExtensions
{
    public static IAsyncEnumerable<IFeatureFlag> CreateFeaturesAsync(this IFeatureManager manager) => manager.GetFeatureNamesAsync().Select(name => FeatureFlag.Create(name));
}
