namespace HomeInventory.Tests;

/// <summary>
/// Indicates a test is expected to fail if run.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class BrokenTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrokenTestAttribute"/> class.
    /// </summary>
    public BrokenTestAttribute() : base("Broken")
    {
    }
}
