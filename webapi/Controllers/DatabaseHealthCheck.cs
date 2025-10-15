using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace webapi.Controllers
{
    public class DatabaseHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            
            if (Bootup.BootupSuccessful)
                return Task.FromResult(HealthCheckResult.Healthy());
            else
                return Task.FromResult(HealthCheckResult.Unhealthy("Health failed on bootup"));
        }
    }
}
