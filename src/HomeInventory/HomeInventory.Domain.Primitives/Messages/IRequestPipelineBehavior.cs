using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives.Messages;

public interface IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    Task<OneOf<TResponse, IError>> OnRequest(IMessageHub hub, TRequest request, Func<Task<OneOf<TResponse, IError>>> handler, CancellationToken cancellationToken = default);
}
