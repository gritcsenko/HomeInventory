using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

public interface ICompiledSingleResultSpecification<T>
{
    Task<T?> ExecuteAsync(DbContext context, CancellationToken cancellationToken);
}