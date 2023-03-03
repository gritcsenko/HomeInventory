using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using HomeInventory.Domain.Events;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

public class PolymorphicDomainEventTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type baseType = typeof(IDomainEvent);
        if (jsonTypeInfo.Type == baseType)
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(ProductAddedEvent), nameof(ProductAddedEvent)),
                    new JsonDerivedType(typeof(ProductRemovedEvent), nameof(ProductRemovedEvent))
                }
            };
        }

        return jsonTypeInfo;
    }
}
