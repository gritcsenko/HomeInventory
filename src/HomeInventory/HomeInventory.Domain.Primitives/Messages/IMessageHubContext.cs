using HomeInventory.Domain.Primitives.Ids;

namespace HomeInventory.Domain.Primitives.Messages;

public interface IMessageHubContext
{
    IIdSupplier<Ulid> EventIdSupplier { get; }

    TimeProvider EventCreatedTimeProvider { get; }
}
