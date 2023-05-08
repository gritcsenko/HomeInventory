using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Web.OpenApi;

internal class SwaggerDefaultValues : IOperationFilter
{
    private readonly IReadOnlyCollection<ISwaggerOperationFilter> _childFilters;

    public SwaggerDefaultValues(IEnumerable<ISwaggerOperationFilter> childFilters)
    {
        _childFilters = childFilters.ToArray();
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var filter in _childFilters)
        {
            filter.Apply(operation, context);
        }
    }
}
