using HomeInventory.Infrastructure.Persistence.Models;

namespace HomeInventory.Infrastructure.Specifications;

internal class ByIdFilterSpecification<TModel> : FilterSpecification<TModel>
    where TModel : class, IPersistentModel
{
    public ByIdFilterSpecification(Guid id)
        : base(x => x.Id == id)
    {
    }
}
