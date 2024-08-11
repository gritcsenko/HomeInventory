using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

public interface IDatabaseContext
{
    DbSet<TEntity> GetDbSet<TEntity>()
        where TEntity : class;

    Option<TEntity> FindTracked<TEntity>(Func<TEntity, bool> condition)
        where TEntity : class;
}
