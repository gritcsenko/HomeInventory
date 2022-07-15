using HomeInventory.Domain.ValueObjects;

namespace HomeInventory.Domain.Entities;
public interface IEntity<TIdentity>
    where TIdentity : notnull, IIdentityValue
{
    TIdentity Id { get; }
}

public interface IEntity<TEntity, TIdentity> : IEntity<TIdentity>, IEquatable<TEntity>
    where TIdentity : notnull, IIdentityValue
    where TEntity : notnull, IEntity<TEntity, TIdentity>
{
}
