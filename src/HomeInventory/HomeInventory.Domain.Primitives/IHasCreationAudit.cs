namespace HomeInventory.Domain.Primitives;

public interface IHasCreationAudit
{
    DateTimeOffset CreatedOn { get; }
}
