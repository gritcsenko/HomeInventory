using BCrypt.Net;
using HomeInventory.Application.Interfaces.Authentication;
using static BCrypt.Net.BCrypt;

namespace HomeInventory.Infrastructure.Services;

internal class BCryptPasswordHasher : IPasswordHasher
{
    private readonly HashType _hashType;
    private readonly bool _enhancedEntropy;

    public BCryptPasswordHasher()
    {
        WorkFactor = 13;
        _enhancedEntropy = true;
        _hashType = HashType.SHA512;
    }

    public int WorkFactor { get; init; }

    public ValueTask<string> HashAsync(string password, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(InternalHash(password));

    public ValueTask<bool> VarifyHashAsync(string password, string hash, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(InternalVerify(password, hash));

    private string InternalHash(string password)
    {
        var salt = GenerateSalt(WorkFactor);
        return HashPassword(password, salt, _enhancedEntropy, _hashType);
    }

    private bool InternalVerify(string password, string hash) =>
        Verify(password, hash, _enhancedEntropy, _hashType);
}
