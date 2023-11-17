using System.Text.Json.Serialization.Metadata;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.UserManagement.Models.Configurations;

internal class DomainEventJsonTypeInfo(params Type[] types) : IDomainEventJsonTypeInfo
{
    public IEnumerable<JsonDerivedType> DomainEventTypes => types.Select(t => new JsonDerivedType(t, t.FullName ?? t.Name));
}
