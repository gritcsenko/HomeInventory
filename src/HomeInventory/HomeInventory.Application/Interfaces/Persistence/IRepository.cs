using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Application.Interfaces.Persistence;

public interface IRepository<TEntity, TIdentity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TEntity : IEntity<TEntity, TIdentity>
{
    Task<OneOf<TEntity, NotFound>> FindFirstOrNotFoundAsync(FilterSpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<bool> HasAsync(FilterSpecification<TEntity> specification, CancellationToken cancellationToken = default);

    Task<OneOf<TEntity, None>> CreateAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : ICreateEntitySpecification<TEntity>;
}
