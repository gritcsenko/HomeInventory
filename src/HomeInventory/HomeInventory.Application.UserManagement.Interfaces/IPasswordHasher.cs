namespace HomeInventory.Application.Interfaces.Authentication;

public interface IPasswordHasher
{
    ValueTask<string> HashAsync(string password, CancellationToken cancellationToken = default);

    ValueTask<bool> VarifyHashAsync(string password, string hash, CancellationToken cancellationToken = default);
}
