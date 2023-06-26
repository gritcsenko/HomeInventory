using System.Text.Json.Serialization.Metadata;
using HomeInventory.Domain.Events;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.UserManagement.Models.Configurations;

internal class DomainEventJsonTypeInfo : IDomainEventJsonTypeInfo
{
    public IEnumerable<JsonDerivedType> DomainEventTypes
    {
        get
        {
            yield return new JsonDerivedType(typeof(UserCreatedDomainEvent), nameof(UserCreatedDomainEvent));
        }
    }
}
