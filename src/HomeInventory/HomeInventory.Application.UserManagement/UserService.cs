using System.Transactions;
using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.UserManagement.Interfaces;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;
using HomeInventory.Domain.Errors;
using HomeInventory.Domain.Primitives;
using HomeInventory.Domain.Primitives.Errors;
using HomeInventory.Domain.Primitives.Ids;
using HomeInventory.Domain.UserManagement.Aggregates;
using HomeInventory.Domain.UserManagement.Errors;
using HomeInventory.Domain.UserManagement.Persistence;
using HomeInventory.Domain.UserManagement.ValueObjects;
using Microsoft.Extensions.Logging;

namespace HomeInventory.Application.UserManagement;

internal class UserService(IAuthenticationTokenGenerator tokenGenerator, IPasswordHasher hasher, IScopeAccessor scopeAccessor, TimeProvider timeProvider, IIdSupplier<Ulid> eventIdSupplier, ILogger<UserService> logger) : IUserService
{
    private readonly IAuthenticationTokenGenerator _tokenGenerator = tokenGenerator;
    private readonly IPasswordHasher _hasher = hasher;
    private readonly IScopeAccessor _scopeAccessor = scopeAccessor;
    private readonly TimeProvider _timeProvider = timeProvider;
    private readonly IIdSupplier<Ulid> _eventIdSupplier = eventIdSupplier;
    private readonly ILogger _logger = logger;

    public async Task<IQueryResult<AuthenticateResult>> AuthenticateAsync(AuthenticateQuery query, CancellationToken cancellationToken = default)
    {
        var optional = await TryFindUserAsync(query, cancellationToken);
        var result = await optional
            .IfAsync((user, t) => IsPasswordMatchAsync(user, query.Password, t), cancellationToken)
            .SelectAsync(async (user, t) => (token: await _tokenGenerator.GenerateTokenAsync(user, t), id: user.Id), cancellationToken);

        var validationResult = result
            .Select(CreateAuthenticateResult)
            .ErrorIfNone(InvalidCredentialsError.Instance);

        return QueryResult.From(validationResult);
    }

    public async Task<IQueryResult<UserIdResult>> GetUserIdAsync(UserIdQuery query, CancellationToken cancellationToken = default)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();

        var result = await userRepository.FindFirstByEmailUserOptionalAsync(query.Email, cancellationToken);
        var validationResult = result
            .Select(CreateUserIdResult)
            .ErrorIfNone(() => new NotFoundError($"User with email {query.Email} was not found"));
        return QueryResult.From(validationResult);
    }

    public async Task<Optional<Error>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();

        if (await userRepository.IsUserHasEmailAsync(command.Email, cancellationToken))
        {
            return DuplicateEmailError.Instance;
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var result = await CreateUserAsync(command, cancellationToken)
            .MapAsync(async user =>
            {
                user.OnUserCreated(_eventIdSupplier, _timeProvider);
                await userRepository.AddAsync(user, cancellationToken);
                await SaveChangesAsync<RegisterCommand>(scope, cancellationToken);
                return Option<Error>.None;
            });

        return result.IfFail(errors => Error.Many(errors)).Match(e => new Optional<Error>(e), Optional<Error>.None);
    }

    private async Task SaveChangesAsync<TRequest>(TransactionScope transactionScope, CancellationToken cancellationToken)
    {
        var unitOfWork = _scopeAccessor.GetRequiredContext<IUnitOfWork>();

        var count = await unitOfWork.SaveChangesAsync(cancellationToken);
        switch (count)
        {
            case 0:
                _logger.HandleUnitOfWorkNotSaved<TRequest>();
                break;
            default:
                _logger.HandleUnitOfWorkSaved<TRequest>(count);
                break;
        }

        transactionScope.Complete();
    }

    private static Validation<Error, UserIdResult> CreateUserIdResult(User user) =>
        new UserIdResult(user.Id);

    private static AuthenticateResult CreateAuthenticateResult((string token, UserId id) t) =>
        new(t.id, t.token);

    private async Task<Optional<User>> TryFindUserAsync(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var userRepository = _scopeAccessor.GetRequiredContext<IUserRepository>();
        return await userRepository.FindFirstByEmailUserOptionalAsync(request.Email, cancellationToken);
    }

    private async Task<bool> IsPasswordMatchAsync(User user, string password, CancellationToken cancellationToken) =>
        await _hasher.VarifyHashAsync(password, user.PasswordHash, cancellationToken);

    private async Task<Validation<Error, User>> CreateUserAsync(RegisterCommand command, CancellationToken cancellationToken = default) =>
        await UserId
            .CreateBuilder()
            .WithNewId()
            .Build()
            .MapAsync(async id => await CreateUserAsync(command, id, cancellationToken));

    private async Task<User> CreateUserAsync(RegisterCommand command, UserId id, CancellationToken cancellationToken) =>
        new(id)
        {
            Email = command.Email,
            PasswordHash = await _hasher.HashAsync(command.Password, cancellationToken),
        };
}
