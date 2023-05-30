using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal interface IDatabaseContext
{
    DbSet<UserModel> Users { get; }

    DbSet<TEntity> Set<TEntity>()
        where TEntity : class;
}
