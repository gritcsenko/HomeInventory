namespace HomeInventory.Domain.Primitives.Ids;

public interface ICuidBuildable<TSelf> : IIdBuildable<TSelf, Cuid, CuidIdentifierObjectBuilder<TSelf>>
    where TSelf : class, ICuidBuildable<TSelf>, ICuidIdentifierObject<TSelf>
{
}

