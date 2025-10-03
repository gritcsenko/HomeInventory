using HomeInventory.Application.UserManagement.Interfaces.Queries;

namespace HomeInventory.Application.UserManagement.Interfaces;

public interface IAuthenticationService
{
    Task<Validation<Error, AuthenticateResult>> AuthenticateAsync(AuthenticateQuery query, CancellationToken cancellationToken = default);
}
