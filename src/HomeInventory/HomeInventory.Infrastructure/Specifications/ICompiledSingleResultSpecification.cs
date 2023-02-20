using HomeInventory.Infrastructure.Persistence;

namespace HomeInventory.Infrastructure.Specifications;

internal interface ICompiledSingleResultSpecification<T>
{
    Task<T?> ExecuteAsync(DatabaseContext context, CancellationToken cancellationToken);
}
