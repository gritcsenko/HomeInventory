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
    Task<OneOf<TEntity, NotFound>> FindFirstOrNotFoundAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : class, IFilterSpecification<TEntity>, IExpressionSpecification<TEntity, bool>;

    Task<bool> HasAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : class, IFilterSpecification<TEntity>, IExpressionSpecification<TEntity, bool>;

    Task<OneOf<TEntity, None>> CreateAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : ICreateEntitySpecification<TEntity>;
}
