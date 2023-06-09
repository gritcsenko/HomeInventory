namespace HomeInventory.Tests;

public interface ICancellation
{
    CancellationToken Token { get; }

    void Cancel();
}
