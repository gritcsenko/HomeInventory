using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Messages;

namespace HomeInventory.Infrastructure.Persistence.Models;

public record OutboxMessage(Ulid Id, DateTimeOffset OccurredOn, IDomainEvent Content) : IPersistentModel, IHasCreationAudit
{
    public required DateTimeOffset CreatedOn { get; init; }
};
