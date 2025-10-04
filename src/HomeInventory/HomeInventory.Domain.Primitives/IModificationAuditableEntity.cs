namespace HomeInventory.Domain.Primitives;

public interface IModificationAuditableEntity
{
    DateTimeOffset ModifiedOn { get; set; }
}
