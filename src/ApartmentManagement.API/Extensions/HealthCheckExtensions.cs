namespace ApartmentManagement.API.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddAppHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var builder = services.AddHealthChecks();
        builder.AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "live" });

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            builder.AddMySql(connectionString, name: "mysql", tags: new[] { "db", "ready" });
        }

        return services;
    }
}
