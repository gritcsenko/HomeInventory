using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using DotNext.Collections.Generic;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Framework;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class PolymorphicDomainEventTypeResolver(IEnumerable<IDomainEventJsonTypeInfo> eventTypeInfoProviders) : DefaultJsonTypeInfoResolver
{
    private readonly IEnumerable<IDomainEventJsonTypeInfo> _eventTypeInfoProviders = eventTypeInfoProviders;

    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type baseType = typeof(IDomainEvent);
        if (jsonTypeInfo.Type == baseType)
        {
            var polymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
            };
            polymorphismOptions.DerivedTypes.AddAll(_eventTypeInfoProviders.SelectMany(p => p.DomainEventTypes));
            jsonTypeInfo.PolymorphismOptions = polymorphismOptions;
        }

        return jsonTypeInfo;
    }
}

