using HomeInventory.Domain.UserManagement.Aggregates;

namespace HomeInventory.Application.UserManagement.Interfaces;

public interface IAuthenticationTokenGenerator
{
    ValueTask<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}
