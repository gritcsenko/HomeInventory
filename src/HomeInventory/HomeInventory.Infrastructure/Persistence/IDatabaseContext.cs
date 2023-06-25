using DotNext;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal interface IDatabaseContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }

    DbSet<UserModel> Users { get; }

    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;

    Optional<TEntity> FindTracked<TEntity>(Predicate<TEntity> condition)
        where TEntity : class;
}
