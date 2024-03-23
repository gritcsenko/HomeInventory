using System.Text.Json;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class OutboxDatabaseConfigurationApplier(PolymorphicDomainEventTypeResolver typeResolver)
    : BaseDatabaseConfigurationApplier<OutboxMessageConfiguration, OutboxMessage>(() => CreateConfiguration(typeResolver))
{
    private static OutboxMessageConfiguration CreateConfiguration(PolymorphicDomainEventTypeResolver typeResolver) => new(CreateOptions(typeResolver));

    internal static JsonSerializerOptions CreateOptions(PolymorphicDomainEventTypeResolver typeResolver) =>
        new(JsonSerializerDefaults.Web)
        {
            TypeInfoResolver = typeResolver,
        };
}

