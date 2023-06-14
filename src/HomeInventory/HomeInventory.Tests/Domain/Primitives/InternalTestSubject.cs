namespace HomeInventory.Tests.Domain.Primitives;

public sealed class InternalTestSubject
{
    internal InternalTestSubject()
    {
    }

    internal InternalTestSubject(object arg)
    {
        _ = arg;
    }
}
