using HomeInventory.Domain.Entities;
using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public class BinarySpecification<T> : FilterSpecification<T>
    where T : notnull, IEntity<T>
{
    private static readonly ParameterReplacer _replacer = new();
    private readonly FilterSpecification<T> _left;
    private readonly FilterSpecification<T> _right;
    private readonly Func<Expression, Expression, BinaryExpression> _combineFunc;

    public BinarySpecification(FilterSpecification<T> left, FilterSpecification<T> right, Func<Expression, Expression, BinaryExpression> combineFunc)
    {
        _left = left;
        _right = right;
        _combineFunc = combineFunc;
    }

    protected override Expression<Func<T, bool>> ToExpression()
    {
        var leftBody = _left.SpecificationExpression.Body;
        var rightBody = _right.SpecificationExpression.Body;
        var combined = _combineFunc(leftBody, rightBody);
        var exprBody = _replacer.Visit(combined);
        return Expression.Lambda<Func<T, bool>>(exprBody, _replacer.Parameter);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        public ParameterExpression Parameter { get; } = Expression.Parameter(typeof(T));

        protected override Expression VisitParameter(ParameterExpression node)
            => base.VisitParameter(Parameter);
    }
}
