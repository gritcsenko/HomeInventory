using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public abstract class FilterSpecification<TEntity> : ExpressionSpecification<TEntity, bool>, IFilterSpecification<TEntity>
    where TEntity : notnull, IEntity<TEntity>
{
    protected FilterSpecification()
    {
    }

    public bool IsSatisfiedBy(TEntity entity) => ToCompiled().Invoke(entity);
}
