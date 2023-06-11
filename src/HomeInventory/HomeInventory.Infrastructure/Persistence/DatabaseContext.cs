﻿using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext : DbContext, IDatabaseContext, IUnitOfWork
{
    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
    {
    }

    public required DbSet<OutboxMessage> OutboxMessages { get; init; }

    public required DbSet<UserModel> Users { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserModelConfiguration());
    }
}
