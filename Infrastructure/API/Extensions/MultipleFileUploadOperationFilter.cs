using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace RealEstate.Infrastructure.API.Extensions
{
    public class MultipleFileUploadOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var parameters = context.MethodInfo.GetParameters();

            var multiFileParams = parameters
                .Where(static p =>
                    p.ParameterType == typeof(List<IFormFile>) ||
                    p.ParameterType == typeof(IFormFile[]) ||
                    (p.ParameterType.IsGenericType &&
                     p.ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>) &&
                     p.ParameterType.GenericTypeArguments[0] == typeof(IFormFile)))
                .ToList();

            if (!multiFileParams.Any())
                return;

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
                                static p => p.Name ?? "files",
                                static _ => new OpenApiSchema
                                {
                                    Type = "array",
                                    Items = new OpenApiSchema
                                    {
                                        Type = "string",
                                        Format = "binary",
                                    },

                                    Extensions = new Dictionary<string, IOpenApiExtension>
                                    {
                                        ["x-ms-allow-multiple"] = new OpenApiBoolean(true),
                                        ["x-multiple"] = new OpenApiBoolean(true),
                                    },
                                }),
                        },
                    },
                },
            };
        }
    }
}
