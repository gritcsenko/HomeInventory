namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Indicates a test is expected to fail if run.
/// </summary>
public sealed class BrokenTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrokenTestAttribute"/> class.
    /// </summary>
    public BrokenTestAttribute()
        : base("Broken")
    {
    }
}
