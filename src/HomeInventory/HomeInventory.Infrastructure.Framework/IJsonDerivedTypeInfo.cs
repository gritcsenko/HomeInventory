using System.Text.Json.Serialization.Metadata;

namespace HomeInventory.Infrastructure.Framework;

public interface IJsonDerivedTypeInfo
{
    IEnumerable<JsonDerivedType> DerivedTypes { get; }
}
