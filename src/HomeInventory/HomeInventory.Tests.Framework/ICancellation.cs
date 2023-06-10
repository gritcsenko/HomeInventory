namespace HomeInventory.Tests.Framework;

public interface ICancellation
{
    CancellationToken Token { get; }

    void Cancel();
}
