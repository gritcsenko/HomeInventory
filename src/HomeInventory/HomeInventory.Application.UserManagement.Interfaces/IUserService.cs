using HomeInventory.Application.Framework.Messaging;
using HomeInventory.Application.UserManagement.Interfaces.Commands;
using HomeInventory.Application.UserManagement.Interfaces.Queries;

namespace HomeInventory.Application.UserManagement.Interfaces;

public interface IUserService
{
    Task<IQueryResult<AuthenticateResult>> AuthenticateAsync(AuthenticateQuery query, CancellationToken cancellationToken = default);
    
    Task<IQueryResult<UserIdResult>> GetUserIdAsync(UserIdQuery query, CancellationToken cancellationToken = default);
    
    Task<Option<Error>> RegisterAsync(RegisterCommand command, CancellationToken cancellationToken = default);
}
