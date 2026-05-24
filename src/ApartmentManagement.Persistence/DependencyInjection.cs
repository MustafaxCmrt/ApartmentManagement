using ApartmentManagement.Application.Common.Interfaces;
using ApartmentManagement.Persistence.Contexts;
using ApartmentManagement.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApartmentManagement.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntityInterceptor>();
        services.AddScoped<SoftDeleteInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionString 'DefaultConnection' was not found.");

        services.AddDbContext<ApplicationDbContext>((sp, opt) =>
        {
            opt.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                my =>
                {
                    my.EnableRetryOnFailure(3, TimeSpan.FromSeconds(2), null);
                    my.CommandTimeout(30);
                    my.MigrationsAssembly("ApartmentManagement.Persistence");
                });

            opt.AddInterceptors(
                sp.GetRequiredService<AuditableEntityInterceptor>(),
                sp.GetRequiredService<SoftDeleteInterceptor>());
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}
