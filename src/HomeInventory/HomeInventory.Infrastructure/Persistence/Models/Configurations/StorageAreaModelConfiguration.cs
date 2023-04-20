using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class StorageAreaModelConfiguration : IEntityTypeConfiguration<StorageAreaModel>
{
    public void Configure(EntityTypeBuilder<StorageAreaModel> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired();
    }
}
