using HomeInventory.Domain.Primitives;

namespace HomeInventory.Infrastructure.Specifications;

internal interface ICompiledSingleResultSpecification<T>
{
    Task<T?> ExecuteAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken);
}
