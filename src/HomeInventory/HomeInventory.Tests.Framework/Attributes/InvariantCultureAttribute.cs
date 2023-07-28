using System.Globalization;
using System.Reflection;
using Xunit.Sdk;

namespace HomeInventory.Tests.Framework.Attributes;

public sealed class InvariantCultureAttribute : BeforeAfterTestAttribute
{
    private CultureInfo _originalCulture = CultureInfo.InvariantCulture;

    public override void Before(MethodInfo methodUnderTest)
    {
        _originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public override void After(MethodInfo methodUnderTest)
    {
        Thread.CurrentThread.CurrentCulture = _originalCulture;
    }
}
