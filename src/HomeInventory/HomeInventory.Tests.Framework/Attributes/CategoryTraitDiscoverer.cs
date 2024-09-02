using Xunit.Abstractions;
using Xunit.Sdk;

namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// This class discovers all of the xUnit tests and test classes that have
/// applied the TraitDiscoverer attribute for this trait discoverer.
/// This class is referenced for example by the Visual Studio test explorer to discover test traits such as unit tests or integration tests.
/// Originally derived from the <a href="https://github.com/xunit/samples.xunit/tree/master/TraitExtensibility">xUnit TraitExtensibility Sample</a>.
/// </summary>
public class CategoryTraitDiscoverer(IMessageSink sink) : ITraitDiscoverer
{
    /// <summary>
    /// The namespace of this class
    /// </summary>
    internal const string _assemblyName = nameof(HomeInventory) + "." + nameof(Tests) + "." + nameof(Framework);

    /// <summary>
    /// The fully qualified name of this class
    /// </summary>
    internal const string _fullyQualifiedName = _assemblyName + "." + nameof(Attributes) + "." + nameof(CategoryTraitDiscoverer);

    public IMessageSink Sink { get; } = sink;

    /// <summary>
    /// Gets the trait values from the Category attribute.
    /// </summary>
    /// <param name="traitAttribute">
    /// The trait attribute containing the trait values.
    /// </param>
    /// <returns>
    /// The trait values.
    /// </returns>
    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var info = traitAttribute as ReflectionAttributeInfo;
        var categoryAttribute = info?.Attribute as CategoryTraitAttribute;
        var categoryValue = categoryAttribute?.Category;

        if (!string.IsNullOrWhiteSpace(categoryValue))
        {
            yield return new KeyValuePair<string, string>("Category", categoryValue);
        }
    }
}
