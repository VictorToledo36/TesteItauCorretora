using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TesteItauCorretora.API.Middleware;

public class RequestIdHeaderFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var response in operation.Responses.Values)
        {
            if (response.Headers == null)
                response.Headers = new Dictionary<string, OpenApiHeader>();

            response.Headers["X-Request-Id"] = new OpenApiHeader
            {
                Description = "UUID único gerado por requisição para rastreabilidade",
                Schema = new OpenApiSchema { Type = "string" }
            };
        }
    }
}