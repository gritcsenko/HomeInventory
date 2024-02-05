using System.Net;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;

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

    public ErrorMapping Build() => new(_default, _mapping);

    public static ErrorMappingBuilder CreateDefault() =>
        new ErrorMappingBuilder()
        .SetDefault(HttpStatusCode.InternalServerError)
        .Add<ConflictError>(HttpStatusCode.Conflict)
        .Add<ValidationError>(HttpStatusCode.BadRequest)
        .Add<NotFoundError>(HttpStatusCode.NotFound)
        .Add<InvalidCredentialsError>(HttpStatusCode.Forbidden);
}
