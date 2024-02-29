using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DotNext.Collections.Generic;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class PolymorphicDomainEventTypeResolver(IEnumerable<IDomainEventJsonTypeInfo> eventTypeInfoProviders) : DefaultJsonTypeInfoResolver
{
    private readonly IReadOnlyCollection<JsonDerivedType> _derivedTypes = eventTypeInfoProviders.SelectMany(p => p.DomainEventTypes).ToArray();

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        if (jsonTypeInfo.Type == typeof(IDomainEvent))
        {
            jsonTypeInfo.PolymorphismOptions = CreateOptions();
        }

        return jsonTypeInfo;
    }

    private JsonPolymorphismOptions CreateOptions()
    {
        var polymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        };
        polymorphismOptions.DerivedTypes.AddAll(_derivedTypes);
        return polymorphismOptions;
    }
}
