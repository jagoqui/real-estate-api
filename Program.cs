using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstate.Infrastructure.Config;
using RealEstate.Application.Contracts;
using RealEstate.Application.Services;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Configuración MongoDB
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
// Inyección de dependencias
// =======================
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();

builder.Services.AddScoped<IPropertyService, PropertyService>();

// =======================
// Configuración API
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

app.MapControllers();

// =======================
// Ejecutar semilla inicial
// =======================
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    var seeder = new DatabaseSeeder(database);
    await seeder.SeedAsync();
}

app.Run();
