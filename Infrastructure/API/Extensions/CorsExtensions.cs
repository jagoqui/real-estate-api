namespace RealEstate.Infrastructure.API.Extensions
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowedOrigins", policy =>
                {
                    policy.WithOrigins("https://v0-real-estate-website-eight-vert.vercel.app")
                        .SetIsOriginAllowed(origin =>
                            origin.StartsWith("http://localhost:") || origin.StartsWith("https://localhost:"))
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
