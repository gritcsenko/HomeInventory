using Ardalis.Specification;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

public class ByIdFilterSpecification<TModel, TId> : Specification<TModel>, ISingleResultSpecification<TModel>
    where TModel : class, IPersistentModel<TId>
    where TId : notnull, IEquatable<TId>
{
    public ByIdFilterSpecification(TId id)
    {
        Query.Where(x => x.Id.Equals(id));
    }
}
