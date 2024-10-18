using Microsoft.FeatureManagement;

namespace HomeInventory.Modules.Interfaces;

public static class FeatureManagerExtensions
{
    public static IAsyncEnumerable<IFeatureFlag> GetExistingFeaturesAsync(this IFeatureManager manager) => manager.GetFeatureNamesAsync().Select(name => FeatureFlag.Create(name));
}
