using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Framework;

public interface IDatabaseContext
{
    DbSet<TEntity> GetDbSet<TEntity>()
        where TEntity : class;

    Option<TEntity> FindTracked<TEntity>(Func<TEntity, bool> condition)
        where TEntity : class;
}
