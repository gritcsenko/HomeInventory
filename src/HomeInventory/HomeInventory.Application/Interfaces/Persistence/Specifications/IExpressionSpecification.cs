using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public interface IExpressionSpecification<TEntity, TResult>
    where TEntity : notnull, IEntity<TEntity>
{
    Expression<Func<TEntity, TResult>> ToExpression();
}
