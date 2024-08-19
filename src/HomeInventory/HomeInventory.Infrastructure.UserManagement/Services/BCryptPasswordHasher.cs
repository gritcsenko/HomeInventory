using BCrypt.Net;
using HomeInventory.Application.Interfaces.Authentication;
using static BCrypt.Net.BCrypt;

namespace HomeInventory.Infrastructure.Services;

internal sealed class BCryptPasswordHasher : IPasswordHasher
{
    private readonly HashType _hashType = HashType.SHA512;
    private readonly bool _enhancedEntropy = true;

    public int WorkFactor { get; init; } = 13;

    public Task<string> HashAsync(string password, CancellationToken cancellationToken = default) =>
        Task.FromResult(InternalHash(password));

    public Task<bool> VarifyHashAsync(string password, string hash, CancellationToken cancellationToken = default) =>
        Task.FromResult(InternalVerify(password, hash));

    private string InternalHash(string password)
    {
        var salt = GenerateSalt(WorkFactor);
        return HashPassword(password, salt, _enhancedEntropy, _hashType);
    }

    private bool InternalVerify(string password, string hash) =>
        Verify(password, hash, _enhancedEntropy, _hashType);
}
