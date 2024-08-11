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
#pragma warning disable CA1508 // Avoid dead conditional code
        if (value is null || value is DBNull || value.Equals(null))
        {
            return new OpenApiNull();
        }
#pragma warning restore CA1508 // Avoid dead conditional code

        var json = JsonSerializer.Serialize(value, type);
        return OpenApiAnyFactory.CreateFromJson(json);
    }
}
