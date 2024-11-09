using HomeInventory.Modules.Interfaces;
using Microsoft.FeatureManagement;

namespace HomeInventory.Modules;

public static class FeatureManagerExtensions
{
    public static IAsyncEnumerable<IFeatureFlag> GetExistingFeaturesAsync(this IFeatureManager manager) => manager.GetFeatureNamesAsync().Select(name => FeatureFlag.Create(name));
}
