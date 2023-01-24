using Ardalis.Specification;
using AutoMapper;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal abstract class Repository<TModel, TEntity> : IRepository<TEntity>
    where TModel : class, IPersistentModel
    where TEntity : class, Domain.Primitives.IEntity<TEntity>
{
    private readonly DatabaseContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ISpecificationEvaluator _evaluator;

    protected Repository(DatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator)
    {
        _dbContext = context;
        _mapper = mapper;
        _evaluator = evaluator;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        _ = await GetDbSet().AddAsync(model, cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);
        await GetDbSet().AddRangeAsync(models, cancellationToken);
        return entities;
    }

    public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        _ = GetDbSet().Update(model);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);
        GetDbSet().UpdateRange(models);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var model = ToModel(entity);
        _ = GetDbSet().Remove(model);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var models = entities.Select(ToModel);
        GetDbSet().RemoveRange(models);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await GetDbSet().CountAsync(cancellationToken);
    }

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return await GetDbSet().AnyAsync(cancellationToken);
    }

    protected TModel ToModel(TEntity entity) => _mapper.Map<TEntity, TModel>(entity);

    protected TEntity ToEntity(TModel model) => _mapper.Map<TModel, TEntity>(model);

    protected IQueryable<TEntity> ToEntity(IQueryable<TModel> query, CancellationToken cancellationToken) => _mapper.ProjectTo<TEntity>(query, cancellationToken);

    protected async ValueTask<OneOf<TEntity, NotFound>> FindFirstOrNotFoundAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        if (specification is ICompiledSingleResultSpecification<TModel> compiled)
        {
            var model = await compiled.ExecuteAsync(_dbContext, cancellationToken);
            if (model is not null)
            {
                return ToEntity(model);
            }
        }
        else
        {
            var query = ApplySpecification(specification);
            var projected = ToEntity(query, cancellationToken);
            if (await projected.FirstOrDefaultAsync(cancellationToken) is TEntity entity)
            {
                return entity;
            }
        }

        return new NotFound();
    }

    protected async ValueTask<bool> HasAsync(ISpecification<TModel> specification, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(specification, evaluateCriteriaOnly: true);
        return await query.AnyAsync(cancellationToken);
    }

    /// <summary>
    /// Filters the entities  of <typeparamref name="T"/>, to those that match the encapsulated query logic of the
    /// <paramref name="specification"/>.
    /// </summary>
    /// <param name="specification">The encapsulated query logic.</param>
    /// <returns>The filtered entities as an <see cref="IQueryable{T}"/>.</returns>
    protected virtual IQueryable<TModel> ApplySpecification(ISpecification<TModel> specification, bool evaluateCriteriaOnly = false)
    {
        return _evaluator.GetQuery(GetDbSet().AsQueryable(), specification, evaluateCriteriaOnly);
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
    protected virtual IQueryable<TResult> ApplySpecification<TResult>(ISpecification<TModel, TResult> specification)
    {
        return _evaluator.GetQuery(GetDbSet().AsQueryable(), specification);
    }

    protected DbSet<TModel> GetDbSet() => _dbContext.Set<TModel>();
}
