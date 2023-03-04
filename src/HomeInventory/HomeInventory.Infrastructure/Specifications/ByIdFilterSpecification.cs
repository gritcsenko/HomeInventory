﻿using Ardalis.Specification;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Specifications;

internal class ByIdFilterSpecification<TModel> : Specification<TModel>, ISingleResultSpecification<TModel>, ICompiledSingleResultSpecification<TModel>
    where TModel : class, IPersistentModel
{
    private static readonly Func<DbContext, Guid, CancellationToken, Task<TModel?>> _cachedQuery =
        EF.CompileAsyncQuery((DbContext ctx, Guid id, CancellationToken t) => ctx.Set<TModel>().FirstOrDefaultAsync(x => x.Id == id, t).GetAwaiter().GetResult());
    private readonly Guid _id;

    public ByIdFilterSpecification(Guid id)
    {
        Query.Where(x => x.Id == id);
        _id = id;
    }

    public Task<TModel?> ExecuteAsync(IUnitOfWork unitOfWork, CancellationToken cancellationToken) => _cachedQuery(unitOfWork.DbContext, _id, cancellationToken);
}