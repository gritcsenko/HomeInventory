using HomeInventory.Domain.Entities;
using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public interface IExpressionSpecification<TEntity, TResult>
    where TEntity : notnull, IEntity<TEntity>
{
    public Expression<Func<TEntity, TResult>> SpecificationExpression { get; }
}
