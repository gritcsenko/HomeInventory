using Ardalis.Specification;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Infrastructure.Framework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeInventory.Infrastructure.Framework;

public abstract class Repository<TPersistent, TAggregateRoot, TIdentifier>(IDatabaseContext context, ISpecificationEvaluator evaluator, IEventsPersistenceService eventsPersistenceService, IPersistentMapper<TPersistent, TAggregateRoot, TIdentifier> mapper) : IRepository<TAggregateRoot>
    where TPersistent : class, IPersistentModel<TIdentifier>
    where TAggregateRoot : AggregateRoot<TAggregateRoot, TIdentifier>
    where TIdentifier : IIdentifierObject<TIdentifier>
{
    private readonly IDatabaseContext _context = context;
    private readonly ISpecificationEvaluator _evaluator = evaluator;
    private readonly IEventsPersistenceService _eventsPersistenceService = eventsPersistenceService;
    private readonly IPersistentMapper<TPersistent, TAggregateRoot, TIdentifier> _mapper = mapper;

    public async Task AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = await InternalAddAsync(set, entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities)
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

        foreach (var entity in entities)
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

        foreach (var entity in entities)
        {
            _ = await InternalDeleteAsync(set, entity, cancellationToken);
        }
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await Set().CountAsync(cancellationToken);

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        await Set().AnyAsync(cancellationToken);

    public IAsyncEnumerable<TAggregateRoot> GetAllAsync(CancellationToken cancellationToken = default) =>
        ToEntity(Set()).ToAsyncEnumerable();

    public async Task<Option<TAggregateRoot>> FindFirstOptionAsync(ISpecification<TPersistent> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification);
        var projected = ToEntity(query);
        return await projected.FirstOrDefaultAsync(cancellationToken) is { } entity
            ? Prelude.Optional(entity)
            : Option<TAggregateRoot>.None;
    }

    public async Task<bool> HasAsync(ISpecification<TPersistent> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification, evaluateCriteriaOnly: true);
        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Filters the entities  of <typeparamref name="TPersistent"/>, to those that match the encapsulated query logic of the
    /// <paramref name="specification"/>.
    /// </summary>
    /// <param name="inputQuery"></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <param name="evaluateCriteriaOnly"></param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TPersistent> ApplySpecification(IQueryable<TPersistent> inputQuery, ISpecification<TPersistent> specification, bool evaluateCriteriaOnly = false) =>
        _evaluator.GetQuery(inputQuery, specification, evaluateCriteriaOnly);

    /// <summary>
    /// Filters all entities of <typeparamref name="TPersistent" />, that matches the encapsulated query logic of the
    /// <paramref name="specification"/>, from the database.
    /// <para>
    /// Projects each entity into a new form, being <typeparamref name="TResult" />.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The type of the value returned by the projection.</typeparam>
    /// <param name="inputQuery"></param>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered projected entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(IQueryable<TPersistent> inputQuery, ISpecification<TPersistent, TResult> specification) =>
        _evaluator.GetQuery(inputQuery, specification);

    private async Task<EntityEntry<TPersistent>> InternalAddAsync(DbSet<TPersistent> set, TAggregateRoot entity, CancellationToken cancellationToken) => await InternalModifyAsync(set, entity, static (s, m) => s.Add(m), cancellationToken);

    private async Task<EntityEntry<TPersistent>> InternalUpdateAsync(DbSet<TPersistent> set, TAggregateRoot entity, CancellationToken cancellationToken) => await InternalModifyAsync(set, entity, static (s, m) => s.Update(m), cancellationToken);

    private async Task<EntityEntry<TPersistent>> InternalDeleteAsync(DbSet<TPersistent> set, TAggregateRoot entity, CancellationToken cancellationToken) => await InternalModifyAsync(set, entity, static (s, m) => s.Remove(m), cancellationToken);

    private async Task<EntityEntry<TPersistent>> InternalModifyAsync(DbSet<TPersistent> set, TAggregateRoot entity, Func<DbSet<TPersistent>, TPersistent, EntityEntry<TPersistent>> modifyAction, CancellationToken cancellationToken)
    {
        var model = ToModel(entity);
        var entry = modifyAction(set, model);
        await _eventsPersistenceService.SaveEventsAsync(entity, cancellationToken);
        return entry;
    }

    private TPersistent ToModel(TAggregateRoot entity) =>
        _context.FindTracked<TPersistent>(m => m.Id.Equals(entity.Id))
            .Match(
                Some: existing => _mapper.ToPersistent(entity, existing),
                None: () => _mapper.ToPersistent(entity));

    private IQueryable<TAggregateRoot> ToEntity(IQueryable<TPersistent> query) =>
        _mapper.FromPersistent(query);

    private DbSet<TPersistent> Set() => _context.GetDbSet<TPersistent>();
}
