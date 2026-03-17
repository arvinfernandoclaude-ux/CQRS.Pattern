using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CQRS.Pattern.Infrastructure.HealthChecks;

internal sealed class SelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Application is running."));
    }
}
