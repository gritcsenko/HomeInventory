using Ardalis.Specification;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

public class ByIdFilterSpecification<TModel, TId> : Specification<TModel>, ISingleResultSpecification<TModel>, ICompiledSingleResultSpecification<TModel>
    where TModel : class, IPersistentModel<TId>
    where TId : UlidIdentifierObject<TId>
{
    private static readonly Func<DbContext, TId, Task<TModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DbContext ctx, TId id) => ctx.Set<TModel>().FirstOrDefault(x => x.Id == id));
    private readonly TId _id;

    public ByIdFilterSpecification(TId id)
    {
        Query.Where(x => x.Id.Equals(id));
        _id = id;
    }

    public Task<TModel?> ExecuteAsync(DbContext context, CancellationToken cancellationToken) => _cachedQuery(context, _id);
}
