namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessage : IHasCreationAudit
{
    Cuid Id { get; }
}
