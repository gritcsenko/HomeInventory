using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Web.OpenApi;

/// <summary>
/// REF: https://github.com/dotnet/aspnetcore/issues/43493
/// REF: https://github.com/dotnet/aspnetcore/issues/35091
/// </summary>
internal sealed class DeprecatedSwaggerOperationFilter : ISwaggerOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        operation.Deprecated |= apiDescription.IsDeprecated();
    }
}
