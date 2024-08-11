namespace HomeInventory.Domain.Primitives.Messages;

public record class ResposeMessage<TRequest, TResponse>(Ulid Id, DateTimeOffset CreatedOn, TRequest Request, TResponse Result) : IMessage
    where TRequest : IRequestMessage<TResponse>;
