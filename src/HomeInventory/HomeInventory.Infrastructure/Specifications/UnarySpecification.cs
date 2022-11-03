using System.Linq.Expressions;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class UnarySpecification<TModel> : FilterSpecification<TModel>
    where TModel : class, IPersistentModel
{
    public UnarySpecification(IFilterSpecification<TModel> left, Func<Expression, UnaryExpression> combineFunc)
        : base(ParameterReplacer<TModel>.Instance.CreateLambda<bool>(combineFunc(left.QueryExpression.Body)))
    {
        Left = left;
    }

    public IFilterSpecification<TModel> Left { get; }
}
