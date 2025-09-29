using RealEstate.Infrastructure.API.Middlewares;

namespace RealEstate.Infrastructure.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseCustomMiddlewares(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("AllowedOrigins");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.MapControllers();
        }
    }
}
