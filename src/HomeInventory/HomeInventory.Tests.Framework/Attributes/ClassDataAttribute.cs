namespace HomeInventory.Tests.Framework.Attributes;

public sealed class ClassDataAttribute<TTheoryData> : ClassDataAttribute
    where TTheoryData : TheoryData
{
    public ClassDataAttribute()
        : base(typeof(TTheoryData))
    {
    }
}
