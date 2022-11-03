using System.Linq.Expressions;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal interface IQuerySpecification<TModel, TResult>
    where TModel : class, IPersistentModel
{
    Expression<Func<TModel, TResult>> QueryExpression { get; }

    IEnumerable<Expression<Func<TModel, object>>> IncludeExpressions { get; }

    IEnumerable<IOrderByExpression<TModel>> OrderByExpressions { get; }

    bool IsSplit { get; }
}
