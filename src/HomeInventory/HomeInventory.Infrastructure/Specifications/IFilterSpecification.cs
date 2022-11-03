using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal interface IFilterSpecification<TModel> : IQuerySpecification<TModel, bool>
    where TModel : class, IPersistentModel
{
    IFilterSpecification<TModel> And(IFilterSpecification<TModel> right);

    IFilterSpecification<TModel> Or(IFilterSpecification<TModel> right);

    IFilterSpecification<TModel> Not();
}
