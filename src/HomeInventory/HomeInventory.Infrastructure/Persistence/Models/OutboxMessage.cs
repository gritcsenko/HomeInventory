using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Persistence.Models;

public record OutboxMessage(Ulid Id, DateTimeOffset OccurredOn, IDomainEvent Content) : IPersistentModel, IHasCreationAudit
{
    public required DateTimeOffset CreatedOn { get; init; }
};
