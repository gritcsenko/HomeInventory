using Microsoft.OpenApi.Any;

namespace HomeInventory.Web.OpenApi;

public interface IOpenApiValueConverter
{
    IOpenApiAny Convert(object? value, Type type);
}
