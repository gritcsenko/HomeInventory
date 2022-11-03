using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal class SpecificationEvaluator : ISpecificationEvaluator
{
    public IQueryable<TModel> FilterBy<TModel>(IQueryable<TModel> queryable, IFilterSpecification<TModel> specification)
        where TModel : class, IPersistentModel =>
        queryable.Where(specification.QueryExpression)
            .AggregateFrom(specification.OrderByExpressions, (q, e) => e.IsDescending ? q.OrderByDescending(e.Expression) : q.OrderBy(e.Expression))
            .AggregateFrom(specification.IncludeExpressions, (q, e) => q.Include(e))
            .SplitIf(specification.IsSplit)
        ;
}
