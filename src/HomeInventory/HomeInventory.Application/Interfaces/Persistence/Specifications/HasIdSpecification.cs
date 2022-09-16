using System.Linq.Expressions;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Persistence.Specifications;

public class HasIdSpecification<TEntity, TIdentity> : FilterSpecification<TEntity>
    where TIdentity : notnull, IIdentifierObject<TIdentity>
    where TEntity : IEntity<TEntity, TIdentity>
{
    private readonly TIdentity _id;

    public HasIdSpecification(TIdentity id) => _id = id;

    protected override Expression<Func<TEntity, bool>> ToExpressionCore() => x => x.Id.Equals(_id);
}
