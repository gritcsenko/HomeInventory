using Ardalis.Specification;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal class ByIdFilterSpecification<TModel> : ByIdFilterSpecification<TModel, Guid>
    where TModel : class, IPersistentModel
{
    public ByIdFilterSpecification(Guid id)
        : base(id)
    {
    }
}

internal class ByIdFilterSpecification<TModel, TId> : Specification<TModel>, ISingleResultSpecification<TModel>, ICompiledSingleResultSpecification<TModel>
    where TModel : class, IPersistentModel<TId>
    where TId : notnull, IEquatable<TId>
{
    private static readonly Func<DbContext, TId, CancellationToken, Task<TModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DbContext ctx, TId id, CancellationToken t) => ctx.Set<TModel>().FirstOrDefaultAsync(x => x.Id.Equals(id), t).GetAwaiter().GetResult());
    private readonly TId _id;

    public ByIdFilterSpecification(TId id)
    {
        Query.Where(x => x.Id.Equals(id));
        _id = id;
    }

    public Task<TModel?> ExecuteAsync(DbContext context, CancellationToken cancellationToken) => _cachedQuery(context, _id, cancellationToken);
}
