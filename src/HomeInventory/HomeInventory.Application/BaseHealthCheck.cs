using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Application;

public abstract class BaseHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var (token, resources) = cancellationToken.WithTimeout(context.Registration.Timeout);
        return await Execute.AndCatchAsync(
            async () =>
            {
                var isHealthy = await IsHealthyAsync(token);
                if (isHealthy)
                {
                    return HealthCheckResult.Healthy();
                }

                return new HealthCheckResult(context.Registration.FailureStatus);
            },
            (Exception ex) => new HealthCheckResult(context.Registration.FailureStatus, exception: ex),
            () => Disposable.Dispose(resources));
    }

    protected abstract ValueTask<bool> IsHealthyAsync(CancellationToken cancellationToken);
}
