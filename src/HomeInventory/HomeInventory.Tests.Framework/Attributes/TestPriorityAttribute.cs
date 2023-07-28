namespace HomeInventory.Tests.Framework.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class TestPriorityAttribute : Attribute
{
    public TestPriorityAttribute(int priority)
    {
        Priority = priority;
    }

    public int Priority { get; }
}
