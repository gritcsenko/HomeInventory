namespace HomeInventory.Domain;

internal class FixedTimeProvider(TimeProvider parent) : TimeProvider
{
    private readonly DateTimeOffset _now = parent.GetUtcNow();

    public override DateTimeOffset GetUtcNow() => _now;
}
