using Wolverine;

namespace HomeInventory.Application.Framework.Messaging;

internal sealed class WolverineSender(IMessageBus messageBus) : ISender
{
    private readonly IMessageBus _messageBus = messageBus;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        return await _messageBus.InvokeAsync<TResponse>(request, cancellationToken);
    }
}
