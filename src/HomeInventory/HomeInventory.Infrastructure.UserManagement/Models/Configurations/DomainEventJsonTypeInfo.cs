using System.Text.Json.Serialization.Metadata;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.UserManagement.Models.Configurations;

internal sealed class DomainEventJsonTypeInfo(params Type[] types) : IJsonDerivedTypeInfo
{
    private readonly Type[] _types = types;

    public IEnumerable<JsonDerivedType> DerivedTypes => _types.Select(static t => new JsonDerivedType(t, t.FullName ?? t.Name));
}
