using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : IUserRepository
{
    private static readonly ICollection<User> _users = new List<User>();
    private readonly IUserIdFactory _userIdFactory;

    public UserRepository(IUserIdFactory userIdFactory)
    {
        _userIdFactory = userIdFactory;
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
        where TSpecification : ICreateEntitySpecification<User>
    {
        await Task.CompletedTask;
        return specification switch
        {
            CreateUserSpecification s => Create(s),
            _ => new None(),
        };
    }

    private User Create(CreateUserSpecification specification)
    {
        var userId = _userIdFactory.CreateNew();
        var user = new User(userId)
        {
            FirstName = specification.FirstName,
            LastName = specification.LastName,
            Email = specification.Email,
            Password = specification.Password,
        };
        _users.Add(user);
        return user;
    }
}
