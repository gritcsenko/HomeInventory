using System.Linq.Expressions;
using FastExpressionCompiler;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public abstract class ExpressionSpecification<TEntity, TResult> : IExpressionSpecification<TEntity, TResult>
    where TEntity : notnull, IEntity<TEntity>
{
    private readonly Lazy<Expression<Func<TEntity, TResult>>> _expression;
    private readonly Lazy<Func<TEntity, TResult>> _compiled;

    protected ExpressionSpecification()
    {
        _expression = new Lazy<Expression<Func<TEntity, TResult>>>(ToExpressionCore);
        _compiled = new Lazy<Func<TEntity, TResult>>(() => ToExpression().CompileFast());
    }

    public Expression<Func<TEntity, TResult>> ToExpression() => _expression.Value;

    protected Func<TEntity, TResult> ToCompiled() => _compiled.Value;

    protected abstract Expression<Func<TEntity, TResult>> ToExpressionCore();
}
