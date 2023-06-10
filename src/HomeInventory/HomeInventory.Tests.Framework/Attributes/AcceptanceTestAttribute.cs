namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Indicates a test is a user acceptance test.
/// </summary>
public sealed class AcceptanceTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcceptanceTestAttribute"/> class.
    /// </summary>
    public AcceptanceTestAttribute()
        : base("Acceptance")
    {
    }
}
