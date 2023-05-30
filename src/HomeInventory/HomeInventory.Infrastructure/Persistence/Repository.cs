using Ardalis.Specification;
using AutoMapper;
using DotNext;
using HomeInventory.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace HomeInventory.Infrastructure.Persistence;

internal abstract class Repository<TModel, TEntity> : IRepository<TEntity>
    where TModel : class
    where TEntity : class, Domain.Primitives.IEntity<TEntity>
{
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ISpecificationEvaluator _evaluator;

    protected Repository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator, IDateTimeService dateTimeService)
    {
        _context = context;
        _mapper = mapper;
        _evaluator = evaluator;
    }

    public ValueTask AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        Set().Add(model);

        return ValueTask.CompletedTask;
    }

    public ValueTask AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        Set().AddRange(models);

        return ValueTask.CompletedTask;
    }

    public ValueTask UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        Set().Update(model);

        return ValueTask.CompletedTask;
    }

    public ValueTask UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        Set().UpdateRange(models);

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);

        Set().Remove(model);

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);

        Set().RemoveRange(models);

        return ValueTask.CompletedTask;
    }

    public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default) =>
        await Set().CountAsync(cancellationToken);

    public async ValueTask<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        await Set().AnyAsync(cancellationToken);

    public IAsyncEnumerable<TEntity> GetAllAsync(CancellationToken cancellationToken = default) =>
        AsyncEnumerable.ToAsyncEnumerable(ToEntity(Set(), cancellationToken));

    public async ValueTask<Optional<TEntity>> FindFirstOptionalAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification);
        var projected = ToEntity(query, cancellationToken);
        if (await projected.FirstOrDefaultAsync(cancellationToken) is TEntity entity)
        {
            return entity;
        }

        return Optional.None<TEntity>();
    }

    public async ValueTask<bool> HasAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification, evaluateCriteriaOnly: true);
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

    private TModel ToModel(TEntity entity) => _mapper.Map<TEntity, TModel>(entity);

    private IQueryable<TEntity> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) =>
        _mapper.ProjectTo<TEntity>(query, cancellationToken);

    private DbSet<TModel> Set() => _context.Set<TModel>();
}
