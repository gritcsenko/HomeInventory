namespace HomeInventory.Tests.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ClassDataAttribute<TTheoryData>() : ClassDataAttribute(typeof(TTheoryData))
    where TTheoryData : TheoryData;
