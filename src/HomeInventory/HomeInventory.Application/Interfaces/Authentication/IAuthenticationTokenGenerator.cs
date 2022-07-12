using HomeInventory.Domain.Entities;

namespace HomeInventory.Application.Interfaces.Authentication;
public interface IAuthenticationTokenGenerator
{
    Task<string> GenerateTokenAsync(User user, CancellationToken cancellationToken = default);
}
