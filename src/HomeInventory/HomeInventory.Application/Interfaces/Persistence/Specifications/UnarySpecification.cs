using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

internal class UnarySpecification<T> : FilterSpecification<T>
    where T : notnull, IEntity<T>
{
    private static readonly ParameterReplacer<T> _replacer = new();
    private readonly FilterSpecification<T> _original;
    private readonly Func<Expression, UnaryExpression> _combineFunc;

    public UnarySpecification(FilterSpecification<T> original, Func<Expression, UnaryExpression> combineFunc)
    {
        _original = original;
        _combineFunc = combineFunc;
    }

    protected override Expression<Func<T, bool>> ToExpressionCore()
    {
        var originalBody = _original.ToExpression().Body;
        var combined = _combineFunc(originalBody);
        var exprBody = _replacer.Visit(combined);
        return Expression.Lambda<Func<T, bool>>(exprBody, _replacer.Parameter);
    }
}
