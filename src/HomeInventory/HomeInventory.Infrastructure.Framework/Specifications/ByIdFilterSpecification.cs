using Ardalis.Specification;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

public class ByIdFilterSpecification<TModel> : Specification<TModel>, ISingleResultSpecification<TModel>, ICompiledSingleResultSpecification<TModel>
    where TModel : class, IPersistentModel
{
    private static readonly Func<DbContext, Ulid, CancellationToken, Task<TModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DbContext ctx, Ulid id, CancellationToken t) => ctx.Set<TModel>().FirstOrDefaultAsync(x => x.Id == id, t).GetAwaiter().GetResult());

    private readonly Ulid _id;

    public ByIdFilterSpecification(Ulid id)
    {
        Query.Where(x => x.Id.Equals(id));
        _id = id;
    }

    public Task<TModel?> ExecuteAsync(DbContext context, CancellationToken cancellationToken) => _cachedQuery(context, _id, cancellationToken);
}

public class ByIdFilterSpecification<TModel, TId> : Specification<TModel>, ISingleResultSpecification<TModel>, ICompiledSingleResultSpecification<TModel>
    where TModel : class, IPersistentModel<TId>
    where TId : UlidIdentifierObject<TId>
{
    private static readonly Func<DbContext, TId, CancellationToken, Task<TModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DbContext ctx, TId id, CancellationToken t) => ctx.Set<TModel>().FirstOrDefaultAsync(x => x.Id == id, t).GetAwaiter().GetResult());
    private readonly TId _id;

    public ByIdFilterSpecification(TId id)
    {
        Query.Where(x => x.Id.Equals(id));
        _id = id;
    }

    public Task<TModel?> ExecuteAsync(DbContext context, CancellationToken cancellationToken) => _cachedQuery(context, _id, cancellationToken);
}
