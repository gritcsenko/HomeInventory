using Ardalis.Specification;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class ByIdFilterSpecification<TModel> : Specification<TModel>, ISingleResultSpecification<TModel>
    where TModel : class, IPersistentModel
{
    public ByIdFilterSpecification(Guid id)
    {
        Query.Where(x => x.Id == id);
    }
}
