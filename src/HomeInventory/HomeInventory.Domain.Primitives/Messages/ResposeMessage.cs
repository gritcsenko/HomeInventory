using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives.Messages;

public record class ResposeMessage<TRequest, TResponse>(Ulid Id, DateTimeOffset CreatedOn, TRequest Request, OneOf<TResponse, IError> Result) : IMessage
    where TRequest : IRequestMessage<TResponse>;
