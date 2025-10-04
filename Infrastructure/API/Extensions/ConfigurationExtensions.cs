using System.Text;
using CloudinaryDotNet;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Application.Services;
using RealEstate.Infrastructure.API.Repositories;
using RealEstate.Infrastructure.API.Services;
using RealEstate.Infrastructure.Config;
using RealEstate.Infrastructure.Services;
using RealEstate.Infrastructure.Utils;

namespace RealEstate.Infrastructure.API.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void LoadEnvironment(this WebApplicationBuilder builder)
        {
            string envPath = builder.Environment.IsDevelopment() ? "environments/.env" : "/etc/secrets/.env";
            if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
            {
                Env.Load(envPath);
                Console.WriteLine($"Loading environment variables from: {envPath}");
            }
            else
            {
                throw new FileNotFoundException($"The specified environment file does not exist: {envPath}");
            }
        }

        public static IServiceCollection AddCloudinary(this IServiceCollection services)
        {
            // 1. Obtener las credenciales del entorno
            var cloudName = Environment.GetEnvironmentVariable("Cloudinary__CloudName")
                             ?? throw new InvalidOperationException("Cloudinary__CloudName is missing.");
            var apiKey = Environment.GetEnvironmentVariable("Cloudinary__ApiKey")
                             ?? throw new InvalidOperationException("Cloudinary__ApiKey is missing.");
            var apiSecret = Environment.GetEnvironmentVariable("Cloudinary__ApiSecret")
                             ?? throw new InvalidOperationException("Cloudinary__ApiSecret is missing.");

            // 2. Configurar la clase CloudinarySettings (opcional, pero buena práctica)
            services.Configure<CloudinarySettings>(options =>
            {
                options.CloudName = cloudName;
                options.ApiKey = apiKey;
                options.ApiSecret = apiSecret;
            });

            // 3. Registrar el cliente Cloudinary como Singleton (es thread-safe y costoso de crear)
            services.AddSingleton(sp =>
            {
                var account = new Account(cloudName, apiKey, apiSecret);
                return new Cloudinary(account);
            });

            // 4. Registrar el servicio de subida de imágenes (Scoped)
            services.AddScoped<IImageUploadService, ImageUploadService>();

            return services;
        }

        public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = Environment.GetEnvironmentVariable("DatabaseSettings__ConnectionString")
                                   ?? throw new InvalidOperationException("MongoDB connection string missing");

            var databaseName = Environment.GetEnvironmentVariable("DatabaseSettings__DatabaseName")
                               ?? throw new InvalidOperationException("MongoDB database name missing");

            services.Configure<DatabaseSettings>(options =>
            {
                options.ConnectionString = connectionString;
                options.DatabaseName = databaseName;
            });

            services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

            services.AddScoped(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(databaseName);
            });

            return services;
        }

        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration config)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("Jwt__Secret")
                            ?? throw new InvalidOperationException("JWT secret missing");

            services.AddHttpContextAccessor();
            services.AddSingleton<JwtHelper>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    };
                });

            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IImageRepository, ImageRepository>();

            // Services
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IOwnerService, OwnerService>();
            services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
            services.AddScoped<IPropertyImageService, PropertyImageService>();
            services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
            services.AddScoped<IPropertyTraceService, PropertyTraceService>();

            // Auth
            services.AddScoped<IAuthService, AuthService>();

            // ✅ Users
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
