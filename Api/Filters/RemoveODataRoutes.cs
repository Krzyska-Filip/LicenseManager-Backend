using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Api.Filters;

public class RemoveODataRoutes : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var toRemove = swaggerDoc.Paths
            .Where(p => p.Key.Contains("({key})"))
            .ToList();

        foreach (var path in toRemove)
        {
            swaggerDoc.Paths.Remove(path.Key);
        }
    }
}