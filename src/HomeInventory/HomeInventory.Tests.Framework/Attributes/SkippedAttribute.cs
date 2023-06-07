using Xunit.Sdk;

namespace HomeInventory.Tests.Framework.Attributes;

[XunitTestCaseDiscoverer("Xunit.Sdk.FactDiscoverer", "xunit.execution.{Platform}")]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
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
