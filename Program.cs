using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstate.Infrastructure.Config;
using RealEstate.Application.Contracts;
using RealEstate.Infrastructure.API.Services;
using RealEstate.Infrastructure.API.Repositories;
using RealEstate.Infrastructure.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// =======================
// MongoDB Configuration
// =======================
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;

    if (string.IsNullOrEmpty(settings.ConnectionString))
        throw new InvalidOperationException("MongoDB ConnectionString is not configured!");

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();

    if (string.IsNullOrEmpty(settings.DatabaseName))
        throw new InvalidOperationException("MongoDB DatabaseName is not configured!");

    return client.GetDatabase(settings.DatabaseName);
});

// =======================
// Dependency Injection
// =======================
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();

builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IOwnerService, OwnerService>(); // <- Added registration

// =======================
// API Configuration
// =======================
builder.Services.AddControllers();
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
