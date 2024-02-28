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

    internal OutboxMessageConfiguration CreateConfiguration()
    {
        var settings = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            TypeInfoResolver = _typeResolver,
        };

        var configuration = new OutboxMessageConfiguration(settings);
        return configuration;
    }
}
