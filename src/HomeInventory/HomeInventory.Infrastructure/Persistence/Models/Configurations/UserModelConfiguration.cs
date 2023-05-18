﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeInventory.Infrastructure.Persistence.Models.Configurations;

internal class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
{
    private readonly IIdFactory<UserId, Guid> _idFactory;

    public UserModelConfiguration(IIdFactory<UserId, Guid> idFactory)
    {
        _idFactory = idFactory;
    }

    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasIdConversion(_idFactory);

        builder.Property(x => x.Email)
            .IsRequired();

        builder.Property(x => x.Password)
            .IsRequired();
    }
}