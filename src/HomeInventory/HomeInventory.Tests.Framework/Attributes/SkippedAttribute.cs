namespace HomeInventory.Tests.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class SkippedAttribute : FactAttribute
{
    public SkippedAttribute()
        : this("Not specified")
    {
    }

    public SkippedAttribute(string reason)
    {
        Skip = reason;
        Reason = reason;
    }

    public string Reason { get; }
}
