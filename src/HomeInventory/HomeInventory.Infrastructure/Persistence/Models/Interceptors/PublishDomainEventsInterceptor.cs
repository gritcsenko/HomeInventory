using HomeInventory.Application.Cqrs.DomainEvents;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reactive.Linq;

namespace HomeInventory.Infrastructure.Persistence.Models.Interceptors;

internal class PublishDomainEventsInterceptor(IMessageHub hub) : SaveChangesInterceptor
{
    private readonly IMessageHub _hub = hub;

    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is { } context)
        {
            PublishEvents(context);
        }

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void PublishEvents(DbContext context) => 
        context.ChangeTracker
            .Entries<OutboxMessage>()
            .Select(e => e.Entity.Content)
            .Select(e => e.CreateDomainNotification())
            .Iter(e => _hub.OnNext(e));
}
