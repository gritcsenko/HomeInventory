using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class ProductAmountModelConfiguration : IEntityTypeConfiguration<ProductAmountModel>
{
    public void Configure(EntityTypeBuilder<ProductAmountModel> builder)
    {
        builder.ToTable("Products");

        builder.Property<Ulid>("Id");
        builder.HasKey("Id");

        builder.Property(x => x.Value)
            .HasColumnName("Amount")
            .IsRequired();

        builder.Property(x => x.UnitName)
            .HasColumnName("Unit")
            .IsRequired();
    }
}
