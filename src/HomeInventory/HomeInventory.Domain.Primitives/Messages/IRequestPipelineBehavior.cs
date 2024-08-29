namespace HomeInventory.Domain.Primitives.Messages;

public interface IRequestPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequestMessage<TResponse>
{
    Task<TResponse> OnRequestAsync(IRequestContext<TRequest> context, Func<IRequestContext<TRequest>, Task<TResponse>> handler);
}
