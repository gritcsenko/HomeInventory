namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessage : IHasCreationAudit
{
    Ulid Id { get; }
}
