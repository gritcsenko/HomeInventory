using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public interface ICreateEntitySpecification<TEntity>
    where TEntity : notnull, IEntity<TEntity>
{
}
