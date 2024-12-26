using System.Text.Json;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HomeInventory.Web.OpenApi;

/// <summary>
/// REF: https://github.com/dotnet/aspnet-api-versioning/issues/429#issuecomment-605402330
/// </summary>
public sealed class JsonOpenApiValueConverter : IOpenApiValueConverter
{
    public IOpenApiAny Convert(object? value, Type type)
    {
        if (value is null or DBNull)
        {
            return new OpenApiNull();
        }

        var json = JsonSerializer.Serialize(value, type);
        return OpenApiAnyFactory.CreateFromJson(json);
    }
}
