using System.Text.Json;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        var settings = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            TypeInfoResolver = new PolymorphicDomainEventTypeResolver(),
        };

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(e => e.Content).HasConversion(
            v => Serialize(v, settings),
            json => Deserialize(json, settings));
    }

    private static IDomainEvent Deserialize(string json, JsonSerializerOptions settings) =>
        JsonSerializer.Deserialize<IDomainEvent>(json, settings)
            ?? throw new InvalidOperationException("Not able to deserialize event");

    private static string Serialize(IDomainEvent obj, JsonSerializerOptions settings) =>
        JsonSerializer.Serialize(obj, settings);
}

