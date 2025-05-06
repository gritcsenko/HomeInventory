using HomeInventory.Domain.UserManagement.Aggregates;

namespace HomeInventory.Application.Interfaces.Authentication;

public interface IAuthenticationTokenGenerator
{
    ValueTask<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}
