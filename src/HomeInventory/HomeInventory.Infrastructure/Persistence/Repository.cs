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

    public async ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await using var container = await GetDbSetAsync(cancellationToken);

        container.Set.Add(model);
    }

    public async ValueTask AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await using var container = await GetDbSetAsync(cancellationToken);

        container.Set.AddRange(entities.Select(ToModel));
    }

    public async ValueTask UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await using var container = await GetDbSetAsync(cancellationToken);

        container.Set.Update(model);
    }

    public async ValueTask UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        await using var container = await GetDbSetAsync(cancellationToken);

        container.Set.UpdateRange(models);
    }

    public async ValueTask DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        await using var container = await GetDbSetAsync(cancellationToken);

        container.Set.Remove(model);
    }

    public async ValueTask DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        await using var container = await GetDbSetAsync(cancellationToken);

        container.Set.RemoveRange(models);
    }

    public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var container = await GetDbSetAsync(cancellationToken);

        return await container.Set.CountAsync(cancellationToken);
    }

    public async ValueTask<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        await using var container = await GetDbSetAsync(cancellationToken);

        return await container.Set.AnyAsync(cancellationToken);
    }

    public async IAsyncEnumerable<TEntity> GetAllAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var container = await GetDbSetAsync(cancellationToken);

        await foreach (var entity in ToEntity(container.Set, cancellationToken))
        {
            yield return entity;
        }
    }

    public async ValueTask<IUnitOfWork> WithUnitOfWorkAsync(CancellationToken cancellationToken = default) =>
        await _container.CreateNewAsync(cancellationToken);

    protected async ValueTask<Optional<TEntity>> FindFirstOptionalAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        if (specification is ICompiledSingleResultSpecification<TModel> compiled)
        {
            return await FindFirstCompiledOptionalAsync(compiled, cancellationToken);
        }

        await using var container = await GetDbSetAsync(cancellationToken);

        var query = ApplySpecification(container.Set, specification);
        var projected = ToEntity(query, cancellationToken);
        if (await projected.FirstOrDefaultAsync(cancellationToken) is TEntity entity)
        {
            return entity;
        }

        return Optional.None<TEntity>();
    }

    protected async ValueTask<bool> HasAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        await using var container = await GetDbSetAsync(cancellationToken);
        var query = ApplySpecification(container.Set, specification, evaluateCriteriaOnly: true);
        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Filters the entities  of <typeparamref name="T"/>, to those that match the encapsulated query logic of the
    /// <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TModel> ApplySpecification(IQueryable<TModel> set, ISpecification<TModel> specification, bool evaluateCriteriaOnly = false) =>
        _evaluator.GetQuery(set, specification, evaluateCriteriaOnly);

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
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(IQueryable<TModel> set, ISpecification<TModel, TResult> specification) =>
        _evaluator.GetQuery(set, specification);

    private async ValueTask<DbSetContainer> GetDbSetAsync(CancellationToken cancellationToken = default)
    {
        var context = await _container.EnsureAsync(cancellationToken);

        return new DbSetContainer(context.Set<TModel>(), _container.Resource);
    }

    private async ValueTask<Optional<TEntity>> FindFirstCompiledOptionalAsync(ICompiledSingleResultSpecification<TModel> compiled, CancellationToken cancellationToken = default)
    {
        var context = await _container.EnsureAsync(cancellationToken);
        await using var _ = _container.Resource;

        if (await compiled.ExecuteAsync(context, cancellationToken) is TModel model)
        {
            return ToEntity(model);
        }

        return Optional.None<TEntity>();
    }

    private TModel ToModel(TEntity entity) => _mapper.Map<TEntity, TModel>(entity);

    private TEntity ToEntity(TModel model) => _mapper.Map<TModel, TEntity>(model);

    private IQueryable<TEntity> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) =>
        _mapper.ProjectTo<TEntity>(query, cancellationToken);

    private sealed class DbSetContainer : Disposable, IAsyncDisposable
    {
        private readonly IAsyncDisposable _resource;

        public DbSetContainer(DbSet<TModel> set, IAsyncDisposable resource)
        {
            Set = set;
            _resource = resource;
        }

        public DbSet<TModel> Set { get; }

        ValueTask IAsyncDisposable.DisposeAsync() => _resource.DisposeAsync();
    }
}
