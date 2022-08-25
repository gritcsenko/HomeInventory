using HomeInventory.Application.Interfaces.Persistence;
using HomeInventory.Application.Interfaces.Persistence.Specifications;
using HomeInventory.Domain.Entities;
using HomeInventory.Domain.ValueObjects;
using HomeInventory.Infrastructure.Persistence.Models;
using MapsterMapper;
using OneOf;
using OneOf.Types;

namespace HomeInventory.Infrastructure.Persistence;

internal class UserRepository : IUserRepository
{
    private static readonly ICollection<User> _users = new List<User>();
    private readonly IUserIdFactory _userIdFactory;
    private readonly IDatabaseContext _context;
    private readonly IMapper _mapper;

    public UserRepository(IUserIdFactory userIdFactory, IDatabaseContext context, IMapper mapper)
    {
        _userIdFactory = userIdFactory;
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
        var userId = _userIdFactory.CreateNew();
        var user = new User(userId)
        {
            FirstName = specification.FirstName,
            LastName = specification.LastName,
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
