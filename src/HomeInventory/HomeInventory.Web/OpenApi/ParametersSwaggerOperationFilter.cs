using HomeInventory.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Web.OpenApi;

/// <summary>
/// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
/// </summary>
internal sealed class ParametersSwaggerOperationFilter(IOpenApiValueConverter converter) : ISwaggerOperationFilter
{
    private readonly IOpenApiValueConverter _converter = converter;

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        var parameterDescriptions = apiDescription.ParameterDescriptions;
        var parameters = operation.Parameters;

        foreach (var parameter in parameters.EmptyIfNull())
        {
            var parameterDescription = parameterDescriptions.First(p => p.Name == parameter.Name);

            parameter.Required |= parameterDescription.IsRequired;
            if (parameterDescription.ModelMetadata is { } modelMetadata)
            {
                UpdateFromMetadata(parameter, modelMetadata, parameterDescription.DefaultValue);
            }
        }
    }

    private void UpdateFromMetadata(OpenApiParameter parameter, ModelMetadata modelMetadata, object? defaultValue)
    {
        parameter.Description ??= modelMetadata.Description;
        if (parameter.Schema is { } schema)
        {
            schema.Default ??= _converter.Convert(defaultValue, modelMetadata.ModelType);
        }
    }
}
