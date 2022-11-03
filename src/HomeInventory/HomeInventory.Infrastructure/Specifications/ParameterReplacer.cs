using System.Linq.Expressions;

namespace HomeInventory.Infrastructure.Specifications;

internal class ParameterReplacer<T> : ExpressionVisitor
{
    public static ParameterReplacer<T> Instance { get; } = new();

    private ParameterExpression _parameter = Expression.Parameter(typeof(T));

    protected override Expression VisitParameter(ParameterExpression node)
        => base.VisitParameter(_parameter);

    public Expression<Func<T, TResult>> CreateLambda<TResult>(Expression expression) =>
        Expression.Lambda<Func<T, TResult>>(Visit(expression), _parameter);
}
