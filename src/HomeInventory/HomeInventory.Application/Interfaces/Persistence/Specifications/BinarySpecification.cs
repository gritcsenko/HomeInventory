using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

internal class BinarySpecification<T> : FilterSpecification<T>
    where T : notnull, IEntity<T>
{
    private static readonly ParameterReplacer<T> _replacer = new();
    private readonly FilterSpecification<T> _left;
    private readonly FilterSpecification<T> _right;
    private readonly Func<Expression, Expression, BinaryExpression> _combineFunc;

    public BinarySpecification(FilterSpecification<T> left, FilterSpecification<T> right, Func<Expression, Expression, BinaryExpression> combineFunc)
    {
        _left = left;
        _right = right;
        _combineFunc = combineFunc;
    }

    protected override Expression<Func<T, bool>> ToExpressionCore()
    {
        var leftBody = _left.ToExpression().Body;
        var rightBody = _right.ToExpression().Body;
        var combined = _combineFunc(leftBody, rightBody);
        var exprBody = _replacer.Visit(combined);
        return Expression.Lambda<Func<T, bool>>(exprBody, _replacer.Parameter);
    }
}
