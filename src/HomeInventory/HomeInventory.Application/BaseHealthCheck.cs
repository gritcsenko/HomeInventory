using DotNext;
using HomeInventory.Domain.Primitives;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HomeInventory.Application;

public abstract class BaseHealthCheck : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var (token, resources) = cancellationToken.WithTimeout(context.Registration.Timeout);
        try
        {
            var isHealthy = await IsHealthyAsync(token);
            if (isHealthy)
            {
                return HealthCheckResult.Healthy();
            }

            return new HealthCheckResult(context.Registration.FailureStatus);
        }
        catch (Exception ex)
        {
            return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
        }
        finally
        {
            Disposable.Dispose(resources);
        }
    }

    protected abstract ValueTask<bool> IsHealthyAsync(CancellationToken cancellationToken);
}
