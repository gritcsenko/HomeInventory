using BCrypt.Net;
using HomeInventory.Application.Interfaces.Authentication;
using Crypt = BCrypt.Net.BCrypt;

namespace HomeInventory.Infrastructure.Services;

internal class BCryptPasswordHasher : IPasswordHasher
{
    private readonly HashType _hashType = HashType.SHA512;
    private readonly int _workFactor;
    private readonly bool _enhancedEntropy;

    public BCryptPasswordHasher()
    {
        _workFactor = 13;
        _enhancedEntropy = true;
    }

    public ValueTask<string> HashAsync(string password, CancellationToken cancellationToken = default)
    {
        var salt = Crypt.GenerateSalt(_workFactor);
        var hash = Crypt.HashPassword(password, salt, _enhancedEntropy, _hashType);
        return ValueTask.FromResult(hash);
    }

    public ValueTask<bool> VarifyHashAsync(string password, string hash, CancellationToken cancellationToken = default)
    {
        var result = Crypt.Verify(password, hash, _enhancedEntropy, _hashType);
        return ValueTask.FromResult(result);
    }
}
