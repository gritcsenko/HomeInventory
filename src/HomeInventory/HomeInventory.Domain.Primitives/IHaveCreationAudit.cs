namespace HomeInventory.Domain.Primitives;

public interface IHaveCreationAudit
{
    DateTimeOffset Created { get; }
}
