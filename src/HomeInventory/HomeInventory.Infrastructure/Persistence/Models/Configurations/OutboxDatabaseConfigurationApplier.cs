using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class OutboxDatabaseConfigurationApplier : IDatabaseConfigurationApplier
{
    private readonly PolymorphicDomainEventTypeResolver _typeResolver;

    public OutboxDatabaseConfigurationApplier(PolymorphicDomainEventTypeResolver typeResolver)
    {
        _typeResolver = typeResolver;
    }

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
