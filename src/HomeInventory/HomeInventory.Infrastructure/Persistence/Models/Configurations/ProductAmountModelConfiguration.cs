using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Visus.Cuid;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal sealed class ProductAmountModelConfiguration : IEntityTypeConfiguration<ProductAmountModel>
{
    public void Configure(EntityTypeBuilder<ProductAmountModel> builder)
    {
        builder.ToTable("Products");

        builder.Property<Cuid>("Id");
        builder.HasKey("Id");

        builder.Property(x => x.Value)
            .HasColumnName("Amount")
            .IsRequired();

        builder.Property(x => x.UnitName)
            .HasColumnName("Unit")
            .IsRequired();
    }
}
