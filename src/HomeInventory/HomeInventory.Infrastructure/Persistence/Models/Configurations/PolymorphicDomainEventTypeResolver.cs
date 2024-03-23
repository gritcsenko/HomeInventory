using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DotNext.Collections.Generic;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class PolymorphicDomainEventTypeResolver(IEnumerable<IJsonDerivedTypeInfo> eventTypeInfoProviders) : DefaultJsonTypeInfoResolver
{
    private readonly IEnumerable<IJsonDerivedTypeInfo> _eventTypeInfoProviders = eventTypeInfoProviders;

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        var jsonTypeInfo = base.GetTypeInfo(type, options);
        if (jsonTypeInfo.Type == typeof(IDomainEvent))
        {
            jsonTypeInfo.PolymorphismOptions = CreateOptions(_eventTypeInfoProviders);
        }

        return jsonTypeInfo;
    }

    private static JsonPolymorphismOptions CreateOptions(IEnumerable<IJsonDerivedTypeInfo> eventTypeInfoProviders)
    {
        var polymorphismOptions = new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$type",
            IgnoreUnrecognizedTypeDiscriminators = true,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
        };
        polymorphismOptions.DerivedTypes.AddAll(eventTypeInfoProviders.SelectMany(p => p.DerivedTypes));
        return polymorphismOptions;
    }
}

