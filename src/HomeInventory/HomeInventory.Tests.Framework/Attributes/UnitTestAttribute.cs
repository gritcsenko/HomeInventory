namespace HomeInventory.Tests.Framework.Attributes;

/// <summary>
/// Indicates a test is a unit test.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class UnitTestAttribute : CategoryTraitAttribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitTestAttribute"/> class.
    /// </summary>
    public UnitTestAttribute()
        : base("Unit")
    {
    }
}
