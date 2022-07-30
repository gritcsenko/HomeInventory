using System.Linq.Expressions;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public class UnarySpecification<T> : FilterSpecification<T>
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

    protected override Expression<Func<T, bool>> ToExpression()
    {
        var originalBody = _original.SpecificationExpression.Body;
        var combined = _combineFunc(originalBody);
        var exprBody = _replacer.Visit(combined);
        return Expression.Lambda<Func<T, bool>>(exprBody, _replacer.Parameter);
    }
}
