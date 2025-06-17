using System.Reflection;

namespace HomeInventory.UI;

/// <summary>
/// Configuration values for <see cref="ViewLocator"/> instances.
/// </summary>
public sealed record ViewLocatorOptions
{
    /// <summary>
    /// When <see langword="true"/>, the locator subscribes to <see cref="AppDomain.AssemblyLoad"/> and scans
    /// every assembly loaded after construction.
    /// </summary>
    public bool AutoSubscribeAssemblyLoad { get; init; }

    /// <summary>
    /// Assemblies that are scanned immediately after the locator is created.
    /// </summary>
    public IReadOnlyCollection<Assembly> InitialAssemblies { get; init; } = [];
}