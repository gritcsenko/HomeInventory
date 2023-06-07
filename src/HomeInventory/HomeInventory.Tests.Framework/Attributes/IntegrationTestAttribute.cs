namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Indicates the test has external dependencies.
/// </summary>
public sealed class IntegrationTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IntegrationTestAttribute"/> class.
    /// </summary>
    public IntegrationTestAttribute()
        : base("Integration")
    {
    }
}
