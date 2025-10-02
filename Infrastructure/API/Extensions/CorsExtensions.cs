using Microsoft.Extensions.Configuration;

namespace RealEstate.Infrastructure.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            var corsAllowedOrigins = Environment.GetEnvironmentVariable("Cors__AllowedOrigins")
                     ?? throw new InvalidOperationException("Cors__AllowedOrigins is not set.");

            var allowedOrigins = corsAllowedOrigins?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", policy =>
                {
                    policy.SetIsOriginAllowed(origin =>
                        allowedOrigins.Any(allowed =>
                            allowed.EndsWith(":") ? origin.StartsWith(allowed) : origin == allowed))
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
