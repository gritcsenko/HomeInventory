using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Framework.Specifications;

public interface ICompiledSingleResultSpecification<T>
{
    Task<T?> ExecuteAsync(DbContext context, CancellationToken cancellationToken);
}
