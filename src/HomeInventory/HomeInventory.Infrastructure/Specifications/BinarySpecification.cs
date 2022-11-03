using System.Linq.Expressions;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class BinarySpecification<TModel> : FilterSpecification<TModel>
    where TModel : class, IPersistentModel
{
    public BinarySpecification(IFilterSpecification<TModel> left, IFilterSpecification<TModel> right, Func<Expression, Expression, BinaryExpression> combineFunc)
        : base(ParameterReplacer<TModel>.Instance.CreateLambda<bool>(combineFunc(left.QueryExpression.Body, right.QueryExpression.Body)))
    {
        Left = left;
        Right = right;
    }

    public IFilterSpecification<TModel> Left { get; }

    public IFilterSpecification<TModel> Right { get; }
}
