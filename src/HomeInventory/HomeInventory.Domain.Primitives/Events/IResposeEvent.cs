using HomeInventory.Domain.Primitives.Errors;
using OneOf.Types;

namespace HomeInventory.Domain.Primitives.Events;

public sealed record class ResposeEvent<TRequest>(Cuid Id, DateTimeOffset CreatedOn, TRequest Request, OneOf<Success, IError> Result) : IRequestResultEvent<TRequest, Success>
    where TRequest : IRequestEvent;

public sealed record class ResposeEvent<TRequest, TResult>(Cuid Id, DateTimeOffset CreatedOn, TRequest Request, OneOf<TResult, IError> Result) : IRequestResultEvent<TRequest, TResult>
    where TRequest : IRequestEvent<TResult>;
