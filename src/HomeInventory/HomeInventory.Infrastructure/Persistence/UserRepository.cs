using AutoMapper;
using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : IUserRepository
{
    private static readonly ICollection<User> _users = new List<User>();
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;

    public UserRepository(IDatabaseContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OneOf<User, NotFound>> FindFirstOrNotFoundAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : class, IExpressionSpecification<User, bool>
    {
        await ValueTask.CompletedTask;
        return _users.AsQueryable().FirstOrDefault(specification.ToExpression()) ?? (OneOf<User, NotFound>)new NotFound();
    }

    public async Task<bool> HasAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : class, IExpressionSpecification<User, bool>
    {
        await ValueTask.CompletedTask;
        return _users.AsQueryable().Any(specification.ToExpression());
    }

    public async Task<OneOf<User, None>> CreateAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : ICreateEntitySpecification<User> =>
        specification switch
        {
            CreateUserSpecification s => await CreateAsync(s, cancellationToken),
            _ => new None(),
        };

    private async Task<User> CreateAsync(CreateUserSpecification specification, CancellationToken cancellationToken)
    {
#pragma warning disable CA2252 // This API requires opting into preview features
        var builder = UserId.CreateBuilder();
#pragma warning restore CA2252 // This API requires opting into preview features
        var userId = builder.WithValue(specification.UserIdSupplier).Invoke();
        var user = new User(userId)
        {
            Email = specification.Email,
            Password = specification.Password,
        };
        _users.Add(user);

        var model = _mapper.Map<UserModel>(user);
        await _context.Users.AddAsync(model, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return user;
    }
}
