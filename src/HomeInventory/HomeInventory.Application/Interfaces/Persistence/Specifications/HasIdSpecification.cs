using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using System.Linq.Expressions;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public class HasIdSpecification<TEntity, TIdentity> : FilterSpecification<TEntity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TEntity : IEntity<TEntity, TIdentity>
{
    public HasIdSpecification(TIdentity id) => Id = id;

    public TIdentity Id { get; }

    protected override Expression<Func<TEntity, bool>> ToExpression() => x => x.Id.Equals(Id);
}
