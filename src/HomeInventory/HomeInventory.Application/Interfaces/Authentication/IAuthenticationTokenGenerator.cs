using HomeInventory.Domain.Aggregates;
using HomeInventory.Domain.Primitives;

namespace HomeInventory.Application.Interfaces.Authentication;

public interface IAuthenticationTokenGenerator
{
    Task<string> GenerateTokenAsync(User user, IDateTimeService dateTimeService, CancellationToken cancellationToken = default);
}
