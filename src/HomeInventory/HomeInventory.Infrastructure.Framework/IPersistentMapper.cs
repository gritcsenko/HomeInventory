using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Infrastructure.Framework.Models;

namespace HomeInventory.Infrastructure.Framework;

public interface IPersistentMapper<TPersistent, TAggregateRoot, TIdentifier>
    where TPersistent : class, IPersistentModel<TIdentifier>
    where TAggregateRoot : AggregateRoot<TAggregateRoot, TIdentifier>
    where TIdentifier : IIdentifierObject<TIdentifier>
{
    TPersistent ToPersistent(TAggregateRoot entity);
    TPersistent ToPersistent(TAggregateRoot entity, TPersistent existing);
    IQueryable<TAggregateRoot> FromPersistent(IQueryable<TPersistent> queryable);
}

