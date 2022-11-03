using System.Linq.Expressions;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal abstract class FilterSpecification<TModel> : QuerySpecification<TModel, bool>, IFilterSpecification<TModel>
    where TModel : class, IPersistentModel
{
    protected FilterSpecification(Expression<Func<TModel, bool>> expression)
        : base(expression)
    {
    }
    public IFilterSpecification<TModel> And(IFilterSpecification<TModel> right)
        => new BinarySpecification<TModel>(this, right, Expression.AndAlso);

    public IFilterSpecification<TModel> Or(IFilterSpecification<TModel> right)
        => new BinarySpecification<TModel>(this, right, Expression.OrElse);

    public IFilterSpecification<TModel> Not()
        => new UnarySpecification<TModel>(this, Expression.Not);
}
