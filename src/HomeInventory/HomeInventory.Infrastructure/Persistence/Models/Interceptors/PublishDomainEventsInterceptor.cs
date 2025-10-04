using HomeInventory.Application.Framework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HomeInventory.Infrastructure.Persistence.Models.Interceptors;

internal class PublishDomainEventsInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            await PublishEventsAsync(context, cancellationToken);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private static async ValueTask PublishEventsAsync(DbContext context, CancellationToken cancellationToken)
    {
        var domainEvents = context.ChangeTracker
            .Entries<OutboxMessage>()
            .Select(static e => e.Entity.Content);

        foreach (var domainEvent in domainEvents)
        {
            var notification = DomainEventNotification.Create(domainEvent);
            _ = notification;
            await Task.Delay(TimeSpan.Zero, cancellationToken);
            // NOTE: Domain event publication is not implemented yet
        }
    }
}
