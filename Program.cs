using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstate.Infrastructure.Config;
using RealEstate.Application.Contracts;
using RealEstate.Application.Services;
using RealEstate.Infrastructure.API.Repositories;
using RealEstate.Infrastructure.Seed;
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
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped<IMongoDatabase>(sp =>
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Global error handling middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();


app.Run();
