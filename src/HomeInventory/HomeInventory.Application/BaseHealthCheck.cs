using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Application;

public abstract class BaseHealthCheck : IHealthCheck
{
    private static readonly IReadOnlyDictionary<string, object> _empty = new Dictionary<string, object>();

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await Execute.AndCatchAsync(
            async () =>
            {
                var status = await CheckHealthAsync(cancellationToken);
                var healthStatus = status.IsFailed ? context.Registration.FailureStatus : HealthStatus.Healthy;
                return new HealthCheckResult(healthStatus, status.Description, exception: null, status.Data);
            },
            (Exception ex) => new HealthCheckResult(context.Registration.FailureStatus, "Failed to perform healthcheck", ex, ExceptionData));

    protected abstract ValueTask<HealthCheckStatus> CheckHealthAsync(CancellationToken cancellationToken);

    protected virtual IReadOnlyDictionary<string, object> ExceptionData => _empty;
}
