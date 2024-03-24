using System.Text;
using HomeInventory.Web.Framework;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Web.Configuration;

internal sealed class JwtOptions : IOptions
{
    public static SectionPath Section { get; } = nameof(JwtOptions);

    private SecurityKey? _securityKey;
    private byte[]? _key;

    public required string Secret { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public string Algorithm { get; init; } = SecurityAlgorithms.HmacSha256;

    public TimeSpan Expiry { get; init; } = TimeSpan.FromMinutes(10);

    public TimeSpan ClockSkew { get; init; } = TimeSpan.FromSeconds(5);

    public SecurityKey SecurityKey => _securityKey ??= new SymmetricSecurityKey(Key);

    private byte[] Key => _key ??= Encoding.UTF8.GetBytes(Secret);
}
