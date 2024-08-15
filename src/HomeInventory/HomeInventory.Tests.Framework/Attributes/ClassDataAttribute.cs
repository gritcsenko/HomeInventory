namespace HomeInventory.Tests.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ClassDataAttribute<TTheoryData> : ClassDataAttribute
    where TTheoryData : TheoryData
{
    public ClassDataAttribute()
        : base(typeof(TTheoryData))
    {
    }
}
