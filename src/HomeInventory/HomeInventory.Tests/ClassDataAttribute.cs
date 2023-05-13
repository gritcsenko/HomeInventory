namespace HomeInventory.Tests;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class ClassDataAttribute<TTheoryData> : ClassDataAttribute
    where TTheoryData : TheoryData
{
    public ClassDataAttribute()
        : base(typeof(TTheoryData))
    {
    }
}
