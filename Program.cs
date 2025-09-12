using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RealEstate.Infrastructure.Config;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Seed; // 👈 importar el Seeder

var builder = WebApplication.CreateBuilder(args);

// Configuración MongoDB
builder.Services.Configure<DatabaseSettings>(
    builder.Configuration.GetSection("DatabaseSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("RealEstateCluster");
});

// Repositorios
builder.Services.AddScoped<PropertyRepository>();
builder.Services.AddScoped<OwnerRepository>(); 

// Seeder
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "RealEstate API",
        Version = "v1"
    });
});

var app = builder.Build();

// Ejecutar Seeder
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Swagger en dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RealEstate API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
