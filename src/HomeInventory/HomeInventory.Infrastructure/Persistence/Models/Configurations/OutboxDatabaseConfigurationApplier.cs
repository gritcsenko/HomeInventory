using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class OutboxDatabaseConfigurationApplier(PolymorphicDomainEventTypeResolver typeResolver) : IDatabaseConfigurationApplier
{
    private readonly PolymorphicDomainEventTypeResolver _typeResolver = typeResolver;

    public void ApplyConfigurationTo(ModelBuilder modelBuilder)
    {
        var configuration = CreateConfiguration();
        modelBuilder.ApplyConfiguration(configuration);
    }

    internal OutboxMessageConfiguration CreateConfiguration() => new(CreateOptions(_typeResolver));

    internal static JsonSerializerOptions CreateOptions(PolymorphicDomainEventTypeResolver typeResolver) =>
        new(JsonSerializerDefaults.Web)
        {
            TypeInfoResolver = typeResolver,
        };
}
