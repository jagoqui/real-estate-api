using RealEstate.Infrastructure.API.Conventions;
using RealEstate.Infrastructure.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// =======================
// Load environment variables
// =======================
builder.LoadEnvironment();

// =======================
// MongoDB & JWT
// =======================
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddJwtAuth(builder.Configuration);

// =======================
// Application Services & Repositories
// =======================
builder.Services.AddApplicationServices();

// =======================
// Controllers & JSON options
// =======================
builder.Services.AddControllers(options =>
{
    options.Conventions.Add(new LowercaseControllerConvention());
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition =
        System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});

// =======================
// Swagger
// =======================
builder.Services.AddCustomSwagger();

// =======================
// CORS
// =======================
builder.Services.AddCustomCors();

var app = builder.Build();

// =======================
// Middleware
// =======================
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowedOrigins");
app.UseCustomMiddlewares();

app.Run();
