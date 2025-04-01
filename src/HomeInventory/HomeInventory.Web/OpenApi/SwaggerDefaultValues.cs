using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Web.OpenApi;

internal class SwaggerDefaultValues(IEnumerable<ISwaggerOperationFilter> childFilters) : IOperationFilter
{
    private readonly IReadOnlyCollection<ISwaggerOperationFilter> _childFilters = [.. childFilters];

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var filter in _childFilters)
        {
            filter.Apply(operation, context);
        }
    }
}
