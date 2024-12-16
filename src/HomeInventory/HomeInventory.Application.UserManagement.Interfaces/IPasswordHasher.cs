namespace HomeInventory.Application.UserManagement.Interfaces;

public interface IPasswordHasher
{
    Task<string> HashAsync(string password, CancellationToken cancellationToken = default);

    Task<bool> VarifyHashAsync(string password, string hash, CancellationToken cancellationToken = default);
}
