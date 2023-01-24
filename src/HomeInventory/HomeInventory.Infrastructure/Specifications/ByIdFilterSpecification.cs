using Ardalis.Specification;
using HomeInventory.Infrastructure.Persistence;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal class ByIdFilterSpecification<TModel> : Specification<TModel>, ISingleResultSpecification<TModel>, ICompiledSingleResultSpecification<TModel>
    where TModel : class, IPersistentModel
{
    private static readonly Func<DatabaseContext, Guid, CancellationToken, Task<TModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DatabaseContext ctx, Guid id, CancellationToken _) => ctx.Set<TModel>().FirstOrDefault(x => x.Id == id));
    private readonly Guid _id;

    public ByIdFilterSpecification(Guid id)
    {
        Query.Where(x => x.Id == id);
        _id = id;
    }

    public Task<TModel?> ExecuteAsync(DatabaseContext context, CancellationToken cancellationToken) => _cachedQuery(context, _id, cancellationToken);
}

internal interface ICompiledSingleResultSpecification<T>
{
    Task<T?> ExecuteAsync(DatabaseContext context, CancellationToken cancellationToken);
}
