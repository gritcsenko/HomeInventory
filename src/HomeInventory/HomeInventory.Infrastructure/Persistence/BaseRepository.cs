using AutoMapper;
using HomeInventory.Domain.Persistence;
using HomeInventory.Domain.Primitives;
using HomeInventory.Infrastructure.Persistence.Models;
using HomeInventory.Infrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal abstract class BaseRepository<TModel, TEntity> : IRepository<TEntity>
    where TModel : class, IPersistentModel
    where TEntity : class, IEntity<TEntity>
{
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;
    private readonly ISpecificationEvaluator _evaluator;

    protected BaseRepository(IDatabaseContext context, IMapper mapper, ISpecificationEvaluator evaluator)
    {
        _context = context;
        _mapper = mapper;
        _evaluator = evaluator;
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        var model = _mapper.Map<TModel>(entity);

        await _context.Set<TModel>().AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    protected async ValueTask<OneOf<TEntity, NotFound>> FindFirstOrNotFoundAsync(IFilterSpecification<TModel> specification, CancellationToken cancellationToken)
    {
        var userModel = await FirstOrDefaultAsync(specification, cancellationToken);
        if (userModel is not null)
        {
            return _mapper.Map<TEntity>(userModel);
        }

        return new NotFound();
    }

    protected async ValueTask<bool> HasAsync(IFilterSpecification<TModel> specification, CancellationToken cancellationToken)
    {
        return await AnyAsync(specification, cancellationToken);
    }

    private async ValueTask<TModel?> FirstOrDefaultAsync(IFilterSpecification<TModel> specification, CancellationToken cancellationToken)
    {
        var query = FilterBy(specification);
        TModel? userModel;
        if (query is IAsyncQueryProvider)
        {
            userModel = await query.FirstOrDefaultAsync(cancellationToken);
        }
        else
        {
            userModel = query.FirstOrDefault();
        }

        return userModel;
    }

    private async ValueTask<bool> AnyAsync(IFilterSpecification<TModel> specification, CancellationToken cancellationToken)
    {
        var query = FilterBy(specification);
        if (query is IAsyncQueryProvider)
        {
            return await query.AnyAsync(cancellationToken);
        }

        return query.Any();
    }

    private IQueryable<TModel> FilterBy(IFilterSpecification<TModel> specification) =>
        _evaluator.FilterBy(_context.Set<TModel>().AsQueryable(), specification);
}
