using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using MapsterMapper;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;
internal class UserRepository : IUserRepository
{
    private static readonly ICollection<User> _users = new List<User>();
    private readonly IMapper _mapper;
    private readonly IUserIdFactory _userIdFactory;

    public UserRepository(IMapper mapper, IUserIdFactory userIdFactory)
    {
        _mapper = mapper;
        _userIdFactory = userIdFactory;
    }

    public async Task<OneOf<User, NotFound>> FindFirstOrNotFoundAsync(FilterSpecification<User> specification, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;
        return _users.FirstOrDefault(specification.IsSatisfiedBy) ?? (OneOf<User, NotFound>)new NotFound();
    }

    public async Task<bool> HasAsync(FilterSpecification<User> specification, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;
        return _users.Any(specification.IsSatisfiedBy);
    }

    public async Task<OneOf<User, None>> CreateAsync<TSpecification>(TSpecification specification, CancellationToken cancellationToken = default)
        where TSpecification : ICreateEntitySpecification<User>
    {
        return specification switch
        {
            CreateUserSpecification s => await CreateAsync(s, cancellationToken),
            _ => new None(),
        };
    }

    private async Task<User> CreateAsync(CreateUserSpecification specification, CancellationToken cancellationToken = default)
    {
        await ValueTask.CompletedTask;
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
