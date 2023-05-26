using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Web.OpenApi;

/// <summary>
/// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1752#issue-663991077
/// </summary>
internal sealed class ResponsesSwaggerOperationFilter : ISwaggerOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var apiDescription = context.ApiDescription;
        foreach (var responseType in apiDescription.SupportedResponseTypes)
        {
            // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/b7cf75e7905050305b115dd96640ddd6e74c7ac9/src/Swashbuckle.AspNetCore.SwaggerGen/SwaggerGenerator/SwaggerGenerator.cs#L383-L387
            var responseKey = responseType.IsDefaultResponse ? "default" : responseType.StatusCode.ToString();
            var response = operation.Responses[responseKey];

            // remove media types not supported by the API
            var content = response.Content;
            var formats = responseType.ApiResponseFormats;
            var notSupportedTypes = content.Keys.Where(t => formats.All(x => x.MediaType != t));
            foreach (var contentType in notSupportedTypes)
            {
                content.Remove(contentType);
            }
        }
    }
}
