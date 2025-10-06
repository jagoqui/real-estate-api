using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealEstate.Infrastructure.API.Extensions
{
    /// <summary>
    /// Permite que Swagger renderice correctamente un input múltiple para List&lt;IFormFile&gt;.
    /// </summary>
    public class MultipleFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameters = context.MethodInfo.GetParameters();

            // Detecta parámetros tipo List<IFormFile> o IEnumerable<IFormFile>
            var multiFileParams = parameters
                .Where(p =>
                    p.ParameterType == typeof(List<IFormFile>) ||
                    p.ParameterType == typeof(IFormFile[]) ||
                    (p.ParameterType.IsGenericType &&
                     p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                     p.ParameterType.GenericTypeArguments[0] == typeof(IFormFile)))
                .ToList();

            if (!multiFileParams.Any())
                return;

            // ✅ Genera un esquema compatible con Swagger UI (agrega explicitly multiple: true)
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = multiFileParams.ToDictionary(
                                p => p.Name,
                                _ => new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary"
                                    },
                                    // 👇 Hack necesario para que Swagger UI agregue el atributo "multiple"
                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        ["x-ms-allow-multiple"] = new OpenApiBoolean(true),
                                        ["x-multiple"] = new OpenApiBoolean(true)
                                    }
                                })
                        }
                    }
                }
            };
        }
    }
}
