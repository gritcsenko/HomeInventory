using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMapping
{
    private readonly IReadOnlyDictionary<Optional<Type>, int> _errorMapping = new Dictionary<Optional<Type>, int>
    {
        [Optional.Some(typeof(ConflictError))] = StatusCodes.Status409Conflict,
        [Optional.Some(typeof(ValidationError))] = StatusCodes.Status400BadRequest,
        [Optional.Some(typeof(NotFoundError))] = StatusCodes.Status404NotFound,
        [Optional.None<Type>()] = StatusCodes.Status500InternalServerError,
    };

    public int GetDefaultError() => _errorMapping[Optional.None<Type>()];

    public int GetError(IError error) => GetError(error.GetType());

    public int GetError<T>()
        where T : IError =>
        GetError(typeof(T));

    private int GetError(Type type) =>
        _errorMapping.GetValueOrDefault(type, GetErrroForBase);

    private int GetErrroForBase(Type type) =>
        type.BaseType is { } baseType ? GetError(baseType) : GetDefaultError();
}
