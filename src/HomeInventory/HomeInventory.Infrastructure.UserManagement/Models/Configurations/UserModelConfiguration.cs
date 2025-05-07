using HomeInventory.Infrastructure.Framework.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.UserManagement.Models.Configurations;

internal sealed class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasKey(static x => x.Id);
        builder.Property(static x => x.Id)
            .HasIdConversion();

        builder.Property(static x => x.Email)
            .IsRequired();

        builder.Property(static x => x.Password)
            .IsRequired();
    }
}
