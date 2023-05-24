using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal interface ICompiledSingleResultSpecification<T>
{
    Task<T?> ExecuteAsync(DbContext context, CancellationToken cancellationToken);
}
