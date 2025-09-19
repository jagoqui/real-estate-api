using DotNetEnv;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.API.Conventions;
using RealEstate.Infrastructure.API.Middlewares;
using RealEstate.Infrastructure.API.Repositories;
using RealEstate.Infrastructure.API.Services;
using RealEstate.Infrastructure.Config;

var builder = WebApplication.CreateBuilder(args);

string envPath = builder.Environment.IsDevelopment() ? "environments/.env" : "/etc/secrets/.env";

if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
{
    Env.Load(envPath);
    Console.WriteLine($"Cargando variables de entorno desde: {envPath}");
}
else
{
    throw new FileNotFoundException($"The specified environment file does not exist: {envPath}");
}

// =======================
// MongoDB Configuration
// =======================
string GetEnvOrThrow(string name) =>
    Environment.GetEnvironmentVariable(name)
        ?? throw new InvalidOperationException($"{name} environment variable is not set or is empty.");

builder.Services.Configure<DatabaseSettings>(options =>
{
    options.ConnectionString = GetEnvOrThrow("DatabaseSettings__ConnectionString");
    options.DatabaseName = GetEnvOrThrow("DatabaseSettings__DatabaseName");
});

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// =======================
// Dependency Injection
// =======================
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();

builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();

builder.Services.AddScoped<IPropertyImageRepository, PropertyImageRepository>();
builder.Services.AddScoped<IPropertyImageService, PropertyImageService>();

builder.Services.AddScoped<IPropertyTraceRepository, PropertyTraceRepository>();
builder.Services.AddScoped<IPropertyTraceService, PropertyTraceService>();

// =======================
// API Configuration
// =======================
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new LowercaseControllerConvention());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// =======================
// Middleware
// =======================
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

// Global error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();
