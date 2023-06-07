namespace HomeInventory.Tests.Framework.Attributes;

public sealed class SkippedAttribute : FactAttribute
{
    public SkippedAttribute()
        : this("Not specified")
    {
    }

    public SkippedAttribute(string reason)
    {
        Skip = reason;
    }
}
