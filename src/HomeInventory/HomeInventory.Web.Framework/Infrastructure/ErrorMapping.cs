using System.Net;
using DotNext;
using HomeInventory.Core;
using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMapping(HttpStatusCode defaultError, IReadOnlyDictionary<Type, HttpStatusCode> errorMapping)
{
    private readonly HttpStatusCode _defaultError = defaultError;
    private readonly IReadOnlyDictionary<Type, HttpStatusCode> _errorMapping = errorMapping;

    public HttpStatusCode GetDefaultError() => _defaultError;

    public HttpStatusCode GetError(IError error) => GetErrorCore(error.GetType()).Or(_defaultError);

    private Optional<HttpStatusCode> GetErrorCore(Type? type)
    {
        while (type is not null)
        {
            if (_errorMapping.GetValueOptional(type) is { HasValue: true } result)
            {
                return result;
            }

            type = type.BaseType;
        }

        return Optional<HttpStatusCode>.None;
    }
}
