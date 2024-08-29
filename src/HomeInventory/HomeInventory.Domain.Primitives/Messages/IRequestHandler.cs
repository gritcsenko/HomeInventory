namespace HomeInventory.Domain.Primitives.Messages;

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    Task<TResponse> HandleAsync(IRequestContext<TRequest> context);
}
