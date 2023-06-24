using Ardalis.Specification;
using AutoMapper;
using DotNext;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace HomeInventory.Infrastructure.Persistence;

internal abstract class Repository<TModel, TAggregateRoot> : IRepository<TAggregateRoot>
    where TModel : class
    where TAggregateRoot : class, Domain.Primitives.IEntity<TAggregateRoot>, IHasDomainEvents
{
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ISpecificationEvaluator _evaluator;

    protected Repository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator)
    {
        _context = context;
        _mapper = mapper;
        _evaluator = evaluator;
    }

    public ValueTask AddAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = InternalAdd(set, entity);

        return ValueTask.CompletedTask;
    }

    public ValueTask AddRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities)
        {
            _ = InternalAdd(set, entity);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = InternalUpdate(set, entity);

        return ValueTask.CompletedTask;
    }

    public ValueTask UpdateRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities)
        {
            _ = InternalUpdate(set, entity);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken = default)
    {
        var set = Set();

        _ = InternalDelete(set, entity);

        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteRangeAsync(IEnumerable<TAggregateRoot> entities, CancellationToken cancellationToken = default)
    {
        var set = Set();

        foreach (var entity in entities)
        {
            _ = InternalDelete(set, entity);
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask<int> CountAsync(CancellationToken cancellationToken = default) =>
        await Set().CountAsync(cancellationToken);

    public async ValueTask<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        await Set().AnyAsync(cancellationToken);

    public IAsyncEnumerable<TAggregateRoot> GetAllAsync(CancellationToken cancellationToken = default) =>
        AsyncEnumerable.ToAsyncEnumerable(ToEntity(Set(), cancellationToken));

    public async ValueTask<Optional<TAggregateRoot>> FindFirstOptionalAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(Set(), specification);
        var projected = ToEntity(query, cancellationToken);
        if (await projected.FirstOrDefaultAsync(cancellationToken) is TAggregateRoot entity)
        {
            return entity;
        }

        return Optional.None<TAggregateRoot>();
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

    private EntityEntry<TModel> InternalAdd(DbSet<TModel> set, TAggregateRoot entity) => InternalModify(set, entity, (s, m) => s.Add(m));

    private EntityEntry<TModel> InternalUpdate(DbSet<TModel> set, TAggregateRoot entity) => InternalModify(set, entity, (s, m) => s.Update(m));

    private EntityEntry<TModel> InternalDelete(DbSet<TModel> set, TAggregateRoot entity) => InternalModify(set, entity, (s, m) => s.Remove(m));

    private EntityEntry<TModel> InternalModify(DbSet<TModel> set, TAggregateRoot entity, Func<DbSet<TModel>, TModel, EntityEntry<TModel>> modifyAction)
    {
        var model = ToModel(entity);
        var entry = modifyAction(set, model);
        SaveEvents(entity);
        return entry;
    }

    private TModel ToModel(TAggregateRoot entity) => _mapper.Map<TAggregateRoot, TModel>(entity);

    private IQueryable<TAggregateRoot> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) =>
        _mapper.ProjectTo<TAggregateRoot>(query, cancellationToken);

    private DbSet<TModel> Set() => _context.Set<TModel>();

    private void SaveEvents(TAggregateRoot entity)
    {
        var events = entity.GetDomainEvents();
        var messages = events.Select(CreateMessage);
        _context.OutboxMessages.AddRange(messages);
        entity.ClearDomainEvents();
    }

    private static OutboxMessage CreateMessage(IDomainEvent domainEvent) =>
        new(domainEvent.Id, domainEvent.Created, domainEvent);
}
