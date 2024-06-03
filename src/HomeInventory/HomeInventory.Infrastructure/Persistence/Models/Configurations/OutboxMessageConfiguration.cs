using System.Text.Json;
using HomeInventory.Domain.Primitives.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class OutboxMessageConfiguration(JsonSerializerOptions settings) : IEntityTypeConfiguration<OutboxMessage>
{
    private readonly JsonSerializerOptions _settings = settings;

    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasKey(x => x.Id); // IsClustered(false)

        builder.Property(x => x.OccurredOn)
            .IsRequired();

        builder.Property(x => x.Content)
            .IsRequired();

        builder.Property(e => e.Content).HasConversion(
            v => Serialize(v, _settings),
            json => Deserialize(json, _settings));
    }

    private static IDomainEvent Deserialize(string json, JsonSerializerOptions settings) =>
        JsonSerializer.Deserialize<IDomainEvent>(json, settings)
            ?? throw new InvalidOperationException("Not able to deserialize event");

    private static string Serialize(IDomainEvent obj, JsonSerializerOptions settings) =>
        JsonSerializer.Serialize(obj, settings);
}
