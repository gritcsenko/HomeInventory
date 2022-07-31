using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

internal class ParameterReplacer<T> : ExpressionVisitor
{
    public ParameterExpression Parameter { get; } = Expression.Parameter(typeof(T));

    protected override Expression VisitParameter(ParameterExpression node)
        => base.VisitParameter(Parameter);
}
