using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal interface ISpecificationEvaluator
{
    IQueryable<TModel> FilterBy<TModel>(IQueryable<TModel> queryable, IFilterSpecification<TModel> specification)
        where TModel : class, IPersistentModel;
}
