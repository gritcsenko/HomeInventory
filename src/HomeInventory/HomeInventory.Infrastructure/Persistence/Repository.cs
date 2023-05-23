using System.Runtime.CompilerServices;
using Ardalis.Specification;
using AutoMapper;
using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal abstract class Repository<TModel, TEntity> : IRepository<TEntity>
    where TModel : class
    where TEntity : class, Domain.Primitives.IEntity<TEntity>
{
    private readonly IDbContextFactory<DatabaseContext> _factory;
    private readonly IMapper _mapper;
    private readonly ISpecificationEvaluator _evaluator;
    private readonly IDateTimeService _dateTimeService;
    private Optional<IUnitOfWork> _unitOfWork = Optional.None<IUnitOfWork>();

    protected Repository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator, IDateTimeService dateTimeService)
    {
        _factory = contextFactory;
        _mapper = mapper;
        _evaluator = evaluator;
        _dateTimeService = dateTimeService;
    }

    public Optional<IUnitOfWork> UnitOfWork => _unitOfWork;

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

        await foreach (var entity in set)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            yield return ToEntity(entity);
        }
    }

    public async Task<IUnitOfWork> WithUnitOfWorkAsync(CancellationToken cancellationToken = default)
    {
        var (unit, _) = await CreateUnitOfWorkAsync(cancellationToken);

        _unitOfWork = Optional.Some(unit);
        return unit;
    }

    protected TModel ToModel(TEntity entity) => _mapper.Map<TEntity, TModel>(entity);

    protected TEntity ToEntity(TModel model) => _mapper.Map<TModel, TEntity>(model);

    protected IQueryable<TEntity> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) => _mapper.ProjectTo<TEntity>(query, cancellationToken);

    protected async ValueTask<OneOf<TEntity, NotFound>> FindFirstOrNotFoundAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        if (specification is ICompiledSingleResultSpecification<TModel> compiled)
        {
            return await FindFirstOrNotFoundAsync(compiled, cancellationToken);
        }
        var (set, resource) = await GetDbSetAsync(cancellationToken);
        await using var _ = resource;

        var query = ApplySpecification(set, specification);
        var projected = ToEntity(query, cancellationToken);
        if (await projected.FirstOrDefaultAsync(cancellationToken) is TEntity entity)
        {
            return entity;
        }

        return new NotFound();
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
    protected virtual IQueryable<TModel> ApplySpecification(DbSet<TModel> set, ISpecification<TModel> specification, bool evaluateCriteriaOnly = false)
    {
        return _evaluator.GetQuery(set.AsQueryable(), specification, evaluateCriteriaOnly);
    }

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
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(DbSet<TModel> set, ISpecification<TModel, TResult> specification)
    {
        return _evaluator.GetQuery(set.AsQueryable(), specification);
    }

    protected async ValueTask<(DbSet<TModel> set, IAsyncDisposable resource)> GetDbSetAsync(CancellationToken cancellationToken = default)
    {
        var (unit, resource) = await GetUnitOfWorkAsync(cancellationToken);

        return (unit.DbContext.Set<TModel>(), resource);
    }

    private async ValueTask<OneOf<TEntity, NotFound>> FindFirstOrNotFoundAsync(ICompiledSingleResultSpecification<TModel> compiled, CancellationToken cancellationToken = default)
    {
        var (unit, resource) = await GetUnitOfWorkAsync(cancellationToken);
        await using var _ = resource;

        if (await compiled.ExecuteAsync(unit, cancellationToken) is TModel model)
        {
            return ToEntity(model);
        }
        return new NotFound();
    }

    private ValueTask<(IUnitOfWork unit, IAsyncDisposable resource)> GetUnitOfWorkAsync(CancellationToken cancellationToken = default) =>
        _unitOfWork.HasValue
            ? new ValueTask<(IUnitOfWork unit, IAsyncDisposable resource)>((_unitOfWork.Value, EmptyAsyncDisposable.Instance))
            : CreateUnitOfWorkAsync(cancellationToken);

    private async ValueTask<(IUnitOfWork unit, IAsyncDisposable resource)> CreateUnitOfWorkAsync(CancellationToken cancellationToken = default)
    {
        var context = await _factory.CreateDbContextAsync(cancellationToken);
        var unit = new UnitOfWork(context, _dateTimeService, new ReleaseUnitOfWork(this));
        return (unit, unit);
    }

    private sealed class ReleaseUnitOfWork : IDisposable
    {
        private readonly Repository<TModel, TEntity> _repository;

        public ReleaseUnitOfWork(Repository<TModel, TEntity> repository) => _repository = repository;

        void IDisposable.Dispose() => _repository._unitOfWork = Optional.None<IUnitOfWork>();
    }

    private sealed class EmptyAsyncDisposable : IAsyncDisposable
    {
        private EmptyAsyncDisposable()
        {
        }

        public static IAsyncDisposable Instance { get; } = new EmptyAsyncDisposable();

        ValueTask IAsyncDisposable.DisposeAsync() => ValueTask.CompletedTask;
    }
}

internal abstract class Repository<TModel, TEntity, TId> : Repository<TModel, TEntity>
    where TModel : class, IPersistentModel<TId>
    where TEntity : class, Domain.Primitives.IEntity<TEntity>
    where TId : GuidIdentifierObject<TId>
{
    protected Repository(IDbContextFactory<DatabaseContext> contextFactory, IMapper mapper, ISpecificationEvaluator evaluator, IDateTimeService dateTimeService)
        : base(contextFactory, mapper, evaluator, dateTimeService)
    {
    }
}
