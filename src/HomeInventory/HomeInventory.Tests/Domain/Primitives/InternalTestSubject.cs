namespace HomeInventory.Tests.Domain.Primitives;

public sealed class InternalTestSubject
{
    internal InternalTestSubject()
    {
    }

#pragma warning disable IDE0060 // Remove unused parameter
    internal InternalTestSubject(object arg)
#pragma warning restore IDE0060 // Remove unused parameter
    {
    }
}
