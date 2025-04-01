using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class PolymorphicDomainEventTypeResolver(IEnumerable<IJsonDerivedTypeInfo> eventTypeInfoProviders) : DefaultJsonTypeInfoResolver
{
    private readonly IReadOnlyCollection<JsonDerivedType> _derivedTypes = [.. eventTypeInfoProviders.SelectMany(static p => p.DerivedTypes)];

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        if (jsonTypeInfo.Type == typeof(IDomainEvent))
        {
            jsonTypeInfo.PolymorphismOptions = CreateOptions(_derivedTypes);
        }

        return jsonTypeInfo;
    }

    private static JsonPolymorphismOptions CreateOptions(IEnumerable<JsonDerivedType> derivedTypes)
    {
        var polymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        };
        polymorphismOptions.DerivedTypes.AddRange(derivedTypes);
        return polymorphismOptions;
    }
}
