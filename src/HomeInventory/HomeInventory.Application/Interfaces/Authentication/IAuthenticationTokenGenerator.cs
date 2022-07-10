namespace HomeInventory.Application.Interfaces.Authentication;
public interface IAuthenticationTokenGenerator
{
    Task<string> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken = default);
}
