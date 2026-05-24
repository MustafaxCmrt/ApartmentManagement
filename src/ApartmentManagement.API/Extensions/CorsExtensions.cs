namespace ApartmentManagement.API.Extensions;

public static class CorsExtensions
{
    public const string PolicyName = "ApartmanCors";

    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, builder =>
            {
                if (origins.Length == 0)
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                }
                else
                {
                    builder
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            });
        });

        return services;
    }
}
