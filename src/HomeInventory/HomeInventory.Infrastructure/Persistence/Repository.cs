using System.Runtime.CompilerServices;
using Ardalis.Specification;
using AutoMapper;
using DotNext;
using DotNext.Collections.Generic;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal abstract class Repository<TModel, TEntity> : IRepository<TEntity>
    where TModel : class
    where TEntity : class, Domain.Primitives.IEntity<TEntity>
{
    private readonly UnitOfWorkContainer _container;
    private readonly IMapper _mapper;
    private readonly ISpecificationEvaluator _evaluator;

    protected Repository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator)
    {
        _container = new UnitOfWorkContainer(contextFactory);
        _mapper = mapper;
        _evaluator = evaluator;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        await set.AddAsync(model, cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        await set.AddRangeAsync(models, cancellationToken);
        return entities;
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        set.Update(model);
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        set.UpdateRange(models);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        set.Remove(model);
    }

    public async Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        set.RemoveRange(models);
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        return await set.CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        return await set.AnyAsync(cancellationToken);
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        await foreach (var entity in ToEntity(set, cancellationToken))
        {
            yield return entity;
        }
    }

    public async Task<IUnitOfWork> WithUnitOfWorkAsync(CancellationToken cancellationToken = default) =>
        await _container.CreateNewAsync(cancellationToken);

    protected TModel ToModel(TEntity entity) => _mapper.Map<TEntity, TModel>(entity);

    protected TEntity ToEntity(TModel model) => _mapper.Map<TModel, TEntity>(model);

    protected IQueryable<TEntity> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) =>
        _mapper.ProjectTo<TEntity>(query, cancellationToken);

    protected async ValueTask<Optional<TEntity>> FindFirstOptionalAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        if (specification is ICompiledSingleResultSpecification<TModel> compiled)
        {
            return await FindFirstCompiledOptionalAsync(compiled, cancellationToken);
        }

        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        var query = ApplySpecification(set, specification);
        var projected = ToEntity(query, cancellationToken);
        if (await projected.FirstOrDefaultAsync(cancellationToken) is TEntity entity)
        {
            return entity;
        }

        return Optional.None<TEntity>();
    }

    protected async ValueTask<bool> HasAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;
        var query = ApplySpecification(set, specification, evaluateCriteriaOnly: true);
        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Filters the entities  of <typeparamref name="T"/>, to those that match the encapsulated query logic of the
    /// <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TModel> ApplySpecification(DbSet<TModel> set, ISpecification<TModel> specification, bool evaluateCriteriaOnly = false) =>
        _evaluator.GetQuery(set.AsQueryable(), specification, evaluateCriteriaOnly);

    /// <summary>
    /// Filters all entities of <typeparamref name="T" />, that matches the encapsulated query logic of the
    /// <paramref name="specification"/>, from the database.
    /// <para>
    /// Projects each entity into a new form, being <typeparamref name="TResult" />.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(DbSet<TModel> set, ISpecification<TModel, TResult> specification) =>
        _evaluator.GetQuery(set.AsQueryable(), specification);

    protected async ValueTask<(DbSet<TModel> set, IAsyncDisposable resource)> GetDbSetAsync(CancellationToken cancellationToken = default)
    {
        var unit = await _container.EnsureAsync(cancellationToken);

        return (unit.DbContext.Set<TModel>(), _container.Resource);
    }

    private async ValueTask<Optional<TEntity>> FindFirstCompiledOptionalAsync(ICompiledSingleResultSpecification<TModel> compiled, CancellationToken cancellationToken = default)
    {
        var unit = await _container.EnsureAsync(cancellationToken);
        await using var _ = _container.Resource;

        if (await compiled.ExecuteAsync(unit.DbContext, cancellationToken) is TModel model)
        {
            return ToEntity(model);
        }

        return Optional.None<TEntity>();
    }
}
