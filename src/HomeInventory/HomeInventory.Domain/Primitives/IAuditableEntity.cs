namespace HomeInventory.Domain.Primitives;

public interface IAuditableEntity : IEntity
{
    DateTimeOffset CreatedOn { get; set; }
    DateTimeOffset ModifiedOn { get; set; }
}
