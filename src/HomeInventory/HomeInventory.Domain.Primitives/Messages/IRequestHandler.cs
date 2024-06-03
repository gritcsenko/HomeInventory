using HomeInventory.Domain.Primitives.Errors;

namespace HomeInventory.Domain.Primitives.Messages;

public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    Task<OneOf<TResponse, IError>> HandleAsync(IMessageHub hub, TRequest request, CancellationToken cancellationToken = default);
}
