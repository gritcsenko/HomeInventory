using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public interface ICreateEntitySpecification<TEntity>
    where TEntity : notnull, IEntity<TEntity>
{
}
