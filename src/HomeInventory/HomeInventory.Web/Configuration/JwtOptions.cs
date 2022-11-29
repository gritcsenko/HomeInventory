using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace HomeInventory.Web.Configuration;

internal class JwtOptions
{
    public required string Secret { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public string Algorithm { get; init; } = SecurityAlgorithms.HmacSha256;
    public TimeSpan Expiry { get; init; } = TimeSpan.FromMinutes(10);
    public byte[] Key => Encoding.UTF8.GetBytes(Secret);
}
