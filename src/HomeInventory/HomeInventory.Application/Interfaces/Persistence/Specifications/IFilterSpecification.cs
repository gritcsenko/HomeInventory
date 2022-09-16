using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public interface IFilterSpecification<TEntity>
    where TEntity : notnull, IEntity<TEntity>
{
    bool IsSatisfiedBy(TEntity entity);
}
