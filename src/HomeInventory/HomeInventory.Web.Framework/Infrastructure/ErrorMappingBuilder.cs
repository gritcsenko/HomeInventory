using System.Net;
using HomeInventory.Core;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMappingBuilder
{
    private readonly HttpStatusCode _default;
    private readonly Dictionary<Type, HttpStatusCode> _mapping;

    private ErrorMappingBuilder(HttpStatusCode @default, IEnumerable<KeyValuePair<Type, HttpStatusCode>> mapping)
    {
        _default = @default;
        _mapping = new Dictionary<Type, HttpStatusCode>(mapping);
    }

    public ErrorMappingBuilder Add<TError>(HttpStatusCode statusCode) => new(_default, _mapping.Concat(KeyValuePair.Create(typeof(TError), statusCode)));

    public ErrorMapping Build() => new(_default, _mapping);

    public static ErrorMappingBuilder CreateDefault() =>
        new ErrorMappingBuilder(HttpStatusCode.InternalServerError, [])
            .Add<ConflictError>(HttpStatusCode.Conflict)
            .Add<ValidationError>(HttpStatusCode.BadRequest)
            .Add<NotFoundError>(HttpStatusCode.NotFound)
            .Add<InvalidCredentialsError>(HttpStatusCode.Forbidden);
}
