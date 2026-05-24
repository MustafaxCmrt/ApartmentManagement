using ApartmentManagement.API.Services;
using ApartmentManagement.Application.Common.Interfaces;

namespace ApartmentManagement.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();

        services.AddJwtAuthentication(configuration);
        services.AddApiVersioningConfig();
        services.AddSwaggerServices();
        services.AddCorsPolicy(configuration);
        services.AddAppHealthChecks(configuration);
        services.AddRateLimiting();

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        return services;
    }
}
