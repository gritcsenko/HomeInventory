namespace HomeInventory.Tests.Framework;

public sealed class DisposableAction(Action action) : Disposable
{
    private readonly Action _action = action;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _action();
        }

        base.Dispose(disposing);
    }
}