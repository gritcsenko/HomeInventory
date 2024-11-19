using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Infrastructure.Framework.Mapping;
using HomeInventory.Infrastructure.Framework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeInventory.Infrastructure.Framework;

public abstract class Repository<TModel, TAggregateRoot, TIdentifier>(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator, IEventsPersistenceService eventsPersistenceService) : IRepository<TAggregateRoot>
    where TModel : class, IPersistentModel<TIdentifier>
    where TAggregateRoot : AggregateRoot<TAggregateRoot, TIdentifier>
    where TIdentifier : IIdentifierObject<TIdentifier>
{
    private readonly IDatabaseContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ISpecificationEvaluator _evaluator = evaluator;
    private readonly IEventsPersistenceService _eventsPersistenceService = eventsPersistenceService;

    public async Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = await InternalAddAsync(set, entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities.WithCancellation(cancellationToken))
        {
            _ = await InternalAddAsync(set, entity, cancellationToken);
        }
    }

    public async Task UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = await InternalUpdateAsync(set, entity, cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities.WithCancellation(cancellationToken))
        {
            _ = await InternalUpdateAsync(set, entity, cancellationToken);
        }
    }

    public async Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = await InternalDeleteAsync(set, entity, cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities.WithCancellation(cancellationToken))
        {
            _ = await InternalDeleteAsync(set, entity, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await Set().CountAsync(cancellationToken);

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        await Set().AnyAsync(cancellationToken);

    public IAsyncEnumerable<TAggregateRoot> GetAllAsync(CancellationToken cancellationToken = default) =>
        ToEntity(Set(), cancellationToken).ToAsyncEnumerable();

    public async Task<Option<TAggregateRoot>> FindFirstOptionAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification);
        var projected = ToEntity(query, cancellationToken);
        if (await projected.FirstOrDefaultAsync(cancellationToken) is { } entity)
        {
            return entity;
        }

        return OptionNone.Default;
    }

    public async Task<bool> HasAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification, evaluateCriteriaOnly: true);
        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Filters the entities  of <typeparamref name="TModel"/>, to those that match the encapsulated query logic of the
    /// <paramref name="specification"/>.
    /// </summary>
    /// <param name="inputQuery"></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly"></param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TModel> ApplySpecification(IQueryable<TModel> inputQuery, ISpecification<TModel> specification, bool evaluateCriteriaOnly = false) =>
        _evaluator.GetQuery(inputQuery, specification, evaluateCriteriaOnly);

    /// <summary>
    /// Filters all entities of <typeparamref name="TModel" />, that matches the encapsulated query logic of the
    /// <paramref name="specification"/>, from the database.
    /// <para>
    /// Projects each entity into a new form, being <typeparamref name="TResult" />.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="inputQuery"></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(IQueryable<TModel> inputQuery, ISpecification<TModel, TResult> specification) =>
        _evaluator.GetQuery(inputQuery, specification);

    private async Task<EntityEntry<TModel>> InternalAddAsync(DbSet<TModel> set, TAggregateRoot entity, CancellationToken cancellationToken) => await InternalModifyAsync(set, entity, static (s, m) => s.Add(m), cancellationToken);

    private async Task<EntityEntry<TModel>> InternalUpdateAsync(DbSet<TModel> set, TAggregateRoot entity, CancellationToken cancellationToken) => await InternalModifyAsync(set, entity, static (s, m) => s.Update(m), cancellationToken);

    private async Task<EntityEntry<TModel>> InternalDeleteAsync(DbSet<TModel> set, TAggregateRoot entity, CancellationToken cancellationToken) => await InternalModifyAsync(set, entity, static (s, m) => s.Remove(m), cancellationToken);

    private async Task<EntityEntry<TModel>> InternalModifyAsync(DbSet<TModel> set, TAggregateRoot entity, Func<DbSet<TModel>, TModel, EntityEntry<TModel>> modifyAction, CancellationToken cancellationToken)
    {
        var model = ToModel(entity);
        var entry = modifyAction(set, model);
        await _eventsPersistenceService.SaveEventsAsync(entity, cancellationToken);
        return entry;
    }

    private TModel ToModel(TAggregateRoot entity) =>
        _context.FindTracked<TModel>(m => m.Id.Equals(entity.Id))
            .IfNone(() => _mapper.MapOrThrow<TAggregateRoot, TModel>(entity));

    private IQueryable<TAggregateRoot> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) =>
        _mapper.ProjectTo<TAggregateRoot>(query, cancellationToken);

    private DbSet<TModel> Set() => _context.GetDbSet<TModel>();
}
