using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace HomeInventory.Infrastructure.Persistence.Mapping;

internal static class ExpressionMappingExtensions
{
    public static IExpressionMapping<TResult> Map<TSource, TResult>(this Expression<Func<TSource, TResult>> sourceExpression)
    {
        return new ExpressionMapping<TSource, TResult>(sourceExpression);
    }

    public interface IExpressionMapping<TResult>
    {
        Expression<Func<TTarget, TResult>> To<TTarget>();
    }

    private class ExpressionMapping<TSource, TResult> : IExpressionMapping<TResult>
    {
        private readonly Expression<Func<TSource, TResult>> _sourceExpression;

        public ExpressionMapping(Expression<Func<TSource, TResult>> sourceExpression)
        {
            _sourceExpression = sourceExpression;
        }

        public Expression<Func<TTarget, TResult>> To<TTarget>()
        {
            var modifier = new ExpressionModifier<TSource, TTarget, TResult>();

            var result = modifier.Visit(_sourceExpression);
            return (Expression<Func<TTarget, TResult>>)result;
        }
    }

    private class ExpressionModifier<TSource, TTarget, TResult> : ExpressionVisitor
    {
        private readonly Dictionary<string, ParameterExpression> _parameters = new();

        [return: NotNullIfNotNull("node")]
        public override Expression? Visit(Expression? node)
        {
            _parameters.Clear();
            return base.Visit(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (typeof(T) == typeof(Func<TSource, TResult>))
            {
                var body = Visit(node.Body);
                return Expression.Lambda<Func<TTarget, TResult>>(body, _parameters.Values);
            }

            return base.VisitLambda(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Member.DeclaringType == typeof(TSource))
            {
                if (node.Member.MemberType == MemberTypes.Property)
                {
                    return Expression.Property(Visit(node.Expression), typeof(TTarget), node.Member.Name);
                }
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == typeof(TSource))
            {
                var parameter = Expression.Parameter(typeof(TTarget), node.Name);
                _parameters[node.Name!] = parameter;
                return parameter;
            }

            return base.VisitParameter(node);
        }
    }
}