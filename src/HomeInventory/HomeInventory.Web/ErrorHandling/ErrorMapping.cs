using System.Net;

namespace HomeInventory.Web.ErrorHandling;

internal sealed class ErrorMapping(HttpStatusCode defaultError, HttpStatusCode defaultValidationError, IReadOnlyDictionary<Type, HttpStatusCode> errorMapping)
{
    private readonly HttpStatusCode _defaultError = defaultError;
    private readonly HttpStatusCode _defaultValidationError = defaultValidationError;
    private readonly IReadOnlyDictionary<Type, HttpStatusCode> _errorMapping = errorMapping;

    public HttpStatusCode GetDefaultError() => _defaultError;

    public HttpStatusCode GetDefaultValidationError() => _defaultValidationError;

    public HttpStatusCode GetError(Type? errorType) => GetErrorCore(errorType).IfNone(_defaultError);

    private Option<HttpStatusCode> GetErrorCore(Type? type)
    {
        while (type is not null)
        {
            if (_errorMapping.TryGetValue(type) is { IsSome: true } result)
            {
                return result;
            }

            type = type.BaseType;
        }

        return OptionNone.Default;
    }
}
