using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMapping
{
    private readonly int _defaultError = StatusCodes.Status500InternalServerError;
    private readonly IReadOnlyDictionary<Type, int> _errorMapping = new Dictionary<Type, int>
    {
        [typeof(ConflictError)] = StatusCodes.Status409Conflict,
        [typeof(ValidationError)] = StatusCodes.Status400BadRequest,
        [typeof(NotFoundError)] = StatusCodes.Status404NotFound,
        [typeof(InvalidCredentialsError)] = StatusCodes.Status403Forbidden,
    };

    public ErrorMapping()
    {
    }

    public int GetDefaultError() => _defaultError;

    public int GetError(IError error) => GetErrorCore(error.GetType());

    public int GetError<T>()
        where T : IError =>
        GetErrorCore(typeof(T));

    private int GetErrorCore(Type type) =>
        _errorMapping
            .GetValueOptional(type)
            .OrInvoke(() =>
                type switch
                {
                    { BaseType: not null } t => GetErrorCore(t.BaseType),
                    _ => _defaultError,
                });
}
