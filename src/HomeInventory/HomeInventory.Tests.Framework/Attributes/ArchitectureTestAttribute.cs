namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Indicates a test is a architecture test.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class ArchitectureTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArchitectureTestAttribute"/> class.
    /// </summary>
    public ArchitectureTestAttribute()
        : base("Architecture")
    {
    }
}
