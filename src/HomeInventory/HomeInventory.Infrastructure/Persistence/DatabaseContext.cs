using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Persistence.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal class DatabaseContext : DbContext
{
    private readonly IIdFactory<UserId, Guid> _userIdFactory;

    public DatabaseContext(DbContextOptions<DatabaseContext> options, IIdFactory<UserId, Guid> userIdFactory)
        : base(options)
    {
        _userIdFactory = userIdFactory;
    }

    public required DbSet<OutboxMessage> OutboxMessages { get; init; }

    public required DbSet<UserModel> Users { get; init; }

    public required DbSet<StorageAreaModel> StorageAreas { get; init; }

    public required DbSet<ProductModel> Products { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new UserModelConfiguration(_userIdFactory));
        modelBuilder.ApplyConfiguration(new StorageAreaModelConfiguration());
        modelBuilder.ApplyConfiguration(new ProductModelConfiguration());
        modelBuilder.ApplyConfiguration(new ProductAmountModelConfiguration());
    }
}
