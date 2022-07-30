using System.Linq.Expressions;
using FastExpressionCompiler;
using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public abstract class ExpressionSpecification<TEntity, TResult> : IExpressionSpecification<TEntity, TResult>
    where TEntity : notnull, IEntity<TEntity>
{
    private readonly Lazy<Expression<Func<TEntity, TResult>>> _expression;
    private readonly Lazy<Func<TEntity, TResult>> _compiled;

    protected ExpressionSpecification()
    {
        _expression = new Lazy<Expression<Func<TEntity, TResult>>>(ToExpression);
        _compiled = new Lazy<Func<TEntity, TResult>>(() => SpecificationExpression.CompileFast());
    }

    public Expression<Func<TEntity, TResult>> SpecificationExpression => _expression.Value;

    protected Func<TEntity, TResult> CompiledExpression => _compiled.Value;

    protected abstract Expression<Func<TEntity, TResult>> ToExpression();
}

public abstract class FilterSpecification<TEntity> : ExpressionSpecification<TEntity, bool>, IFilterSpecification<TEntity>
    where TEntity : notnull, IEntity<TEntity>
{
    protected FilterSpecification()
    {
    }

    public bool IsSatisfiedBy(TEntity entity) => CompiledExpression(entity);
}
