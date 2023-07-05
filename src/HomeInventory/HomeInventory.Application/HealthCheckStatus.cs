namespace HomeInventory.Application;

public sealed class HealthCheckStatus
{
    public bool IsFailed { get; init; }

    public required string Description { get; init; }

    public Dictionary<string, object> Data { get; } = new();
}
