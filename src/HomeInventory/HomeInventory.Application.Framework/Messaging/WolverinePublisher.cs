using Wolverine;

namespace HomeInventory.Application.Framework.Messaging;

internal sealed class WolverinePublisher(IMessageBus messageBus) : IPublisher
{
    private readonly IMessageBus _messageBus = messageBus;

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) 
        where TNotification : INotification
    {
        await _messageBus.PublishAsync(notification);
    }
}
