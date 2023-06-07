namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Indicates a test is a architecture test.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ArchitectureTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptanceTestAttribute"/> class.
    /// </summary>
    public ArchitectureTestAttribute() : base("Architecture")
    {
    }
}