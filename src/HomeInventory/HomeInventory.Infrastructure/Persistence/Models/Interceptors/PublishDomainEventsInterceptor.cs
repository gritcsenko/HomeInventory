using HomeInventory.Application.Framework;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HomeInventory.Infrastructure.Persistence.Models.Interceptors;

internal class PublishDomainEventsInterceptor(IPublisher publisher) : SaveChangesInterceptor
{
    private readonly IPublisher _publisher = publisher;

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            await PublishEventsAsync(context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private async ValueTask PublishEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var domainEvents = context.ChangeTracker
            .Entries<OutboxMessage>()
            .Select(static e => e.Entity.Content);

        foreach (var domainEvent in domainEvents)
        {
            object notification = DomainEventNotification.Create(domainEvent);
            await _publisher.Publish(notification, cancellationToken);
        }
    }
}
