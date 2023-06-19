using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Persistence.Models;

public record OutboxMessage(Ulid Id, DateTimeOffset OccurredOn, IDomainEvent Content) : IPersistentModel, ICreationAuditableModel
{
    public DateTimeOffset CreatedOn { get; set; }
};
