using System.Text.Json.Serialization.Metadata;

namespace HomeInventory.Infrastructure.Framework;

public interface IDomainEventJsonTypeInfo
{
    IEnumerable<JsonDerivedType> DomainEventTypes { get; }
}
