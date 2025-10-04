namespace HomeInventory.Domain.Primitives;

public interface IHasModificationAudit
{
    DateTimeOffset ModifiedOn { get; }
}
