namespace HomeInventory.Domain.Primitives;

public interface IAuditableEntity : IEntity
{
    DateTimeOffset CreatedOn { get; }
    DateTimeOffset ModifiedOn { get; }
}
