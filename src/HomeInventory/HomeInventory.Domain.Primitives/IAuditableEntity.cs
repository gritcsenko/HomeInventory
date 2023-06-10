namespace HomeInventory.Domain.Primitives;

public interface IAuditableEntity
{
    DateTimeOffset CreatedOn { get; set; }
    DateTimeOffset ModifiedOn { get; set; }
}
