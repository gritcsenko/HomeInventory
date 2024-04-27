namespace HomeInventory.Domain.Primitives;

public interface ICreationAuditableEntity
{
    DateTimeOffset CreatedOn { get; set; }
}
