using Ardalis.Specification;
using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class ByIdFilterSpecification<TModel> : ByIdFilterSpecification<TModel, Guid>
    where TModel : class, IPersistentModel
{
    public ByIdFilterSpecification(Guid id)
        : base(id)
    {
    }
}

internal class ByIdFilterSpecification<TModel, TId> : Specification<TModel>, ISingleResultSpecification<TModel>
    where TModel : class, IPersistentModel<TId>
    where TId : notnull, IEquatable<TId>
{
    public ByIdFilterSpecification(TId id)
    {
        Query.Where(x => x.Id.Equals(id));
    }
}
