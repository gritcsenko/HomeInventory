using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using Microsoft.AspNetCore.Http;

namespace HomeInventory.Web.Infrastructure;

internal sealed class ErrorMapping
{
    private readonly IReadOnlyDictionary<Option<Type>, int> _errorMapping = new Dictionary<Option<Type>, int>
    {
        [Option.Some(typeof(ConflictError))] = StatusCodes.Status409Conflict,
        [Option.Some(typeof(ValidationError))] = StatusCodes.Status400BadRequest,
        [Option.Some(typeof(NotFoundError))] = StatusCodes.Status404NotFound,
        [Option.None<Type>()] = StatusCodes.Status500InternalServerError,
    };

    public int GetDefaultError() => _errorMapping[Option.None<Type>()];

    public int GetError(IError error) => GetError(error.GetType());

    public int GetError<T>()
        where T : IError =>
        GetError(typeof(T));

    private int GetError(Type type) =>
        _errorMapping.GetValueOrDefault(type, GetErrroForBase);

    private int GetErrroForBase(Type type) =>
        type.BaseType is { } baseType ? GetError(baseType) : GetDefaultError();
}
