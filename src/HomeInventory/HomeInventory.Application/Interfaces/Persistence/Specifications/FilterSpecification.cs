using FastExpressionCompiler;
using HomeInventory.Domain.Entities;
using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;
public abstract class FilterSpecification<TEntity> : IFilterSpecification<TEntity>, IExpressionSpecification<TEntity, bool>
    where TEntity : notnull, IEntity<TEntity>
{
    private readonly Lazy<Expression<Func<TEntity, bool>>> _expression;
    private readonly Lazy<Func<TEntity, bool>> _compiled;

    protected FilterSpecification()
    {
        _expression = new Lazy<Expression<Func<TEntity, bool>>>(ToExpression);
        _compiled = new Lazy<Func<TEntity, bool>>(() => SpecificationExpression.CompileFast());
    }

    public Expression<Func<TEntity, bool>> SpecificationExpression => _expression.Value;

    public bool IsSatisfiedBy(TEntity entity) => _compiled.Value(entity);

    protected abstract Expression<Func<TEntity, bool>> ToExpression();
}
