using System.Text.Json.Serialization.Metadata;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.UserManagement.Models.Configurations;

internal class DomainEventJsonTypeInfo(params Type[] types) : IDomainEventJsonTypeInfo
{
    private readonly Type[] _types = types;

    public IEnumerable<JsonDerivedType> DomainEventTypes => _types.Select(t => new JsonDerivedType(t, t.FullName ?? t.Name));
}
