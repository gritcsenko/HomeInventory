using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
    public UserModelConfiguration()
    {
    }

    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasIdConversion();

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.Password)
            .IsRequired();
    }
}
