using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Persistence;

public interface IRepository<TEntity, TIdentity>
    where TIdentity : notnull, IIdentityValue
    where TEntity : IEntity<TEntity, TIdentity>
{
    Task<OneOf<TEntity, NotFound>> FindByIdAsync(TIdentity id, CancellationToken cancellationToken = default);

    Task<OneOf<Success>> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
}
