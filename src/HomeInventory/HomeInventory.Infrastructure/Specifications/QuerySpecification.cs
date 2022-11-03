using System.Linq.Expressions;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal abstract class QuerySpecification<TModel, TResult> : IQuerySpecification<TModel, TResult>
    where TModel : class, IPersistentModel
{
    private readonly List<Expression<Func<TModel, object>>> _includeExpressions = new();
    private readonly List<OrderByExpression> _orderByExpressions = new();
    private readonly bool _isSplit;

    protected QuerySpecification(Expression<Func<TModel, TResult>> expression, bool isSplit = false)
    {
        QueryExpression = expression;
        _isSplit = isSplit;
    }

    public Expression<Func<TModel, TResult>> QueryExpression { get; }

    public IEnumerable<Expression<Func<TModel, object>>> IncludeExpressions => _includeExpressions;

    public IEnumerable<IOrderByExpression<TModel>> OrderByExpressions => _orderByExpressions;

    public bool IsSplit => _isSplit && _includeExpressions.Any();

    protected void Include(Expression<Func<TModel, object>> expression) =>
        _includeExpressions.Add(expression);

    protected void OrderBy(Expression<Func<TModel, object>> expression, bool isDescending = false) =>
        _orderByExpressions.Add(new OrderByExpression(expression, isDescending));

    private record OrderByExpression(Expression<Func<TModel, object>> Expression, bool IsDescending) : IOrderByExpression<TModel>;
}
