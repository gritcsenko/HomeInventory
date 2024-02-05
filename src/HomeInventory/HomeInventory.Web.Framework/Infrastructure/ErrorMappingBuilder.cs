using System.Net;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMappingBuilder
{
    private readonly Dictionary<Type, HttpStatusCode> _mapping = new();
    private HttpStatusCode _default = HttpStatusCode.InternalServerError;

    public ErrorMappingBuilder()
    {
    }

    public ErrorMappingBuilder Add<TError>(HttpStatusCode statusCode)
    {
        _mapping.Add(typeof(TError), statusCode);
        return this;
    }

    public ErrorMappingBuilder SetDefault(HttpStatusCode statusCode)
    {
        _default = statusCode;
        return this;
    }

    public ErrorMapping Build() => new ErrorMapping(_default, _mapping);
}
